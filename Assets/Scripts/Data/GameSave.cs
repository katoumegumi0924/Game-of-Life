using System.IO;
using UnityEngine;

public static class GameSave
{
    public static readonly string saveExt = ".gol";

    public static void SaveGame(string saveName, GameData gameData)
    {
        string fileName = Configs.gameOfLifeConfig.gameSaveFolder + saveName + saveExt;

        using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            using (BinaryWriter w = new BinaryWriter(fs))
            {
                gameData.Export(w);
                // 保存同名的预览图
                LifeData.SavePreviewImage(gameData.lifeData.currentTex, fileName);
            }
        }
    }

    public static void LoadGame(string saveName, GameData gameData)
    {
        //string fileName = Configs.gameOfLifeConfig.gameSaveFolder + saveName + saveExt;

        using (FileStream fs = new FileStream(saveName, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            using (BinaryReader r = new BinaryReader(fs))
            {
                gameData.Import(r);
            }
        }
    }
}
