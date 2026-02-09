using UnityEngine;
using UnityEngine.UI;

public class LifeRenderer
{
    private GameData gameData;
    private GameLogic gameLogic;

    private RawImage displayImage;
    private Material gridMaterial;

    private Material paintMaterial;

    public void Init(GameData _gameData, GameLogic _gameLogic)
    {
        gameData = _gameData;
        gameLogic = _gameLogic;

        displayImage = gameData.lifeData.displayImage;
        gridMaterial = displayImage.material;
        gridMaterial.SetFloat("_Resolution", Configs.gameOfLifeConfig.resolutionX);
        paintMaterial = Material.Instantiate(Configs.gameResourcesConfig.paint);

        UIRoot.instance.settingPanel.OnGridToggle += OnGridShow;
    }

    public void Free()
    {
        displayImage = null;
        gridMaterial = null;

        if (paintMaterial != null)
        {
            Material.Destroy(paintMaterial);
        }

        UIRoot.instance.settingPanel.OnGridToggle -= OnGridShow;
    }

    public void OnUpdate()
    {
        displayImage.texture = gameData.lifeData.currentTex;

        // 只有uv坐标有效时才进行绘制
        Vector2 taregtUV = gameLogic.playerController.cellUV;
        if (!float.IsNegativeInfinity(taregtUV.x))
        {
            PaintCell(gameLogic.playerController.cellUV, gameLogic.playerController.cellValue, gameLogic.playerController.brushSize);
        }
    }

    public void OnGridShow(bool showGrid)
    {
        gridMaterial.SetFloat("_ShowGrid", showGrid ? 1.0f : 0.0f);
    }

    private void PaintCell(Vector2 uv, float value, float brushSize)
    {
        int resolutionX = (int)Configs.gameOfLifeConfig.resolutionX;
        int resolutionY = (int)Configs.gameOfLifeConfig.resolutionY;

        paintMaterial.SetVector("_MousePos", uv);
        paintMaterial.SetFloat("_PaintColor", value);
        paintMaterial.SetFloat("_BrushSize", brushSize);

        float aspect = resolutionX / resolutionY;
        paintMaterial.SetFloat("_AspectRatio", aspect);

        RenderTexture temp = RenderTexture.GetTemporary(resolutionX, resolutionY, 0);
        Graphics.Blit(gameData.lifeData.currentTex, temp, paintMaterial);
        Graphics.Blit(temp, gameData.lifeData.currentTex);
        RenderTexture.ReleaseTemporary(temp);
    }

    //private void Paint()
    //{
    //    Vector2 _lastFrameUV = Vector2.negativeInfinity;

    //    Vector2 currentUV = gameLogic.playerController.cellUV;
    //    float value = gameLogic.playerController.cellValue;
    //    float brushSize = gameLogic.playerController.brushSize;

    //    bool isInputting = !float.IsNegativeInfinity(currentUV.x);

    //    if (isInputting)
    //    {
    //        // 没有上yiz
    //    }
    //}
}
