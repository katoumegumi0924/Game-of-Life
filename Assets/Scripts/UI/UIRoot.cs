using UnityEngine;

/// <summary>
/// UIRoot：
/// </summary>
public class UIRoot : ManualBehavior
{
    private static UIRoot _instance;

    public static UIRoot instance { get { return _instance; } }

    [SerializeField] public UIGame uiGame;
    [SerializeField] public UIMainMenu uiMainMenu;

    protected override void _OnCreate()
    {
        _instance = this;

        uiGame._Create();
        uiMainMenu._Create();
    }

    protected override void _OnDestroy()
    {
        _instance = null;

        uiGame._Free();
        uiMainMenu._Free();
    }

    public void OpenGameUI(GameMain _gameMain)
    {
        uiGame._Init(_gameMain);
        uiGame._Open();
    }

    public void CloseGameUI()
    {
        uiGame._Close();
    }

    public void OpenMainMenuUI()
    {
        uiMainMenu._Init(null);
        uiMainMenu._Open();
    }

    public void CloseMainMenuUI()
    {
        uiMainMenu._Close();
    }
}
