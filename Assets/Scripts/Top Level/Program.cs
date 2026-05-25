using UnityEngine;

/// <summary>
/// LifeGame：
/// </summary>
public class Program : MonoBehaviour
{
    public static Program instance;

    [SerializeField] public GameMain gameMain;
    [SerializeField] public UIRoot uiRoot;

    public static GameDesc gameDesc;

    public static string loadFile;

    private void OnEnable()
    {
        instance = this;

        gameMain.gameObject.SetActive(false);

        uiRoot._Create();
        uiRoot._Init(null);
        uiRoot._Open();
        uiRoot.OpenMainMenuUI();

        gameDesc = new GameDesc();
        gameDesc.Init();
    }

    private void OnDisable()
    {
        if (uiRoot != null)
        {
            uiRoot.CloseMainMenuUI();
            uiRoot._Close();
            uiRoot._Free();
            uiRoot._Destroy();
        }
        
        if (gameDesc != null)
        {
            gameDesc.Free();
            gameDesc = null;
        }
    }

    private void Update()
    {
        if (uiRoot.active)
            uiRoot._Update();
    }

    public void StartGame(string _loadFile)
    {
        loadFile = _loadFile;
    }
}
