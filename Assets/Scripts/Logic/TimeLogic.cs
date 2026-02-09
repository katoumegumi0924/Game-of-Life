using UnityEngine;

public class TimeLogic
{
    public GameData gameData;
    public TimeData time;

    public void Init(GameData _gameData)
    {
        gameData = _gameData;
        time = gameData.lifeTimeData;   
    }

    public void Free()
    {
        gameData = null;
        time = null; 
    }

    public void SetNew()
    {

    }

    public void EarlyTick()
    {
        time.tickCounter += time.tickDelta;
    }

    public void LateTick()
    {

    }
}
