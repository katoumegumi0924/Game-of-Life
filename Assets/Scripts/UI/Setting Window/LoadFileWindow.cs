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

    private List<UISaveEntry> saveEntriesPool = new List<UISaveEntry>();

    public static bool isHoveringScroll = false;

    protected override void _OnOpen()
    {
        RefreshList();
    }

    // 刷新存档列表 非常需要优化 应该可以使用DataPool
    // 而且存档功能应该要重写，现在完全没有Import和Export
    public void RefreshList()
    {
        FileInfo[] files = GetAllSaveFiles();

        // 回收uiEntry
        for (int i = 0; i < saveEntriesPool.Count; ++i)
        {
            UISaveEntry unusedEntry = saveEntriesPool[i];

            // 回收时解绑事件
            unusedEntry.onLoadAction -= OnClickFileLoad;
            unusedEntry.onDeleteAction -= OnClickFileDelete;

            unusedEntry._Free();
        }

        // 遍历存档文件
        for (int i = 0; i < files.Length; i++)
        {
            UISaveEntry uiEntry;
            FileInfo file = files[i];

            if (i < saveEntriesPool.Count)
            {
                // 复用对象池中的对象
                uiEntry = saveEntriesPool[i];
            }
            else
            {
                // 池子中数量不够，创建新的对象
                GameObject fileItemObj = GameObject.Instantiate(fileItemPrefabs, content, false);
                uiEntry = fileItemObj.GetComponent<UISaveEntry>();
                uiEntry._Create();
                saveEntriesPool.Add(uiEntry);
            }

            // 填充数据
            Texture2D preImage = GameSave.LoadPreviewImage(file.FullName);
            uiEntry._Init(new SaveEntryInfo(file.FullName, file.Name, preImage));
            uiEntry._Open();

            uiEntry.onLoadAction += OnClickFileLoad;
            uiEntry.onDeleteAction += OnClickFileDelete;
        }
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
