using UnityEngine;
using UnityEngine.UI;

public class LifeRenderer
{
    public GameData gameData;
    public GameLogic gameLogic;
    public GameObject displayObj;
    public MeshRenderer displayRenderer;

    private Material sceneMaterial;
    private Material paintMaterial;
    private Vector2 _lastFrameUV;

    public void Init(GameData _gameData, GameLogic _gameLogic)
    {
        gameData = _gameData;
        gameLogic = _gameLogic;

        displayObj = CreateDisplayObj();
        displayRenderer = displayObj.GetComponent<MeshRenderer>();

        sceneMaterial = displayRenderer.material;

        float resX = gameData.lifeData.resX;
        float resY = gameData.lifeData.resY;
        sceneMaterial.SetVector("_Resolution", new Vector2(resX, resY));

        // 默认关闭网格
        // gridMaterial.SetFloat("_ShowGrid", 0.0f);
        paintMaterial = Material.Instantiate(Configs.gameResourcesConfig.paint);
    }

    public void Free()
    {
        if (sceneMaterial != null)
        {
            Material.Destroy(sceneMaterial);
            sceneMaterial = null;
        }

        if (paintMaterial != null)
        {
            Material.Destroy(paintMaterial);
            paintMaterial = null;
        }

        if (displayObj != null)
        {
            displayRenderer = null;
            GameObject.Destroy(displayObj);
        }
    }

    public void OnUpdate()
    {
        if (sceneMaterial != null)
        {
            sceneMaterial.SetBuffer("_CellBuffer", gameLogic.lifeLogic.outputBuffer);
        }

        // Paint();
    }

    private GameObject CreateDisplayObj()
    {
        GameObject prefab = Configs.gameResourcesConfig.displayImagePrefab;

        GameObject quadObj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);

        float resX = gameData.gameDesc.resolutionX;
        float resY = gameData.gameDesc.resolutionY;

        float scaleY = 10f;
        float scaleX = scaleY * (resX / resY);
        quadObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        return quadObj;
    }

    public void OnGridShow(bool showGrid)
    {
        sceneMaterial.SetFloat("_ShowGrid", showGrid ? 1.0f : 0.0f);
    }

    private void PaintCell(Vector2 uv, float value)
    {
        int resX = (int)gameData.gameDesc.resolutionX;
        int resY = (int)gameData.gameDesc.resolutionY;
        var lifeShader = gameLogic.lifeLogic.lifeShader;

        int kernel = lifeShader.FindKernel("CSPaint");
        lifeShader.SetVector("mouseUV", uv);
        lifeShader.SetFloat("paintValue", value);
        lifeShader.SetVector("resolution", new Vector2(resX, resY));
        lifeShader.SetBuffer(kernel, "OutPut", gameLogic.lifeLogic.outputBuffer);
        lifeShader.Dispatch(kernel, Mathf.CeilToInt(resX / 8f), Mathf.CeilToInt(resY / 8f), 1);

        //paintMaterial.SetVector("_MousePos", uv);
        //paintMaterial.SetFloat("_PaintColor", value);
        //paintMaterial.SetFloat("_ResolutionX", resolutionX);
        //paintMaterial.SetFloat("_ResolutionY", resolutionY);

        //RenderTexture temp = RenderTexture.GetTemporary(resolutionX, resolutionY, 0);
        //Graphics.Blit(gameData.lifeData.currentTex, temp, paintMaterial);
        //Graphics.Blit(temp, gameData.lifeData.currentTex);
        //RenderTexture.ReleaseTemporary(temp);
    }

    private void PaintLine(Vector2 start, Vector2 end, float value)
    {
        float dist = Vector2.Distance(start, end);

        // 采样次数与距离之间的倍率，避免画线时出现间断点，也许需要根据分辨率来设置
        float stepSize = 1000f;
        int steps = Mathf.CeilToInt(dist * stepSize);

        // 循环绘制所有插值点
        for (int i = 1; i <= steps; ++i)
        {
            float t = (float)i / steps;
            Vector2 lerpPos = Vector2.Lerp(start, end, t);
            PaintCell(lerpPos, value);
        }
    }

    private void Paint()
    {
        Vector2 currentUV = gameLogic.playerController.cellUV;
        float value = gameLogic.playerController.cellValue;

        bool isInputting = !float.IsNegativeInfinity(currentUV.x);

        if (isInputting)
        {
            // 没有上一帧数据或是距离很近
            if (float.IsNegativeInfinity(_lastFrameUV.x) || Vector2.Distance(_lastFrameUV, currentUV) < 0.001f)
            {
                // 只画一个点
                PaintCell(currentUV, value);
            }
            else
            {
                // 计算两帧数之间的插值，绘制不间断线段
                PaintLine(_lastFrameUV, currentUV, value);
            }

            _lastFrameUV = currentUV;
        }
        else
        {
            _lastFrameUV = Vector2.negativeInfinity;
        }
    }
}
