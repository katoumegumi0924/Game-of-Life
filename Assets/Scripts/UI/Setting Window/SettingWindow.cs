using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine;

public class SettingWindow : ManualBehavior
{
    [Header("Window")]
    public SaveInputWindow saveinputWindow;
    public LoadFileWindow loadFileWindow;

    [Header("Button")]
    public Button gridButton;
    public Button playButton;
    public Button resetButton;
    public Button stepForwardButton;
    public Button speedUpButton;
    public Button slowDownButton;

    public Button clearButton;
    public Button saveButton;
    public Button loadButton;

    //private bool showGrid;

    protected override void _OnCreate()
    {
        loadFileWindow._Create();
        saveinputWindow._Create();
    }

    protected override void _OnDestroy()
    {
        loadFileWindow._Destroy();
        saveinputWindow._Destroy();
    }

    protected override bool _OnInit()
    {
        loadFileWindow._Init(null);
        saveinputWindow._Init(null);

        return true;
    }

    protected override void _OnFree()
    {
        loadFileWindow._Free();
        saveinputWindow._Free();
    }

    protected override void _OnRegEvent()
    {
        gridButton.onClick.AddListener(OnClickGridButton);
        playButton.onClick.AddListener(OnClickPlayButton);
        stepForwardButton.onClick.AddListener(OnClickStepForwardButton);
        speedUpButton.onClick.AddListener(OnClickSpeedUpButton);
        slowDownButton.onClick.AddListener(OnClickSlowDownButton);

        resetButton.onClick.AddListener(OnClickResetButton);
        clearButton.onClick.AddListener(OnClickClearButton);
        saveButton.onClick.AddListener(OnClickSaveButton);
        loadButton.onClick.AddListener(OnClickLoadButton);
    }

    protected override void _OnUnregEvent()
    {
        gridButton.onClick.RemoveListener(OnClickGridButton);
        playButton.onClick.RemoveListener(OnClickPlayButton);
        stepForwardButton.onClick.RemoveListener(OnClickStepForwardButton);
        speedUpButton.onClick.RemoveListener(OnClickSpeedUpButton);
        slowDownButton.onClick.RemoveListener(OnClickSlowDownButton);

        resetButton.onClick.RemoveListener(OnClickResetButton);
        clearButton.onClick.RemoveListener(OnClickClearButton);
        saveButton.onClick.RemoveListener(OnClickSaveButton);
        loadButton.onClick.RemoveListener(OnClickLoadButton);
    }

    private void OnClickGridButton()
    {
        GameMain.instance.gameData.lifeData.showGrid = !GameMain.instance.gameData.lifeData.showGrid;
    }

    private void OnClickPlayButton()
    {
        GameMain.instance.gameData.lifeTimeData.TogglePause();
    }

    private void OnClickStepForwardButton()
    {
        GameMain.instance.gameData.lifeTimeData.Pause();
        GameMain.instance.gameLogic.lifeLogic.LifeUpdate();
    }

    private  void OnClickResetButton()
    {
        GameMain.instance.gameData.lifeData.ResetTexture();
    }

    private void OnClickSpeedUpButton()
    {
        GameMain.instance.gameData.lifeTimeData.UnPause();
        GameMain.instance.gameData.lifeTimeData.SpeedUp();
    }

    private void OnClickSlowDownButton()
    {
        GameMain.instance.gameData.lifeTimeData.UnPause();
        GameMain.instance.gameData.lifeTimeData.SlowDown();
    }

    private void OnClickClearButton()
    {
        GameMain.instance.gameLogic.lifeLogic.OnClearLife();
    }

    private void OnClickSaveButton()
    {
        string fileName = $"save_{System.DateTime.Now:yyyyMMdd_HHmmss}";
        saveinputWindow.saveNameInputField.text = fileName;
        saveinputWindow._Open();
    }

    private void OnClickLoadButton()
    {
        if (!loadFileWindow.active)
        {
            loadFileWindow._Open();
        }
        else
        {
            loadFileWindow._Close();
        }
    }
}
