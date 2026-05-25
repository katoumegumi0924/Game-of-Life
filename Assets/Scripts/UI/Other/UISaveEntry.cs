using System;
using UnityEngine;
using UnityEngine.UI;

public class UISaveEntry : ManualBehavior
{
    public Text saveNameText;
    public RawImage previewImage;
    public Button LoadButton;
    public Button deleteButton;

    public string path;
    public Action<string> onLoadAction;
    public Action<string> onDeleteAction;
    public SaveEntryInfo currentEntryInfo;

    //protected override bool _OnInit()
    //{
    //    gameMain = data as GameMain;
    //    if (gameMain == null)
    //        return false;

    //    return true;
    //}

    //protected override void _OnFree()
    //{
    //    gameMain = null;
    //}

    protected override void _OnRegEvent()
    {
        LoadButton.onClick.AddListener(OnLoadClick);
        deleteButton.onClick.AddListener(OnDeleteClick);
    }

    protected override void _OnUnregEvent()
    {
        LoadButton.onClick.RemoveListener(OnLoadClick);
        deleteButton.onClick.RemoveListener(OnDeleteClick);
    }

    private void OnLoadClick()
    {
        UIRoot.instance.uiMainMenu._Close();
        UIRoot.instance.uiGame._Open();

        // GameMain.instance.gameData.lifeTimeData.Pause();
        // GameSave.LoadGame(path, GameMain.instance.gameData);

        // 加载存档时更新lifeData的initTex
        // GameMain.instance.gameData.lifeData.SetInitTexture(GameMain.instance.gameData.lifeData.currentTex);
        Program.loadFile = currentEntryInfo.path;
        Program.instance.gameMain.gameObject.SetActive(true);
    }

    private void OnDeleteClick()
    {
        if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            return;

        GameSave.DeleteSave(path);
        // UIRoot.instance.uiGame.uiSettingMenu.loadFileWindow.RefreshList();
        UIRoot.instance.uiMainMenu.uiLoadFile.RefreshList();
    }

    // 用于为已经初始化的对象更新显示数据
    public void SetData(SaveEntryInfo info)
    {
        // 清理旧数据
        if (currentEntryInfo != null && currentEntryInfo.previewTex != null)
        {
            if (currentEntryInfo.previewTex != info.previewTex)
            {
                Texture2D.Destroy(currentEntryInfo.previewTex);
            }
        }

        currentEntryInfo = info;
        path = currentEntryInfo.path;

        if (saveNameText != null)
        {
            saveNameText.text = currentEntryInfo.saveName;
        }

        if (previewImage != null && currentEntryInfo.previewTex != null)
        {
            previewImage.texture = currentEntryInfo.previewTex;
        }
        else if (previewImage != null)
        {
            previewImage.texture = null;
            previewImage.color = Color.white;
        }
    }
}

public class SaveEntryInfo
{
    public string path;
    public string saveName;
    public Texture2D previewTex;

    public SaveEntryInfo(string _path, string _saveName, Texture2D _previewTex)
    {
        path = _path;
        saveName = _saveName;
        previewTex = _previewTex;
    }
}