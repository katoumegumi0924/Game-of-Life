using System;
using UnityEngine;
using UnityEngine.UI;

public class UISaveEntry : ManualBehavior
{
    public Text saveNameText;
    public RawImage previewImage;
    public Button LoadButton;
    public Button deleteButton;

    private string path;
    public Action<string> onLoadAction;
    public Action<string> onDeleteAction;
    private SaveEntryInfo currentEntryInfo;

    protected override bool _OnInit()
    {
        currentEntryInfo = data as SaveEntryInfo;

        if (currentEntryInfo.path == null || currentEntryInfo.saveName == null)
            return false;

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

        return true;
    }

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
        onLoadAction?.Invoke(path);
    }

    private void OnDeleteClick()
    {
        onDeleteAction?.Invoke(path);
    }

    // 用于为已经初始化的对象更新显示数据
    public void SetDisplayInfo(SaveEntryInfo info)
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