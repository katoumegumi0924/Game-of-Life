using UnityEngine;

public class LifeLogic
{
    private GameData gameData;
    private ComputeShader lifeShader;
    private int updateKernel;
    private uint resolutionX = Configs.gameOfLifeConfig.resolutionX;
    private uint resolutionY = Configs.gameOfLifeConfig.resolutionY;

    private int accumulator = 0;

    private LifeRuleConfig lifeRule;

    public void Init(GameData data)
    {
        gameData = data;
        lifeShader = data.lifeData.lifeShader;
        updateKernel = lifeShader.FindKernel("CSUpdate");

        UIRoot.instance.iterationRulesPanel.OnRuleSelected += OnSelectLifeRule;

        UIRoot.instance.settingPanel.OnPlayButtonClick += TogglePause;
        UIRoot.instance.settingPanel.OnStepForwardClick += OnStepForward;
        UIRoot.instance.settingPanel.OnSpeedUpButtonClick += OnSpeedUp;
        UIRoot.instance.settingPanel.OnSlowDownButtonClick += OnSlowDown;
        UIRoot.instance.settingPanel.OnClearButtonClick += OnClearLife;
        UIRoot.instance.settingPanel.OnSaveButtonClick += OnOpenSaveInput;
        UIRoot.instance.settingPanel.OnLoadButtonClick += OnOpenLoad;
        UIRoot.instance.settingPanel.OnRestButtonClick += OnReset;

        UIRoot.instance.settingPanel.loadFileWindow.OnFileSelected += OnFileLoad;
        UIRoot.instance.settingPanel.saveinputWindow.OnConfirm += OnFileSave;
    }

    public void Free()
    {
        gameData = null;
        lifeShader = null;
        updateKernel = 0;

        UIRoot.instance.iterationRulesPanel.OnRuleSelected -= OnSelectLifeRule;

        UIRoot.instance.settingPanel.OnPlayButtonClick -= TogglePause;
        UIRoot.instance.settingPanel.OnStepForwardClick -= OnStepForward;
        UIRoot.instance.settingPanel.OnSpeedUpButtonClick -= OnSpeedUp;
        UIRoot.instance.settingPanel.OnSlowDownButtonClick -= OnSlowDown;
        UIRoot.instance.settingPanel.OnClearButtonClick -= OnClearLife;
        UIRoot.instance.settingPanel.OnSaveButtonClick -= OnOpenSaveInput;
        UIRoot.instance.settingPanel.OnLoadButtonClick -= OnOpenLoad;
        UIRoot.instance.settingPanel.OnRestButtonClick -= OnReset;

        UIRoot.instance.settingPanel.loadFileWindow.OnFileSelected -= OnFileLoad;
        UIRoot.instance.settingPanel.saveinputWindow.OnConfirm -= OnFileSave;
    }

    public void SetNew()
    {
        // 获取默认迭代规则
        lifeRule = Configs.lifeRuleSet.GetLifeRule(0);
    }

    public void OnUpdate()
    {
        accumulator += gameData.lifeTimeData.tickDelta;
        while (accumulator > Configs.gameOfLifeConfig.singleStepTick)
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

    private void OnSelectLifeRule(LifeRuleConfig _lifeRule)
    {
        lifeRule = _lifeRule;
    }

    private void OnStepForward()
    {
        gameData.lifeTimeData.Pause();
        LifeUpdate();
    }

    private void TogglePause()
    {
        gameData.lifeTimeData.TogglePause();
    }

    private void OnSpeedUp()
    {
        gameData.lifeTimeData.UnPause();
        gameData.lifeTimeData.SpeedUp();
    }

    private void OnSlowDown()
    {
        gameData.lifeTimeData.UnPause();
        gameData.lifeTimeData.SlowDown();
    }

    private void OnReset()
    {
        gameData.lifeData.ResetTexture();
    }

    private void OnOpenSaveInput()
    {
        string fileName = $"save_{System.DateTime.Now:yyyyMMdd_HHmmss}";
        UIRoot.instance.settingPanel.saveinputWindow.saveNameInputField.text = fileName;
        UIRoot.instance.settingPanel.saveinputWindow._Open();     
    }

    private void OnFileSave(string fileName)
    {
        RenderTexture texToSave = gameData.lifeData.currentTex;
        GameSave.SaveToBinary(texToSave, fileName);
        UIRoot.instance.settingPanel.loadFileWindow._Open();
        UIRoot.instance.settingPanel.loadFileWindow.RefreshList();
        UIRoot.instance.settingPanel.saveinputWindow._Close();
    }

    private void OnFileLoad(string path)
    {
        gameData.lifeTimeData.Pause();
        GameSave.LoadFromBinary(gameData.lifeData.currentTex, path);

        // 加载存档时更新lifeData的initTex
        gameData.lifeData.SetInitTexture(gameData.lifeData.currentTex);
    }

    private void OnOpenLoad()
    {
        // OnClearLife();
        // gameData.lifeTimeData.Pause();
        if (!UIRoot.instance.settingPanel.loadFileWindow.active)
        {
            UIRoot.instance.settingPanel.loadFileWindow._Open();
        }
        else
        {
            UIRoot.instance.settingPanel.loadFileWindow._Close();
        }
    }
}
