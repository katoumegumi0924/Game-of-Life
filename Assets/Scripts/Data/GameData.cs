using UnityEngine;

/// <summary>
/// GameData：
/// </summary>
public class GameData
{
    public LifeData lifeData;
    public TimeData lifeTimeData;

    public void Init()
    {
        lifeData = new LifeData();
        lifeData.Init();

        lifeTimeData = new TimeData();
        lifeTimeData.Init();
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
    }

    public void Export(System.IO.BinaryWriter w)
    {
        w.Write((byte)0);

        lifeTimeData.Export(w);
        lifeData.Export(w);
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
    }
}
