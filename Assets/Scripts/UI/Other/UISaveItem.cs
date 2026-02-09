using System;
using UnityEngine;
using UnityEngine.UI;

public class UISaveItem : ManualBehavior
{
    public Text saveNameText;
    public RawImage previewImage;
    public Button LoadButton;
    public Button deleteButton;

    private string path;
    public Action<string> onLoadAction;
    public Action<string> onDeleteAction;
    private SaveItemInfo itemInfo;

    protected override bool _OnInit()
    {
        itemInfo = data as SaveItemInfo;

        if (itemInfo.path == null || itemInfo.saveName == null)
            return false;

        path = itemInfo.path;

        if (saveNameText != null)
        {
            saveNameText.text = itemInfo.saveName;
        }

        if (previewImage != null && itemInfo.previewTex != null)
        {
            previewImage.texture = itemInfo.previewTex;
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

public class SaveItemInfo
{
    public string path;
    public string saveName;
    public Texture2D previewTex;

    public SaveItemInfo(string _path, string _saveName, Texture2D _previewTex)
    {
        path = _path;
        saveName = _saveName;
        previewTex = _previewTex;
    }
}