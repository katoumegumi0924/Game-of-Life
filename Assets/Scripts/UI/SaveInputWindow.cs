using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class SaveInputWindow : ManualBehavior
{
    public InputField saveNameInputField;
    public Button confirmButton;
    public Button cancelButton;

    public event Action<string> OnConfirm;

    public static bool isSaveInput = false;

    protected override void _OnOpen()
    {
        isSaveInput = true;
    }

    protected override void _OnClose()
    {
        isSaveInput = false;
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

        OnConfirm?.Invoke(inputName);
    }

    private void OnCancelClick()
    {
        _Close();
    }

}
