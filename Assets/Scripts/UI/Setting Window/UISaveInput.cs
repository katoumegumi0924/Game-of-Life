using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class UISaveInput : ManualBehavior
{
    public InputField saveNameInputField;
    public Button confirmButton;
    public Button cancelButton;

    //public static bool isSaveInput = false;

    protected override void _OnOpen()
    {
        //isSaveInput = true;

        GameMain.instance.gameData.lifeTimeData.Pause();
    }

    protected override void _OnClose()
    {
        //isSaveInput = false;

        GameMain.instance.gameData.lifeTimeData.UnPause();
    }

    protected override void _OnRegEvent()
    {
        confirmButton.onClick.AddListener(OnConfirmClick);
        cancelButton.onClick.AddListener(OnCancelClick);
    }

    protected override void _OnUnregEvent()
    {
        confirmButton.onClick.RemoveListener(OnConfirmClick);
        cancelButton.onClick.RemoveListener(OnCancelClick);
    }

    private void OnConfirmClick()
    {
        string inputName = saveNameInputField.text;

        // 存档名称不能为空
        if (string.IsNullOrWhiteSpace(inputName))
        {
            Debug.LogWarning("存档名称不能为空");
            return;
        }

        GameSave.SaveGame(inputName, GameMain.instance.gameData);
        UIRoot.instance.settingWindow.loadFileWindow.RefreshList();
        _Close();
    }

    private void OnCancelClick()
    {
        _Close();
    }

}
