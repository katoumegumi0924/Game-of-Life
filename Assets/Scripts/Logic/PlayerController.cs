using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController
{
    private GameData gameData;

    private RawImage displayImage;

    public Vector2 cellUV;
    public float cellValue;
    public float brushSize;

    public void Init(GameData _gameData)
    {
        gameData = _gameData;

        displayImage = gameData.lifeData.displayImage;
        
    }

    public void Free()
    {
        gameData = null;

        displayImage = null; 
    }

    public void SetNew()
    {
        cellUV = Vector2.zero;
        cellValue = 0f;
        SetBrushSize(1);
    }

    public void OnUpdate()
    {
        HandlePauseInput();
        HandlePainting();
    }

    // 空格键暂停/恢复
    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameData.lifeTimeData.TogglePause();
        }
    }

    // 鼠标编辑细胞状态
    private void HandlePainting()
    {
        if (LoadFileWindow.isHoveringScroll || SettingWindow.isHoveringSettingWindow || IterationRulesWindow.isHoveringIterationWindow || SaveInputWindow.isSaveInput)
        {
            return;
        }

        bool leftClick = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButton(1);
        if (leftClick || rightClick)
        {
            cellUV = GetUVFromMouse();

            if (cellUV != Vector2.negativeInfinity)
            {
                cellValue = leftClick ? 0.0f : 1.0f;
            }
        }
        else
        {
            // 没有点击鼠标时 cellUv 设为无效值
            cellUV = Vector2.negativeInfinity;
        }
    }

    // 坐标转换
    private Vector2 GetUVFromMouse()
    {
        Vector2 localPoint;
        RectTransform rectTransform = displayImage.rectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out localPoint))
        {
            Vector2 normalizedUV = Rect.PointToNormalized(rectTransform.rect, localPoint);
            Rect uvRect = displayImage.uvRect;
            normalizedUV.x = normalizedUV.x * uvRect.width + uvRect.x;
            normalizedUV.y = normalizedUV.y * uvRect.height + uvRect.y;

            // 边界检查
            if (normalizedUV.x >= 0 && normalizedUV.x <= 1 && normalizedUV.y >= 0 && normalizedUV.y <= 1)
            {
                return normalizedUV;
            }
        }
        return Vector2.negativeInfinity;
    }

    // 设置笔刷大小
    private void SetBrushSize(int size)
    {
        float heightResolution = Configs.gameOfLifeConfig.resolutionY;
        float singlePixelUVSize = 1.0f / heightResolution;

        brushSize = singlePixelUVSize * 0.5f * size;
    }
}
