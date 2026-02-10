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
    private SaveEntryInfo entryInfo;

    protected override bool _OnInit()
    {
        entryInfo = data as SaveEntryInfo;

        if (entryInfo.path == null || entryInfo.saveName == null)
            return false;

        path = entryInfo.path;

        if (saveNameText != null)
        {
            saveNameText.text = entryInfo.saveName;
        }

        if (previewImage != null && entryInfo.previewTex != null)
        {
            previewImage.texture = entryInfo.previewTex;
        }
        else if (previewImage != null)
        {
            previewImage.texture = null;
            previewImage.color = Color.black;
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