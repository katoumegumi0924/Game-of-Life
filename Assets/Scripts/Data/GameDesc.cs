using System.IO;
using UnityEngine;

/// <summary>
/// GameDesc：
/// </summary>
public class GameDesc
{
    public int resolutionX;
    public int resolutionY;

    public int lifeRuleIndex;

    public void Init()
    {
        resolutionX = Configs.gameOfLifeConfig.resolutionX;
        resolutionY = Configs.gameOfLifeConfig.resolutionY;

        lifeRuleIndex = 0;
    }

    public void Free()
    {
        resolutionX = 0;
        resolutionY = 0;

        lifeRuleIndex = 0;
    }

    public void Export(BinaryWriter w)
    {
        w.Write(0);

        w.Write(resolutionX);
        w.Write(resolutionY);
        w.Write(lifeRuleIndex);
    }

    public void Import(BinaryReader r)
    {
        r.ReadInt32();

        resolutionX = r.ReadInt32();
        resolutionY = r.ReadInt32();

        lifeRuleIndex = r.ReadInt32();
    }
}
