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
        gameDesc = Program.gameDesc;

        lifeData = new LifeData();
        lifeData.Init(this);

        lifeTimeData = new TimeData();
        lifeTimeData.Init(); 
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
        gameDesc.Import(r);
        lifeData.Import(r);
        
        // lifeData.Init(this);
    }

    public void Export(System.IO.BinaryWriter w)
    {
        w.Write((byte)0);

        lifeTimeData.Export(w);
        gameDesc.Export(w);
        lifeData.Export(w);
    }
}
