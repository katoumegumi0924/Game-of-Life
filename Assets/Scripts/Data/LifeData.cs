using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LifeData
{
    // 双缓存纹理 交替读写
    public RenderTexture texA;
    public RenderTexture texB;
    public bool useTexA = true;
    public RenderTexture currentTex { get { return useTexA ? texA : texB; } }
    public ComputeShader lifeShader;
    public uint resolutionY = Configs.gameOfLifeConfig.resolutionY;
    public uint resolutionX = Configs.gameOfLifeConfig.resolutionX;

    private GameObject displayImageObj;
    public RawImage displayImage;

    // 初始状态，初始化与加载存档后更新
    public RenderTexture initTex;

    public void Init()
    {
        lifeShader = Configs.gpuConfig.lifeShader;

        texA = CreateRenderTexture();
        texB = CreateRenderTexture();

        displayImageObj = CreateDisplayObj();
        displayImage = displayImageObj.GetComponent<RawImage>();
    }

    public void SetNew()
    {
        SeedTexture(texA);
        SetInitTexture(texA);
    }

    public void Free()
    {
        if (texA != null)
        {
            texA.Release();
            texA = null;
        }

        if (texB != null)
        {
            texB.Release();
            texB = null;
        }

        if (displayImageObj != null)
        {
            displayImage = null;
            GameObject.Destroy(displayImageObj);
        }
    }

    public void Swap()
    {
        useTexA = !useTexA;
    }

    private RenderTexture CreateRenderTexture()
    {
        RenderTexture rt = new RenderTexture((int)resolutionX, (int)resolutionY, 0);
        rt.enableRandomWrite = true;
        rt.filterMode = FilterMode.Point;
        rt.wrapMode = TextureWrapMode.Repeat;
        rt.Create();
        return rt;
    }

    private GameObject CreateDisplayObj()
    {
        GameObject prefab = Configs.gameResourcesConfig.displayImagePrefab;
        Canvas canvas = UIRoot.instance.worldCanvas;
        return GameObject.Instantiate(prefab, canvas.transform, false);   
    }

    // 调用 ComputeShader 进行随机填充初始化
    private void SeedTexture(RenderTexture target)
    {
        if (lifeShader == null)
            return;

        int kernel = lifeShader.FindKernel("CSInitRandom");

        lifeShader.SetTexture(kernel, "OutputTex", target);
        lifeShader.SetVector("resolution", new Vector2(resolutionX, resolutionY));
        lifeShader.SetFloat("seed", Random.Range(0f, 10f));

        int groupsX = Mathf.CeilToInt(resolutionX / 8.0f);
        int groupsY = Mathf.CeilToInt(resolutionY / 8.0f);
        lifeShader.Dispatch(kernel, groupsX, groupsY, 1);
    }

    // 设置初始状态
    public void SetInitTexture(RenderTexture source)
    {
        if (source == null)
            return;

        if (initTex == null)
        {
            initTex = new RenderTexture(source.descriptor);
            initTex.enableRandomWrite = true;
            initTex.Create();
        }

        Graphics.Blit(source, initTex);
    }

    // 重置当前状态为初始状态
    public void ResetTexture()
    {
        useTexA = true;
        Graphics.Blit(initTex, texA);
    }
}
