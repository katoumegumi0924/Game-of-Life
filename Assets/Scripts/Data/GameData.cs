using UnityEngine;

/// <summary>
/// GameData：
/// </summary>
public class GameData
{
    public LifeData lifeData;
    public TimeData lifeTimeData;

    public GameDesc gameDesc;

    public void Init()
    {
        lifeData = new LifeData();
        lifeData.Init();

        lifeTimeData = new TimeData();
        lifeTimeData.Init();

        gameDesc = Program.gameDesc;
    }

    public void Free()
    {
        if (lifeData != null)
        {
            lifeData.Free();
            lifeData = null;
        }

        if (lifeTimeData != null)
        {
            lifeTimeData.Free();
            lifeTimeData = null;
        }

        gameDesc = null;
    }

    public void SetNew()
    {
        lifeData.SetNew();
        lifeTimeData.SetNew();
    }

    public void Import(System.IO.BinaryReader r)
    {
        int ver = r.ReadByte();

        lifeTimeData.Import(r);
        lifeData.Import(r);
        gameDesc.Import(r);
    }

    public void Export(System.IO.BinaryWriter w)
    {
        w.Write((byte)0);

        lifeTimeData.Export(w);
        lifeData.Export(w);
        gameDesc.Export(w);
    }
}
