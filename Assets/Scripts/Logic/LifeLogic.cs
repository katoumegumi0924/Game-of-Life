using UnityEngine;

public class LifeLogic
{
    public GameData gameData;
    public ComputeShader lifeShader;
    public LifeRuleConfig lifeRule;

    private int updateKernel;
    private uint resolutionX;
    private uint resolutionY;

    private int accumulator = 0;

    public void Init(GameData data)
    {
        gameData = data;
        lifeShader = data.lifeData.lifeShader;
        updateKernel = lifeShader.FindKernel("CSUpdate");

        resolutionX = gameData.gameDesc.resolutionX;
        resolutionY = gameData.gameDesc.resolutionY;
    }

    public void Free()
    {
        gameData = null;
        lifeShader = null;
        updateKernel = 0;
    }

    public void SetNew()
    {
        // 获取默认迭代规则
        lifeRule = Configs.lifeRuleSet.GetLifeRule(0);
    }

    public void AfterImport()
    {
        lifeRule = Configs.lifeRuleSet.GetLifeRule(gameData.lifeData.currentModeIndex);
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

        lifeShader.SetTexture(updateKernel, "InputTex", input);
        lifeShader.SetTexture(updateKernel, "OutputTex", output);
        lifeShader.SetInt("birthMask", lifeRule.birthMask);
        lifeShader.SetInt("survivalMask", lifeRule.survivalMask);
        lifeShader.SetVector("resolution", new Vector2(resolutionX, resolutionY));

        int groupsX = Mathf.CeilToInt(resolutionX / 8.0f);
        int groupsY = Mathf.CeilToInt(resolutionY / 8.0f);
        lifeShader.Dispatch(updateKernel, groupsX, groupsY, 1);

        gameData.lifeData.Swap();
    }

    public void OnClearLife()
    {
        int clearKernel = lifeShader.FindKernel("CSClear");
        lifeShader.SetTexture(clearKernel, "OutputTex", gameData.lifeData.currentTex);

        int groupX = Mathf.CeilToInt(resolutionX / 8.0f);
        int groupY = Mathf.CeilToInt(resolutionY / 8.0f);

        lifeShader.Dispatch(clearKernel, groupX, groupY, 1);
    }
}
