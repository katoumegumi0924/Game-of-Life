using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadFileWindow : ManualBehavior, IPointerEnterHandler, IPointerExitHandler
{
    public Transform content;
    public GameObject fileItemPrefabs;
    public event Action<string> OnFileSelected;

    private Dictionary<string, UISaveEntry> saveEntries = new Dictionary<string, UISaveEntry>();

    public static bool isHoveringScroll = false;

    protected override void _OnOpen()
    {
        RefreshList();
    }

    // 刷新存档列表
    public void RefreshList()
    {
        FileInfo[] files = GetAllSaveFiles();
        foreach (var item in saveEntries)
        {
            UISaveEntry uiItem = item.Value;
            if (uiItem != null)
            {
                uiItem.onLoadAction -= OnClickFileLoad;
                uiItem.onDeleteAction -= OnClickFileDelete;

                uiItem._Close();
                uiItem._Free();

                if (uiItem.gameObject != null)
                {
                    GameObject.Destroy(uiItem.gameObject);
                }
            }
        }

        saveEntries.Clear();

        foreach (var file in files)
        {
            CreateSaveEntry(file);
        }
    }

    // 创建存档Item
    private void CreateSaveEntry(FileInfo file)
    {
        if (fileItemPrefabs == null)
            return;

        GameObject fileItem = GameObject.Instantiate(fileItemPrefabs, content, false);
        UISaveEntry uiSaveItem = fileItem.GetComponent<UISaveEntry>();
        Texture2D preImage = GameSave.LoadPreviewImage(file.FullName);
        uiSaveItem._Create();
        uiSaveItem._Init(new SaveEntryInfo(file.FullName, file.Name, preImage));
        uiSaveItem._Open();

        saveEntries.Add(file.Name, uiSaveItem);

        // 绑定点击事件
        uiSaveItem.onLoadAction += OnClickFileLoad;
        uiSaveItem.onDeleteAction += OnClickFileDelete;
    }

    // 获取存档文件
    private FileInfo[] GetAllSaveFiles()
    {
        DirectoryInfo dir = new DirectoryInfo(Configs.gameOfLifeConfig.gameSaveFolder);
        if (!dir.Exists)
            dir.Create();

        FileInfo[] files = dir.GetFiles($"*{GameSave.saveExt}");
        return files;
    }

    // 删除存档文件
    private void DeleteSave(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        string imgPath = Path.ChangeExtension(path, ".PNG");
        if (File.Exists(imgPath))
        {
            File.Delete(imgPath);
        }

        RefreshList();
        Debug.Log($"已删除存档：{path}");
    }

    private void OnClickFileLoad(string path)
    {
        // 触发选择事件
        OnFileSelected?.Invoke(path);
    }

    private void OnClickFileDelete(string path)
    {
        if (path == null)
            return;

        string fileName = Path.GetFileName(path);
        if (saveEntries.ContainsKey(fileName))
        {
            UISaveEntry currentFileItem = saveEntries[fileName];
            currentFileItem.onLoadAction -= OnClickFileLoad;
            currentFileItem.onDeleteAction -= OnClickFileDelete;
            currentFileItem._Close();
            currentFileItem._Free();
        }

        DeleteSave(path);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHoveringScroll = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHoveringScroll = false;
    }
}
