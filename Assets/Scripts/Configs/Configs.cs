using UnityEngine;

//public class Configs : MonoBehaviour
//{
//    #region MONO
//    static Configs instance;

//    private void Awake()
//    {
//        instance = this;
//    }

//    private void OnDestroy()
//    {
//        instance = null;
//    }
//    #endregion

//    public GameResourcesConfig m_resourcesConfig;
//    public static GameResourcesConfig gameResourcesConfig { get { return instance == null ? null : instance.m_resourcesConfig; } }

//    public GameOfLifeConfig m_gameOfLifeConfig;
//    public static GameOfLifeConfig gameOfLifeConfig { get { return instance == null ? null : instance.m_gameOfLifeConfig; } }

//    public GPUConfig m_gpuConfig;
//    public static GPUConfig gpuConfig { get { return instance == null ? null : instance.m_gpuConfig; } }

//    public LifeRuleSet m_lifeRuleSet;
//    public static LifeRuleSet lifeRuleSet { get { return instance == null ? null : instance.m_lifeRuleSet; } }
//}

public static class Configs
{
    private static GameOfLifeConfig _gameOfLifeConfig;
    public static GameOfLifeConfig gameOfLifeConfig
    {
        get
        {
            if (_gameOfLifeConfig == null)
            {
                _gameOfLifeConfig = Resources.Load<GameOfLifeConfig>("Configs/GameOfLifeConfig");

                if (_gameOfLifeConfig == null)
                {
                    Debug.LogError("错误：在 Resources 文件夹下找不到名为 'GameOfLifeConfig' 的配置文件！");
                }
            }
            return _gameOfLifeConfig;
        }
    }

    private static GameResourcesConfig _gameResourcesConfig;
    public static GameResourcesConfig gameResourcesConfig
    {
        get
        {
            if (_gameResourcesConfig == null)
            {
                _gameResourcesConfig = Resources.Load<GameResourcesConfig>("Configs/GameResourcesConfig");

                if (_gameResourcesConfig == null)
                {
                    Debug.LogError("错误：在 Resources 文件夹下找不到名为 'GameResourcesConfig' 的配置文件！");
                }
            }
            return _gameResourcesConfig;
        }
    }

    private static GPUConfig _gpuConfig;
    public static GPUConfig gpuConfig
    {
        get
        {
            if (_gpuConfig == null)
            {
                _gpuConfig = Resources.Load<GPUConfig>("Configs/GPUConfig");

                if (_gpuConfig == null)
                {
                    Debug.LogError("错误：在 Resources 文件夹下找不到名为 'GPUConfig' 的配置文件！");
                }
            }
            return _gpuConfig;
        }
    }

    private static LifeRuleSetConfig _lifeRuleSet;
    public static LifeRuleSetConfig lifeRuleSet
    {
        get
        {
            if (_lifeRuleSet == null)
            {
                _lifeRuleSet = Resources.Load<LifeRuleSetConfig>("Configs/LifeRuleSetConfig");

                if (_lifeRuleSet == null)
                {
                    Debug.LogError("错误：在 Resources 文件夹下找不到名为 'LifeRuleSetConfig' 的配置文件！");
                }
            }
            return _lifeRuleSet;
        }
    }
}