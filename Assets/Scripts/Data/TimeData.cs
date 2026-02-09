using UnityEngine;

public class TimeData
{
    public long tickCounter;

    private int tickDeltaIndex;
    public int tickDelta { get { return _pausing ? 0 : tickDeltaSteps[tickDeltaIndex]; } }
    private readonly static int[] tickDeltaSteps = { 1, 2, 10, 15, 20, 40 };
    private bool _pausing;

    public void Init()
    {
        tickCounter = 0L;
        tickDeltaIndex = 1;  
    }

    public void Free()
    {
        tickCounter = 0L;
        tickDeltaIndex = 1;
    }

    public void SetNew()
    {
        tickCounter = 0L;
        tickDeltaIndex = 1;
    }

    public void SpeedUp()
    {
        tickDeltaIndex++;
        if (tickDeltaIndex >= tickDeltaSteps.Length)
            tickDeltaIndex = tickDeltaSteps.Length - 1;
    }

    public void SlowDown()
    {
        tickDeltaIndex--;
        if (tickDeltaIndex < 0)
            tickDeltaIndex = 0;
    }

    public void TogglePause()
    {
        _pausing = !_pausing;
    }

    public void Pause()
    {
        _pausing = true;
    }

    public void UnPause()
    {
        _pausing = false;
    }
}
