using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class UILoadFile : ManualBehavior, IPointerEnterHandler, IPointerExitHandler
{
    public Transform content;
    public UISaveEntry uiSaveEntryPrefab;

    private List<UISaveEntry> saveEntries;

    public bool isHoveringLoadWindow = false;

    protected override bool _OnInit()
    {
        saveEntries = new List<UISaveEntry>();

        return true;
    }

    protected override void _OnFree()
    {
        if (saveEntries != null)
        {
            for (int i = 0; i < saveEntries.Count; ++i)
            {
                saveEntries[i]._Free();
            }

            saveEntries.Clear();
            saveEntries = null;
        }
        
    }

    protected override void _OnOpen()
    {
        RefreshList();
    }

    protected override void _OnClose()
    {
        
    }

    public void RefreshList()
    {
        FileInfo[] files = GetAllSaveFiles();

        for (int i = 0; i < files.Length; ++i)
        {
            if (i >= saveEntries.Count)
            {
                UISaveEntry newFileEntry = UISaveEntry.Instantiate(uiSaveEntryPrefab, content, false);
                newFileEntry._Create();
                newFileEntry._Init(null);
                saveEntries.Add(newFileEntry);
            }

            var entry = saveEntries[i];
            Texture2D preImage = LifeData.LoadPreviewImage(files[i].FullName);
            var saveInfo = new SaveEntryInfo(files[i].FullName, files[i].Name, preImage);
            entry.SetData(saveInfo);
            if (this.active)
                entry._Open();
        }

        for (int i = files.Length; i < saveEntries.Count; ++i)
        {
            saveEntries[i]._Close();
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

    public void OnPointerExit(PointerEventData eventData)
    {
        isHoveringLoadWindow = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHoveringLoadWindow = true;
    }
}
