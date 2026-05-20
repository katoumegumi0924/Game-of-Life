using UnityEngine;

public class UIRoot : ManualBehavior
{
    private static UIRoot _instance;

    public static UIRoot instance { get { return _instance; } }

    [Header("Canvases")]
    public Canvas overlayCanvas;
    public Canvas worldCanvas;

    [Header("Panel")]
    public IterationRulesWindow iterationRulesPanel;
    public SettingWindow settingWindow;
    
    protected override void _OnCreate()
    {
        _instance = this;
        iterationRulesPanel._Create();
        settingWindow._Create();
    }

    protected override void _OnDestroy()
    {
        _instance = null;
        iterationRulesPanel._Destroy();
        settingWindow._Destroy();
    }

    protected override bool _OnInit()
    {
        iterationRulesPanel._Init(null);
        iterationRulesPanel._Open();

        settingWindow._Init(null);
        settingWindow._Open();

        return true;
    }
}
