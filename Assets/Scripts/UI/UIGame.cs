using System;
using UnityEngine;

public class UIGame : ManualBehavior
{
    public UIIterationRules uiIterationRules;
    public UISettingMenu uiSettingMenu;

    [NonSerialized]
    public GameMain gameMain;

    protected override void _OnCreate()
    {
        uiIterationRules._Create();
        uiSettingMenu._Create();
    }

    protected override void _OnDestroy()
    {
        uiIterationRules._Destroy();
        uiSettingMenu._Destroy();
    }

    protected override bool _OnInit()
    {
        gameMain = data as GameMain;

        uiIterationRules._Init(gameMain);
        uiSettingMenu._Init(gameMain);
        return true;
    }

    protected override void _OnFree()
    {
        uiIterationRules._Free();
        uiSettingMenu._Free();
    }

    protected override void _OnOpen()
    {
        uiIterationRules._Open();
        uiSettingMenu._Open();
    }

    protected override void _OnClose()
    {
        uiIterationRules._Close();
        uiSettingMenu._Close();
    }

    protected override void _OnUpdate()
    {
       
    }
}
