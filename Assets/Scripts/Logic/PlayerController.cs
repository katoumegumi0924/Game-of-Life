using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlayerController
{
    private GameData gameData;

    private RawImage displayImage;

    public Vector2 cellUV;
    public float cellValue;
    public float brushSize;

    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();

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
        bool leftClick = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButton(1);

        if (!leftClick && !rightClick)
        {
            // 没有点击鼠标时 cellUv 设为无效值
            cellUV = Vector2.negativeInfinity;
            return;
        }

        if (IsPointerOverBlockingUI())
        {
            return;
        }

        cellUV = GetUVFromMouse();

        if (cellUV != Vector2.negativeInfinity)
        {
            cellValue = leftClick ? 0.0f : 1.0f;
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

    // 射线检测 判断鼠标当前是否位于真正的UI上
    // 由于演化的RenderTexture也是在UI上，所以无法使用EventSystem.current.IsPointerOverGameObject()
    private bool IsPointerOverBlockingUI()
    {
        if (_pointerEventData == null)
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
        }

        _pointerEventData.position = Input.mousePosition;
        _raycastResults.Clear();

        // 发出射线 获取所有被点中的UI元素
        EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

        for (int i = 0; i < _raycastResults.Count; ++i)
        {
            // 如果射线检测结果中存在演化UI之外的UI，说明鼠标被其他UI遮挡了
            if (_raycastResults[i].gameObject != displayImage.gameObject)
            {
                return true;
            }
        }

        return false;
    }
}
