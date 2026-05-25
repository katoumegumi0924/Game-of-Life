using UnityEngine;

public class GameLogic
{
    private GameData gameData;

    public LifeLogic lifeLogic;
    public TimeLogic lifeTimeLogic;
    public CameraController cameraController;
    public PlayerController playerController;

    public void Init(GameData _gameData)
    {
        gameData = _gameData;

        lifeLogic = new LifeLogic();
        lifeLogic.Init(gameData);

        lifeTimeLogic = new TimeLogic();
        lifeTimeLogic.Init(gameData);

        cameraController = new CameraController();
        cameraController.Init();

        playerController = new PlayerController();
        playerController.Init(gameData);
    }

    public void Free()
    {
        gameData = null;

        if (lifeLogic != null)
        {
            lifeLogic.Free();
            lifeLogic = null;
        }

        if (lifeTimeLogic != null)
        {
            lifeTimeLogic.Free();
            lifeTimeLogic = null;
        }

        if (cameraController != null)
        {
            cameraController.Free();
            cameraController = null;
        }

        if (playerController != null)
        {
            playerController.Free();
            playerController = null;
        }
    }

    public void SetNew()
    {
        lifeLogic.SetNew();
        lifeTimeLogic.SetNew();
        playerController.SetNew();
        cameraController.SetNew();   
    }

    public void AfterImport()
    {
        lifeLogic.AfterImport();
    }

    public void OnUpdate()
    {
        cameraController.OnUpdate();
        playerController.OnUpdate();
    }

    public void GameTick()
    {
        lifeTimeLogic.EarlyTick();

        lifeLogic.OnUpdate();

        lifeTimeLogic.LateTick();
    }
}
