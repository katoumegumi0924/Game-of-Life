using System.IO;
using UnityEngine;

public static class GameSave
{
    public static readonly string saveExt = ".gol";

    public static void SaveToBinary(RenderTexture sourceTex, string saveName)
    {
        string path = Configs.gameOfLifeConfig.gameSaveFolder + saveName + saveExt;
        string directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

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

        using (FileStream fs = new FileStream(path, FileMode.Create))
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

        // 保存同名预览图
        SavePreviewImage(sourceTex, path);
        Debug.Log($"存档完成，当前状态已保存到 {path}");
    }

    public static void LoadFromBinary(RenderTexture targetTex, string savePath)
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

            // 遍历每一个像素 计算位置 感觉需要优化
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

    public static void SavePreviewImage(RenderTexture source, string path)
    {
        int w = (int)(Configs.gameOfLifeConfig.resolutionX);
        int h = (int)(Configs.gameOfLifeConfig.resolutionY);
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
}
