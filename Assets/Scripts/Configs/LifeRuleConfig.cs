using UnityEngine;

[CreateAssetMenu(fileName = "LifeRuleConfig", menuName = "GameOfLife/GameConfig/LifeRuleConfig")]
public class LifeRuleConfig : ScriptableObject
{
    [Header("Basic Info")]
    public string ruleName;
    [TextArea] public string Description;

    [Header("Conditions")]
    [Tooltip("重生条件（B）：勾选表示当有N个邻居时重生")]
    public bool[] birth = new bool[9];
    [Tooltip("存活条件（S）：勾选表示当有N个邻居时存活")]
    public bool[] survival = new bool[9];

    public int birthMask { get { return BoolArrayToMask(birth); } }
    public int survivalMask { get { return BoolArrayToMask(survival); } }
    
    private int BoolArrayToMask(bool[] conditions)
    {
        int mask = 0;
        for (int i = 0; i < conditions.Length; i++)
        {
            if (conditions[i])
            {
                mask |= (1 << i);
            }
        }
        return mask;
    }

    // 自动校验条件数组长度
    private void OnValidate()
    {
        if (survival.Length != 9)
            System.Array.Resize(ref survival, 9);

        if (birth.Length != 9)
            System.Array.Resize(ref birth, 9);
    }

    // 重写 ToString 方便调试
    public override string ToString()
    {
        return $"{ruleName} (B{string.Join("", birthMask)}/S{string.Join("", survivalMask)})";
    }
}
