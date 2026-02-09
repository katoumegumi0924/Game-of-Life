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
    public SettingWindow settingPanel;
    
    

    protected override void _OnCreate()
    {
        _instance = this;
        iterationRulesPanel._Create();
        settingPanel._Create();
    }

    protected override void _OnDestroy()
    {
        _instance = null;
        iterationRulesPanel._Destroy();
        settingPanel._Destroy();
    }

    protected override bool _OnInit()
    {
        iterationRulesPanel._Init(null);
        iterationRulesPanel._Open();

        settingPanel._Init(null);
        settingPanel._Open();

        return true;
    }
}
