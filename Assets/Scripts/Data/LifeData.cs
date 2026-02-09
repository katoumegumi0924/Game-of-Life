using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LifeData
{
    // 双缓存纹理 交替读写
    public RenderTexture texA;
    public RenderTexture texB;
    public bool useTexA = true;
    public RenderTexture currentTex { get { return useTexA ? texA : texB; } }
    public ComputeShader shader;
    public uint resolutionY = Configs.gameOfLifeConfig.resolutionY;
    public uint resolutionX = Configs.gameOfLifeConfig.resolutionX;

    private GameObject displayImageObj;
    public RawImage displayImage;

    public RenderTexture initTex;

    public void Init()
    {
        shader = Configs.gpuConfig.gameOfLifeShader;

        texA = CreateRenderTexture();
        texB = CreateRenderTexture();

        displayImageObj = CreateDisplayObj();
        displayImage = displayImageObj.GetComponent<RawImage>();
    }

    public void SetNew()
    {
        SeedTexture(texA);
        SetInitTexture(texA);
    }

    public void Free()
    {
        if (texA != null)
        {
            texA.Release();
            texA = null;
        }

        if (texB != null)
        {
            texB.Release();
            texB = null;
        }

        if (displayImageObj != null)
        {
            displayImage = null;
            GameObject.Destroy(displayImageObj);
        }
    }

    public void Swap()
    {
        useTexA = !useTexA;
    }

    private RenderTexture CreateRenderTexture()
    {
        RenderTexture rt = new RenderTexture((int)resolutionX, (int)resolutionY, 0);
        rt.enableRandomWrite = true;
        rt.filterMode = FilterMode.Point;
        rt.wrapMode = TextureWrapMode.Repeat;
        rt.Create();
        return rt;
    }

    private GameObject CreateDisplayObj()
    {
        GameObject prefab = Configs.gameResourcesConfig.displayImagePrefab;
        Canvas canvas = UIRoot.instance.worldCanvas;
        return GameObject.Instantiate(prefab, canvas.transform, false);   
    }

    // 调用 ComputeShader 进行随机填充初始化
    private void SeedTexture(RenderTexture target)
    {
        if (shader == null)
            return;

        int kernel = shader.FindKernel("CSInitRandom");

        shader.SetTexture(kernel, "OutputTex", target);
        shader.SetVector("resolution", new Vector2(resolutionX, resolutionY));
        shader.SetFloat("seed", Random.Range(0f, 10f));

        int groupsX = Mathf.CeilToInt(resolutionX / 8.0f);
        int groupsY = Mathf.CeilToInt(resolutionY / 8.0f);
        shader.Dispatch(kernel, groupsX, groupsY, 1);
#if UNITY_EDITOR
        // 调试代码
        Texture2D tex = RenderTextureToTexture2D(target);
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes("Assets/Resources/test.png", bytes);
#endif
    }

    public static Texture2D RenderTextureToTexture2D(RenderTexture rt)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(
            rt.width,
            rt.height,
            TextureFormat.RGBA32,
            false,
            false        // linear = false（重要）
        );

        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        RenderTexture.active = prev;
        return tex;
    }

    public void SetInitTexture(RenderTexture source)
    {
        if (source == null)
            return;

        if (initTex == null)
        {
            initTex = new RenderTexture(source.descriptor);
            initTex.enableRandomWrite = true;
            initTex.Create();
        }

        Graphics.Blit(source, initTex);
    }

    public void ResetTexture()
    {
        useTexA = true;
        Graphics.Blit(initTex, texA);
    }
}
