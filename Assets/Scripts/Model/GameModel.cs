using UnityEngine;

public class GameModel
{ 
    public LifeRenderer gameOfLifeRenderer;

    public void Init(GameData gameData, GameLogic gameLogic)
    {
        gameOfLifeRenderer = new LifeRenderer();
        gameOfLifeRenderer.Init(gameData, gameLogic);
    }

    public void Free()
    {
        if (gameOfLifeRenderer != null)
        {
            gameOfLifeRenderer.Free();
            gameOfLifeRenderer = null;
        }
    }

    public void OnUpdate()
    {
        gameOfLifeRenderer.OnUpdate();
    }
}
