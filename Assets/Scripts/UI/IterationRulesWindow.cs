using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class IterationRulesWindow : ManualBehavior, IPointerEnterHandler, IPointerExitHandler
{
    public Dropdown rulesDropDown;

    public event Action<LifeRuleConfig> OnRuleSelected;

    public static bool isHoveringIterationWindow;

    protected override bool _OnInit()
    {
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

    private void OnDropdownValueChanged(int index)
    {
        var ruleSet = Configs.lifeRuleSet;

        if (ruleSet == null || index < 0 || index >= ruleSet.GetLifeRuleCount())
            return;

        LifeRuleConfig lifeRule = ruleSet.GetLifeRule(index);

        OnRuleSelected?.Invoke(lifeRule);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHoveringIterationWindow = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHoveringIterationWindow = false;
    }
}
