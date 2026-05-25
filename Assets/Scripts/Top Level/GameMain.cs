using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static GameMain instance { get; private set; }

    public GameData gameData;
    public GameLogic gameLogic;
    public GameModel gameModel;

    public void Init()
    {
        instance = this;

        gameData = new GameData();
        //gameData.Init();

        gameLogic = new GameLogic();
        //gameLogic.Init(gameData);

        gameModel = new GameModel();
        //gameModel.Init(gameData, gameLogic);

        NewOrLoad();
    }

    public void Free()
    {
        if (gameModel != null)
        {
            gameModel.Free();
            gameModel = null;
        }

        if (gameLogic != null)
        {
            gameLogic.Free();
            gameLogic = null;
        }

        if (gameData != null)
        {
            gameData.Free();
            gameData = null;
        }

        instance = null;
    }

    void NewOrLoad()
    {
        if (string.IsNullOrEmpty(Program.loadFile))
        {
            gameData.Init();
            gameData.SetNew();
            gameLogic.Init(gameData);
            gameLogic.SetNew();
            gameModel.Init(gameData, gameLogic);
            UIRoot.instance.OpenGameUI(this);
        }
        else
        {
            gameData.Init();
            GameSave.LoadGame(Program.loadFile, gameData);
            gameLogic.Init(gameData);
            gameLogic.AfterImport();
            gameModel.Init(gameData, gameLogic);
            UIRoot.instance.OpenGameUI(this);
        }
    }

    private void Update()
    {
        gameLogic.OnUpdate();
        gameModel.OnUpdate();
    }

    private void FixedUpdate()
    {
        gameLogic.GameTick();
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        Free();
    }
}
