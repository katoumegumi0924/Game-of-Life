using UnityEngine;

public class LifeLogic
{
    public GameData gameData;
    public ComputeShader lifeShader;
    public ComputeBuffer inputBuffer;
    public ComputeBuffer outputBuffer;
    public LifeRuleConfig lifeRule;

    private int updateKernel;
    private int resX;
    private int resY;

    private int accumulator = 0;

    public void Init(GameData data)
    {
        gameData = data;
        lifeShader = data.lifeData.lifeShader;
        updateKernel = lifeShader.FindKernel("CSUpdate");

        resX = gameData.lifeData.resX;
        resY = gameData.lifeData.resY;

        inputBuffer = new ComputeBuffer(gameData.lifeData.texA.Length, sizeof(int));
        outputBuffer = new ComputeBuffer(gameData.lifeData.texB.Length, sizeof(int));
    }

    public void Free()
    {
        gameData = null;
        lifeShader = null;
        updateKernel = 0;
        resX = 0;
        resY = 0;

        if (inputBuffer != null)
        {
            inputBuffer.Release();
            inputBuffer = null;
        }

        if (outputBuffer != null)
        {
            outputBuffer.Release();
            outputBuffer = null;
        }
    }

    public void SetNew()
    {
        // 获取默认迭代规则
        lifeRule = Configs.lifeRuleSet.GetLifeRule(0);
    }

    public void AfterImport()
    {
        lifeRule = Configs.lifeRuleSet.GetLifeRule(gameData.lifeData.currentRuleIndex);
    }

    public void OnUpdate()
    {
        accumulator += gameData.lifeTimeData.tickDelta;
        while (accumulator >= Configs.gameOfLifeConfig.singleStepTick)
        {
            LifeUpdate();
            accumulator -= Configs.gameOfLifeConfig.singleStepTick;
        }
    }

    public void LifeUpdate()
    {
        var input = gameData.lifeData.useTexA ? gameData.lifeData.texA : gameData.lifeData.texB;
        var output = gameData.lifeData.useTexA ? gameData.lifeData.texB : gameData.lifeData.texA;

        inputBuffer.SetData(input);
        outputBuffer.SetData(output);

        lifeShader.SetBuffer(updateKernel, "Input", inputBuffer);
        lifeShader.SetBuffer(updateKernel, "Output", outputBuffer);
        lifeShader.SetInt("birthMask", lifeRule.birthMask);
        lifeShader.SetInt("survivalMask", lifeRule.survivalMask);
        lifeShader.SetInts("resolution", resX, resY);

        int groupsX = Mathf.CeilToInt(resX / 8.0f);
        int groupsY = Mathf.CeilToInt(resY / 8.0f);
        lifeShader.Dispatch(updateKernel, groupsX, groupsY, 1);

        gameData.lifeData.Swap();
    }

    public void OnClearLife()
    {
        var output = gameData.lifeData.useTexA ? gameData.lifeData.texB : gameData.lifeData.texA;
        outputBuffer.SetData(output);

        int clearKernel = lifeShader.FindKernel("CSClear");
        lifeShader.SetBuffer(clearKernel, "Output", outputBuffer);

        int groupX = Mathf.CeilToInt(resX / 8.0f);
        int groupY = Mathf.CeilToInt(resY / 8.0f);

        lifeShader.Dispatch(clearKernel, groupX, groupY, 1);
    }

    public RenderTexture ArrayToRenderTexture(int[] data, int resX, int resY)
    {
        RenderTexture resultTex = new RenderTexture(resX, resY, 0);
        Color[] pixels = new Color[resX * resY];

        for (int y = 0; y < resY; ++y)
        {
            for (int x = 0; x < resX; ++x)
            {
                int saveIndex = y * resX + x;

                if (data[saveIndex] == 0)
                {
                    pixels[saveIndex] = Color.white;
                }
                else
                {
                    pixels[saveIndex] = Color.black;
                }
            }
        }

        Texture2D tex = new Texture2D(resX, resY, TextureFormat.RGB24, false);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixels);
        tex.Apply();
        Graphics.Blit(tex, resultTex);
        Texture2D.Destroy(tex);

        return resultTex;
    }
}
