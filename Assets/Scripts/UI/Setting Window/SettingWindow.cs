using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine;

public class SettingWindow : ManualBehavior, IPointerEnterHandler, IPointerExitHandler
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

    public event Action<bool> OnGridToggle;
    public event Action OnPlayButtonClick;
    public event Action OnStepForwardClick;
    public event Action OnRestButtonClick;
    public event Action OnSpeedUpButtonClick;
    public event Action OnSlowDownButtonClick;

    public event Action OnClearButtonClick;
    public event Action OnSaveButtonClick;
    public event Action OnLoadButtonClick;

    private bool showGrid;

    public static bool isHoveringSettingWindow = false;

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
        showGrid = !showGrid;

        OnGridToggle?.Invoke(showGrid);
    }

    private void OnClickPlayButton()
    {
        OnPlayButtonClick?.Invoke();
    }

    private void OnClickStepForwardButton()
    {
        OnStepForwardClick?.Invoke();
    }

    private  void OnClickResetButton()
    {
        OnRestButtonClick?.Invoke();
    }

    private void OnClickSpeedUpButton()
    {
        OnSpeedUpButtonClick?.Invoke();
    }

    private void OnClickSlowDownButton()
    {
        OnSlowDownButtonClick?.Invoke();
    }

    private void OnClickClearButton()
    {
        OnClearButtonClick?.Invoke();
    }

    private void OnClickSaveButton()
    {
        OnSaveButtonClick?.Invoke();
    }

    private void OnClickLoadButton()
    {
        OnLoadButtonClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHoveringSettingWindow = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHoveringSettingWindow = false;
    }
}
