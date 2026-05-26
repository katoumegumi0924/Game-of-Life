using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class UIIterationRules : ManualBehavior
{
    public Dropdown rulesDropDown;
    public Text textDesc;

    GameMain gameMain;

    protected override bool _OnInit()
    {
        gameMain = data as GameMain;
        if (gameMain == null)
            return false;

        rulesDropDown.ClearOptions();

        var ruleSet = Configs.lifeRuleSet;
        if (ruleSet == null)
            return false;

        for (int i = 0; i < ruleSet.GetLifeRuleCount(); ++i)
        {
            rulesDropDown.options.Add(new Dropdown.OptionData(ruleSet.GetLifeRule(i).ruleName));
        }

        return true;
    }

    protected override void _OnFree()
    {
        rulesDropDown.ClearOptions();
    }

    protected override void _OnRegEvent()
    {
        rulesDropDown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    protected override void _OnUnregEvent()
    {
        rulesDropDown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    protected override void _OnOpen()
    {
        var ruleSet = Configs.lifeRuleSet;
        int index = gameMain.gameData.lifeData.currentRuleIndex;

        textDesc.text = ruleSet.GetLifeRule(index).Description;
    }

    protected override void _OnClose()
    {
        
    }

    private void OnDropdownValueChanged(int index)
    {
        var ruleSet = Configs.lifeRuleSet;

        if (ruleSet == null || index < 0 || index >= ruleSet.GetLifeRuleCount())
            return;

        var lifeData = gameMain.gameData.lifeData;
        lifeData.currentRuleIndex = index;
        var lifeLogic = gameMain.gameLogic.lifeLogic;
        lifeLogic.lifeRule = ruleSet.GetLifeRule(lifeData.currentRuleIndex);

        textDesc.text = ruleSet.GetLifeRule(index).Description;
    }
}
