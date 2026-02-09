using System.IO;
using UnityEngine;

public class SaveLogic
{
    public void Init()
    {

    }

    public void Free()
    {

    }

    public void SaveToPNG(RenderTexture sourceTex, string savePath)
    {
        int width = sourceTex.width;
        int height = sourceTex.height;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // 从GPU读取数据
        RenderTexture.active = sourceTex;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();

        File.WriteAllBytes(savePath, bytes);

        Debug.Log($"当前迭代状态已保存到：{savePath}");
    }

    public void LoadFromPNG(RenderTexture targetTex, string savePath)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("存档文件不存在！");
            return;
        }

        byte[] bytes = File.ReadAllBytes(savePath);
        Texture2D tex = new Texture2D(2, 2);
        // 存档图片格式校验，存档图片需要是png
        if (tex.LoadImage(bytes))
        {
            Texture2D canvasTex = new Texture2D(targetTex.width, targetTex.height, TextureFormat.RGB24, false);

            Debug.Log("加载成功！");
        }
        else
        {
            Debug.LogError("加载失败，图片格式错误！");
        }

        Texture.Destroy(tex);
    }

    public void SaveToBinary(RenderTexture sourceTex, string savePath)
    {
        int width = sourceTex.width;
        int height = sourceTex.height;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // 从GPU读取数据
        RenderTexture.active = sourceTex;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        Color[] pixels = tex.GetPixels();
        Texture2D.Destroy(tex);

        using (FileStream fs = new FileStream(savePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            // 存储元数据
            writer.Write("GOL_SAVE");
            writer.Write(width);
            writer.Write(height);

            // 存储细胞数据
            byte currentByte = 0;
            int bitIndex = 0;

            for (int i = 0; i < pixels.Length; ++i)
            {
                bool isAlive = pixels[i].r < 0.5f;

                // 存活状态设置为1
                if (isAlive)
                {
                    currentByte |= (byte)(1 << bitIndex);
                }

                bitIndex++;

                if (bitIndex == 8 || i == pixels.Length - 1)
                {
                    writer.Write(currentByte);
                    currentByte = 0;
                    bitIndex = 0;
                }
            }
        }

        Debug.Log($"二进制存档完成，当前状态已保存到 {savePath}");
    }

    public void LoadFromBinary(RenderTexture targetTex, string savePath)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("存档文件不存在！");
            return;
        }

        using (FileStream fs = new FileStream(savePath, FileMode.Open))
        using (BinaryReader reader = new BinaryReader(fs))
        {
            string head = reader.ReadString();
            if (head != "GOL_SAVE")
            {
                Debug.LogError("存档文件格式错误");
                return;
            }

            int saveWidth = reader.ReadInt32();
            int saveHeight = reader.ReadInt32();

            int totalPixels = saveHeight * saveWidth;
            int byteCount = Mathf.CeilToInt(totalPixels / 8.0f);
            byte[] fileBytes = reader.ReadBytes(byteCount);

            int targetWidth = targetTex.width;
            int targetHeight = targetTex.height;
            Color[] targetPixels = new Color[targetHeight * targetWidth];

            // 初始化为全白
            for (int i = 0; i < targetPixels.Length; i++)
                targetPixels[i] = Color.white;

            // 计算不同分辨下的偏移
            int offsetX = (targetWidth - saveWidth) / 2;
            int offsetY = (targetHeight - saveHeight) / 2;

            // 遍历每一个像素 计算位置
            for (int y = 0; y < saveHeight; y++)
            {
                for (int x = 0; x < saveWidth; x++)
                {
                    int drawX = x + offsetX;
                    int drawY = y + offsetY;

                    // 边界检查
                    if (drawX >= 0 && drawX < targetWidth && drawY >= 0 && drawY < targetHeight)
                    {
                        int saveIndex = y * saveWidth + x;

                        int byteIndex = saveIndex / 8;
                        int bitOffset = saveIndex % 8;

                        if (byteIndex < fileBytes.Length)
                        {
                            bool isAlive = ((fileBytes[byteIndex] >> bitOffset) & 1) == 1;

                            // 活细胞变为黑色
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
        }
        Debug.Log("存档加载完成！");
    }

}
