using UnityEngine;

[CreateAssetMenu(fileName = "GameOfLifeConfig", menuName = "GameOfLife/GameConfig/GameOfLifeConfig")]
public class GameOfLifeConfig : ScriptableObject
{
    [Header("纹理分辨率")]
    public int resolutionY = 888;
    public int resolutionX= 1580;

    [Header("迭代间隔Tick")]
    public int singleStepTick = 50;

    [Header("存档文件路径")]
    public string gameSaveFolder = "F:\\Cases\\Game Of Life\\Save\\";
}
