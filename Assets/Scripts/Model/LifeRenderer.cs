using UnityEngine;
using UnityEngine.UI;

public class LifeRenderer
{
    public GameData gameData;
    public GameLogic gameLogic;
    public GameObject displayObj;
    //public RawImage displayImage;
    public MeshRenderer displayRenderer;

    private Material gridMaterial;
    private Material paintMaterial;
    private Vector2 _lastFrameUV;

    public void Init(GameData _gameData, GameLogic _gameLogic)
    {
        gameData = _gameData;
        gameLogic = _gameLogic;

        displayObj = CreateDisplayObj();
        //displayImage = displayImageObj.GetComponent<RawImage>();
        displayRenderer = displayObj.GetComponent<MeshRenderer>();

        //gridMaterial = new Material(displayImage.material);
        gridMaterial = displayRenderer.material;
        gridMaterial.SetFloat("_ResolutionX", Configs.gameOfLifeConfig.resolutionX);
        gridMaterial.SetFloat("_ResolutionY", Configs.gameOfLifeConfig.resolutionY);
        // 默认关闭网格
        gridMaterial.SetFloat("_ShowGrid", 0.0f);
        //displayImage.material = gridMaterial;
        paintMaterial = Material.Instantiate(Configs.gameResourcesConfig.paint);
    }

    public void Free()
    {
        if (gridMaterial != null)
        {
            Material.Destroy(gridMaterial);
            gridMaterial = null;
        }

        if (paintMaterial != null)
        {
            Material.Destroy(paintMaterial);
            paintMaterial = null;
        }

        if (displayObj != null)
        {
            // displayImage = null;
            displayRenderer = null;
            GameObject.Destroy(displayObj);
        }
    }

    public void OnUpdate()
    {
        //displayImage.texture = gameData.lifeData.currentTex;

        if (gridMaterial != null)
        {
            gridMaterial.mainTexture = gameData.lifeData.currentTex;
        }

        Paint();
    }

    private GameObject CreateDisplayObj()
    {
        GameObject prefab = Configs.gameResourcesConfig.displayImagePrefab;

        //Canvas canvas = UIRoot.instance.worldCanvas;
        //var displayImage = GameObject.Instantiate(prefab, canvas.transform, false);
        //RectTransform displayRect = displayImage.transform as RectTransform;
        //displayRect.sizeDelta = new Vector2(Configs.gameOfLifeConfig.resolutionX, Configs.gameOfLifeConfig.resolutionY);

        //return displayImage;

        GameObject quadObj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);

        float resX = Configs.gameOfLifeConfig.resolutionX;
        float resY = Configs.gameOfLifeConfig.resolutionY;

        float scaleY = 800f;
        float scaleX = scaleY * (resX / resY);
        quadObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        return quadObj;
    }

    public void OnGridShow(bool showGrid)
    {
        gridMaterial.SetFloat("_ShowGrid", showGrid ? 1.0f : 0.0f);
    }

    private void PaintCell(Vector2 uv, float value, float brushSize)
    {
        Debug.Log($"Attempting to paint: Pos={uv}, ColorValue={value}");

        int resolutionX = (int)Configs.gameOfLifeConfig.resolutionX;
        int resolutionY = (int)Configs.gameOfLifeConfig.resolutionY;

        paintMaterial.SetVector("_MousePos", uv);
        paintMaterial.SetFloat("_PaintColor", value);
        paintMaterial.SetFloat("_BrushSize", brushSize);
        paintMaterial.SetFloat("_ResolutionX", resolutionX);
        paintMaterial.SetFloat("_ResolutionY", resolutionY);

        RenderTexture temp = RenderTexture.GetTemporary(resolutionX, resolutionY, 0);
        Graphics.Blit(gameData.lifeData.currentTex, temp, paintMaterial);
        Graphics.Blit(temp, gameData.lifeData.currentTex);
        RenderTexture.ReleaseTemporary(temp);
    }

    private void PaintLine(Vector2 start, Vector2 end, float value, float brushSize)
    {
        float dist = Vector2.Distance(start, end);

        float stepSize = Mathf.Max(brushSize * 0.5f, 0.001f);
        int steps = Mathf.CeilToInt(dist / stepSize);

        // 循环绘制所有插值点
        for (int i = 1; i <= steps; ++i)
        {
            float t = (float)i / steps;
            Vector2 lerpPos = Vector2.Lerp(start, end, t);
            PaintCell(lerpPos, value, brushSize);
        }
    }

    private void Paint()
    {
        Vector2 currentUV = gameLogic.playerController.cellUV;
        float value = gameLogic.playerController.cellValue;
        float brushSize = gameLogic.playerController.brushSize;

        bool isInputting = !float.IsNegativeInfinity(currentUV.x);

        if (isInputting)
        {
            // 没有上一帧数据或是距离很近
            if (float.IsNegativeInfinity(_lastFrameUV.x) || Vector2.Distance(_lastFrameUV, currentUV) < 0.001f)
            {
                // 只画一个点
                PaintCell(currentUV, value, brushSize);
            }
            else
            {
                // 计算两帧数之间的插值，绘制不间断线段
                PaintLine(_lastFrameUV, currentUV, value, brushSize);
            }

            _lastFrameUV = currentUV;
        }
        else
        {
            _lastFrameUV = Vector2.negativeInfinity;
        }
    }
}
