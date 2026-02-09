using UnityEngine;

[CreateAssetMenu(fileName = "LifeRuleSetConfig", menuName = "GameOfLife/GameConfig/LifeRuleSetConfig")]
public class LifeRuleSetConfig : ScriptableObject
{
    [SerializeField]
    private LifeRuleConfig[] rules;

    // 获取规则
    public LifeRuleConfig GetLifeRule(int index)
    {
        if (index < 0 || index >= rules.Length)
            return null;
        return rules[index];
    }

    public int GetLifeRuleCount()
    {
        return rules.Length;
    }
}
