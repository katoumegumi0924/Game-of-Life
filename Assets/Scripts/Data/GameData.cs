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
