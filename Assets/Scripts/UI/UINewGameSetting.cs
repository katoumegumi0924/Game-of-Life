using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIGameDesc：
/// </summary>
public class UINewGameSetting : ManualBehavior
{
    [SerializeField] public InputField widthInput;
    [SerializeField] public InputField heightInput;

    [SerializeField] public UIButton confirmBtn;
    [SerializeField] public UIButton cancelBtn;

    [SerializeField] public GameMain gameMain;

    protected override bool _OnInit()
    {
        return true;
    }

    protected override void _OnFree()
    {

    }

    protected override void _OnRegEvent()
    {
        confirmBtn.onClick += OnConfirmBtnClick;
        cancelBtn.onClick += OnCancelBtnClick;
    }

    protected override void _OnUnregEvent()
    {
        confirmBtn.onClick -= OnConfirmBtnClick;
        cancelBtn.onClick -= OnCancelBtnClick;
    }

    public void OnConfirmBtnClick(int data)
    {
        var gameDesc = Program.gameDesc;
        string widthText = widthInput.text;
        if (string.IsNullOrEmpty(widthText) || string.IsNullOrWhiteSpace(widthText))
        {
            gameDesc.resolutionX = Configs.gameOfLifeConfig.resolutionX;
        }
        else
        {
            if (int.TryParse(widthText, out int value))
            {
                gameDesc.resolutionX = (uint)value;
            }
        }

        string heightText = heightInput.text;
        if (string.IsNullOrEmpty(heightText) || string.IsNullOrWhiteSpace(heightText))
        {
            gameDesc.resolutionY = Configs.gameOfLifeConfig.resolutionY;
        }
        else
        {
            if (int.TryParse(heightText, out int value))
            {
                gameDesc.resolutionY = (uint)value;
            }
        }

        _Close();
        UIRoot.instance.uiMainMenu._Close();
        gameMain.gameObject.SetActive(true);
    }

    public void OnCancelBtnClick(int data)
    {
        _Close();
    }
}
