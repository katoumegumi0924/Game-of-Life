using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LifeData
{
    public GameData gameData;

    public int[] texA;
    public int[] texB;
    public bool useTexA = true;
    
    public int currentRuleIndex;

    public int resX;
    public int resY;

    public int totalPixel
    {
        get
        {
            return resX* resY;
        }
    }

    private bool _showGrid;
    public bool showGrid
    {
        get 
        { 
            return _showGrid; 
        }

        set
        {
            _showGrid = value;
        }
    }

    public int[] currentTex { get { return useTexA ? texA : texB; } }
    public ComputeShader lifeShader;

    // 初始状态，初始化与加载存档后更新
    public int[] initTex;

    public void Init(GameData _gameData)
    {
        gameData = _gameData;

        lifeShader = Configs.gpuConfig.lifeShader;
        
    }

    public void Free()
    {
        lifeShader = null;

        texA = null;
        texB = null;
        initTex = null;

        showGrid = false;
    }

    public void SetSize(int resX, int resY)
    {
        this.resX = resX;
        this.resY = resY;

        texA = new int[totalPixel];
        texB = new int[totalPixel];
    }

    public void SetNew()
    {
        resX = gameData.gameDesc.resolutionX;
        resY = gameData.gameDesc.resolutionY;

        SetSize(resX, resY);

        //SeedTexture(texA);
        //SetInitTexture(texA);

        texA[0] = 1;
        texA[1] = 1;
        texA[2] = 1;
        texA[3] = 1;

        currentRuleIndex = 0;
        showGrid = false;
    }

    public void Import(System.IO.BinaryReader r)
    {
        r.ReadInt32();

        resX = r.ReadInt32();
        resY = r.ReadInt32();

        SetSize(resX, resY);

        currentRuleIndex = r.ReadInt32();
        showGrid = r.ReadBoolean();
    }

    public void Export(System.IO.BinaryWriter w)
    {
        w.Write(0);

        w.Write(resX);
        w.Write(resY);
        w.Write(currentRuleIndex);
        w.Write(showGrid);
    }

    public void Swap()
    {
        useTexA = !useTexA;
    }

    private RenderTexture CreateRenderTexture()
    {
        RenderTexture rt = new RenderTexture((int)resX, (int)resY, 0);
        rt.enableRandomWrite = true;
        rt.filterMode = FilterMode.Point;
        rt.wrapMode = TextureWrapMode.Repeat;
        rt.Create();
        return rt;
    }

    // 调用 ComputeShader 进行随机填充初始化
    private void SeedTexture(RenderTexture target)
    {
        if (lifeShader == null)
            return;

        int kernel = lifeShader.FindKernel("CSInitRandom");

        lifeShader.SetTexture(kernel, "OutputTex", target);
        lifeShader.SetVector("resolution", new Vector2(resX, resY));
        lifeShader.SetFloat("seed", Random.Range(0f, 10f));

        int groupsX = Mathf.CeilToInt(resX / 8.0f);
        int groupsY = Mathf.CeilToInt(resY / 8.0f);
        lifeShader.Dispatch(kernel, groupsX, groupsY, 1);
    }

    // 设置初始状态
    public void SetInitTexture(int[] source)
    {
        if (source == null)
            return;

        if (initTex == null)
        {
            initTex = source;
        }
    }

    // 重置当前状态为初始状态
    public void ResetTexture()
    {
        useTexA = true;
        texA = initTex;
        texB = initTex;
    }

    // 保存预览图
    public static void SavePreviewImage(RenderTexture source, string path)
    {
        int w = (int)(GameMain.instance.gameData.gameDesc.resolutionX);
        int h = (int)(GameMain.instance.gameData.gameDesc.resolutionY);
        RenderTexture preRenderTexture = RenderTexture.GetTemporary(w, h, 0);

        Graphics.Blit(source, preRenderTexture);

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        RenderTexture.active = preRenderTexture;
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        string imgPath = Path.ChangeExtension(path, ".PNG");
        File.WriteAllBytes(imgPath, bytes);

        RenderTexture.ReleaseTemporary(preRenderTexture);
        Texture2D.Destroy(tex);
    }

    // 加载预览图
    public static Texture2D LoadPreviewImage(string path)
    {
        string imgPath = Path.ChangeExtension(path, ".PNG");

        if (File.Exists(imgPath))
        {
            byte[] bytes = File.ReadAllBytes(imgPath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            return tex;
        }

        return null;
    }

    // 保存当前迭代状态
    public void RTSaveToBinary(System.IO.BinaryWriter w, RenderTexture sourceTex)
    {
        int width = sourceTex.width;
        int height = sourceTex.height;

        Texture2D tex = new Texture2D(width, height);
        // 从gpu读取数据
        RenderTexture.active = sourceTex;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        Color[] pixels = tex.GetPixels();
        Texture2D.Destroy(tex);

        // 开始写入数据
        w.Write("GOL_SAVE");
        w.Write(width);
        w.Write(height);

        // 写入细胞数据
        byte currentByte = 0;
        int bitIndex = 0;
        for (int i = 0; i < pixels.Length; ++i)
        {
            bool isAlive = pixels[i].r < 0.5f;

            if (isAlive)
            {
                currentByte |= (byte)(1 << bitIndex);
            }
            bitIndex++;

            if (bitIndex == 8 || i == pixels.Length - 1)
            {
                w.Write(currentByte);
                currentByte = 0;
                bitIndex = 0;
            }
        }

        // 保存同名的预览图
        // SavePreviewImage(sourceTex, path);

#if UNITY_EDITOR
        Debug.Log($"存档完成，当前状态已保存");
#endif
    }

    // 加载存档迭代状态
    public void RTLoadFromBinary(System.IO.BinaryReader r, RenderTexture targetTex)
    {
        string head = r.ReadString();
        if (head != "GOL_SAVE")
        {
            Debug.LogError("存档文件格式错误");
            return;
        }

        // 读取存档数据
        int saveWidth = r.ReadInt32();
        int saveHeight = r.ReadInt32();
        int totalPixels = saveWidth * saveHeight;
        int byteCount = Mathf.CeilToInt(totalPixels / 8.0f);
        byte[] fileBytes = r.ReadBytes(byteCount);

        // 处理数据
        int targetWidth = targetTex.width;
        int targetHeight = targetTex.height;
        Color[] targetPixels = new Color[targetWidth * targetHeight];

        // 初始化为全白
        for (int i = 0; i < targetPixels.Length; ++i)
            targetPixels[i] = Color.white;

        // 计算不同分辨率下的偏移
        int offsetX = (targetWidth - saveWidth) / 2;
        int offsetY = (targetHeight - saveHeight) / 2;

        // 遍历每一个像素 计算位置
        for (int y = 0; y < saveHeight; ++y)
        {
            for (int x = 0; x < saveWidth; ++x)
            {
                int drawX = x + offsetX;
                int drawY = y + offsetY;

                // 边界检查
                if (drawX >= 0 && drawX < targetWidth && drawY >= 0 && drawY < targetHeight)
                {
                    // 行优先 提高缓存命中
                    int saveIndex = y * saveWidth + x;

                    int byteIndex = saveIndex / 8;
                    int bitOffset = saveIndex % 8;

                    if (byteIndex < fileBytes.Length)
                    {
                        bool isAlive = ((fileBytes[byteIndex] >> bitOffset) & 1) == 1;

                        // 活细胞置为黑色
                        if (isAlive)
                        {
                            int targetIndex = drawY * targetWidth + drawX;
                            targetPixels[targetIndex] = Color.black;
                        }
                    }
                }
            }
        }

        // 上传到GPU
        Texture2D tex = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(targetPixels);
        tex.Apply();
        Graphics.Blit(tex, targetTex);
        Texture2D.Destroy(tex);

#if UNITY_EDITOR
        Debug.Log("存档加载完成！");
#endif
    }
}
