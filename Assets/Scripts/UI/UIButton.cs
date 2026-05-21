using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Button button;
    public InputField input;
    public int data;
    public AudioSettings audios;
    public TipSettings tips;

    [Serializable]
    public struct AudioSettings
    {
        public string enterName;
        public string downName;
        public string upName;
    }

    public enum ItemTipType
    {
        None = -1,
        Item = 0,
        Recipe = 1,
        Other = 2,
        IgnoreIncPoint = 3
    }

    [Serializable]
    public struct TipSettings
    {
        public bool topLevel;
        public int level;   // 0: 必要提示  1: 重要非必要提示   2:不重要/新手提示
        public float delay;
        public int itemId;
        public int itemCount;
        public int itemInc;
        public Sprite tipSprite;
        public string tipTitle;
        public string tipText;
        public int corner;
        public Vector2 offset;
        public int width;
        public ItemTipType type;
    }

    [Serializable]
    public class Transition
    {
        public Graphic target;
        public float damp;

        public float mouseoverSize;
        public float pressedSize;
        public Color normalColor;
        public Color mouseoverColor;
        public Color pressedColor;
        public Color disabledColor;
        public bool alphaOnly;

        public float highlightSizeMultiplier;
        public float highlightColorMultiplier;
        public float highlightAlphaMultiplier;
        public Color highlightColorOverride;

        //public Sprite raycastAlphaMaskSprite;
    }

    public Transition[] transitions;
    public bool highlighted = false;

    public bool updating = false;

    public string tipTitleFormatString = "";
    public string tipTextFormatString = "";

    //public bool useRaycastAlphaMask = false;

    private bool isPointerDown = false;
    private bool isPointerEnter = false;

    private RectTransform rt;

    public event System.Action<int, PointerEventData.InputButton> onMouseDown;
    public event System.Action<int> onClick;
    public event System.Action<int> onDoubleClick;
    public event System.Action<int> onRightClick;
    public event System.Action<int, bool> onClickEnable;

    public void BindOnClickSafe(Action<int> func)
    {
        if (onClick == null)
            onClick += func;
    }

    MonoBehaviour tip;
    public bool tipShowing { get { return tip != null && tip.gameObject.activeSelf; } }

#if UNITY_EDITOR
    [ContextMenu("Init Transitions")]
    void InitTransitions()
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].damp == 0)
                transitions[i].damp = 0.3f;
            if (transitions[i].mouseoverSize == 0f)
                transitions[i].mouseoverSize = 1f;
            if (transitions[i].pressedSize == 0f)
                transitions[i].pressedSize = 1f;
            if (transitions[i].highlightSizeMultiplier == 0f)
                transitions[i].highlightSizeMultiplier = 1f;
            if (transitions[i].highlightColorMultiplier == 0f)
                transitions[i].highlightColorMultiplier = 1f;
            if (transitions[i].highlightAlphaMultiplier == 0f)
                transitions[i].highlightAlphaMultiplier = 1f;
            if (transitions[i].target != null &&
                transitions[i].normalColor.r <= 0f &&
                transitions[i].normalColor.g <= 0f &&
                transitions[i].normalColor.b <= 0f &&
                transitions[i].normalColor.a <= 0f)
            {
                transitions[i].normalColor = transitions[i].target.color;
            }
            if (transitions[i].target != null &&
                transitions[i].mouseoverColor.r <= 0f &&
                transitions[i].mouseoverColor.g <= 0f &&
                transitions[i].mouseoverColor.b <= 0f &&
                transitions[i].mouseoverColor.a <= 0f)
            {
                transitions[i].mouseoverColor = transitions[i].target.color;
            }
            if (transitions[i].target != null &&
                transitions[i].pressedColor.r <= 0f &&
                transitions[i].pressedColor.g <= 0f &&
                transitions[i].pressedColor.b <= 0f &&
                transitions[i].pressedColor.a <= 0f)
            {
                transitions[i].pressedColor = transitions[i].target.color;
            }
            if (transitions[i].target != null &&
                transitions[i].disabledColor.r <= 0f &&
                transitions[i].disabledColor.g <= 0f &&
                transitions[i].disabledColor.b <= 0f &&
                transitions[i].disabledColor.a <= 0f)
            {
                transitions[i].disabledColor = transitions[i].target.color;
                transitions[i].disabledColor.a *= 0.3f;
            }
        }
    }
    [ContextMenu("Init Reference")]
    void InitReference()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    [ContextMenu("预览 Normal")]
    void PreviewNormal()
    {
        if (transitions != null)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].target != null)
                {
                    transitions[i].target.transform.localScale = new Vector3(1f, 1f, 1f);
                    transitions[i].target.color = transitions[i].normalColor;
                }
            }
        }
    }

    [ContextMenu("预览 Mouseover")]
    void PreviewHover()
    {
        if (transitions != null)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].target != null)
                {
                    transitions[i].target.transform.localScale = new Vector3(1f, 1f, 1f) * transitions[i].mouseoverSize;
                    transitions[i].target.color = transitions[i].mouseoverColor;
                }
            }
        }
    }

    [ContextMenu("预览 Pressed")]
    void PreviewPressed()
    {
        if (transitions != null)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].target != null)
                {
                    transitions[i].target.transform.localScale = new Vector3(1f, 1f, 1f) * transitions[i].pressedSize;
                    transitions[i].target.color = transitions[i].pressedColor;
                }
            }
        }
    }

    [ContextMenu("预览 Disabled")]
    void PreviewDisabled()
    {
        if (transitions != null)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].target != null)
                {
                    transitions[i].target.transform.localScale = new Vector3(1f, 1f, 1f);
                    transitions[i].target.color = transitions[i].disabledColor;
                }
            }
        }
    }

    [ContextMenu("预览 Highlighted")]
    void PreviewHighlighted()
    {
        if (transitions != null)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].target != null)
                {
                    transitions[i].target.transform.localScale = new Vector3(1f, 1f, 1f) * transitions[i].highlightSizeMultiplier;
                    Color c = transitions[i].highlightColorOverride;
                    if (c.a > 0)
                        transitions[i].target.color = c;
                    else
                        transitions[i].target.color = new Color(
                        transitions[i].highlightColorMultiplier * transitions[i].normalColor.r, transitions[i].highlightColorMultiplier * transitions[i].normalColor.g,
                        transitions[i].highlightColorMultiplier * transitions[i].normalColor.b, transitions[i].highlightAlphaMultiplier * transitions[i].normalColor.a);
                }
            }
        }
    }

#endif
    public void RefreshTransitionsImmediately()
    {
        bool disabled = false;
        if (button != null)
            disabled = !button.enabled || !button.interactable;

        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].target != null)
            {
                Transition trs = transitions[i];
                Color wantColor = trs.normalColor;
                if (wantColor.r == 0 && wantColor.g == 0 && wantColor.b == 0 && wantColor.a == 0)
                    wantColor = trs.target.color;

                float wantSize = 1f;

                if (disabled)
                {
                    wantColor = trs.disabledColor;
                    transitions[i].target.color = wantColor;
                }
                else if (isPointerEnter && isPointerDown)
                {
                    wantSize = trs.pressedSize;
                    wantColor = trs.pressedColor;
                }
                else if (isPointerEnter && !isPointerDown)
                {
                    wantSize = trs.mouseoverSize;
                    wantColor = trs.mouseoverColor;
                }

                if (highlighted)
                {
                    wantSize *= trs.highlightSizeMultiplier;
                    if (trs.highlightColorOverride.a > 0)
                        wantColor = trs.highlightColorOverride;
                    else
                        wantColor *= new Color(
                        trs.highlightColorMultiplier, trs.highlightColorMultiplier,
                        trs.highlightColorMultiplier, trs.highlightAlphaMultiplier);
                }
                else
                {
                    if (trs.alphaOnly)
                    {
                        wantColor.r = trs.normalColor.r;
                        wantColor.g = trs.normalColor.g;
                        wantColor.b = trs.normalColor.b;
                    }
                }

                wantColor = (Color)(Color32)(wantColor);

                trs.target.transform.localScale = new Vector3(wantSize, wantSize, wantSize);
                transitions[i].target.color = wantColor;
            }
        }
    }

    bool inited = false;

    public void Init()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (transitions != null && !inited)
        {
            for (int i = 0; i < transitions.Length; i++)
                if (transitions[i].target != null && transitions[i].normalColor == Color.clear)
                    transitions[i].normalColor = transitions[i].target.color;
        }

        MReset();
        inited = true;
    }

    void Start()
    {
        Init();
        rt = transform as RectTransform;
    }

    void OnEnable()
    {
        updating = true;

        if (inited)
        {
            bool disabled = false;
            if (button != null)
                disabled = !button.enabled || !button.interactable;
            if (transitions != null)
            {
                for (int i = 0; i < transitions.Length; i++)
                {
                    if (transitions[i].target != null)
                    {
                        if (highlighted)
                        {
                            transitions[i].target.color =
                                transitions[i].highlightColorOverride.a > 0 ?
                                transitions[i].highlightColorOverride :
                                transitions[i].normalColor;
                        }
                        else if (disabled)
                        {
                            transitions[i].target.color = transitions[i].disabledColor;
                        }
                        else
                        {
                            transitions[i].target.color = transitions[i].normalColor;
                        }
                    }
                }
            }
        }
    }

    void OnDisable()
    {
        isPointerEnter = false;
        isPointerDown = false;

        if (tip != null)
            tip.gameObject.SetActive(false);

        enterTime = 0;
    }

    void OnDestroy()
    {
        if (tip != null)
        {
            GameObject.Destroy(tip.gameObject);
            tip = null;
        }
    }

    public void CloseTip()
    {
        enterTime = 0;
        if (tip != null)
        {
            GameObject.Destroy(tip.gameObject);
            tip = null;
        }
    }

    public void ResetTipDelay()
    {
        enterTime = 0;
    }

    bool prevDisabled;
    bool prevHighlighted;
    float enterTime = 0;
    public float pointerEnterTime
    {
        get { return enterTime; }
    }
    public void LateUpdate()
    {
        if (input != null)
        {
            highlighted = input.isFocused;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (EventSystem.current.currentSelectedGameObject == gameObject)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }
        if (prevHighlighted != highlighted)
        {
            prevHighlighted = highlighted;
            updating = true;
        }

        bool disabled = false;
        if (button != null)
            disabled = !button.enabled || !button.interactable;
        if (prevDisabled != disabled)
        {
            prevDisabled = disabled;
            updating = true;
        }

        //closeTip = false;
        if (!updating)
            return;

        bool not_done_yet = false;

        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].target != null)
            {
                Transition trs = transitions[i];
                float damp = trs.damp;
                Color nowColor = trs.target.color;
                Color wantColor = trs.normalColor;
                if (wantColor.r == 0 && wantColor.g == 0 && wantColor.b == 0 && wantColor.a == 0)
                    wantColor = nowColor;
                float nowSize = transitions[i].target.transform.localScale.x;
                float wantSize = 1f;

                if (disabled)
                {
                    wantColor = trs.disabledColor;
                    nowColor = wantColor;
                    if (transitions[i].target.color != nowColor)
                        transitions[i].target.color = nowColor;
                }
                else if (isPointerEnter && isPointerDown)
                {
                    wantSize = trs.pressedSize;
                    wantColor = trs.pressedColor;
                }
                else if (isPointerEnter && !isPointerDown)
                {
                    wantSize = trs.mouseoverSize;
                    wantColor = trs.mouseoverColor;
                }

                if (trs.alphaOnly)
                {
                    wantColor.r = nowColor.r;
                    wantColor.g = nowColor.g;
                    wantColor.b = nowColor.b;
                }

                if (highlighted)
                {
                    wantSize *= trs.highlightSizeMultiplier;
                    if (trs.highlightColorOverride.a > 0)
                        wantColor = trs.highlightColorOverride;
                    else
                        wantColor *= new Color(
                        trs.highlightColorMultiplier, trs.highlightColorMultiplier,
                        trs.highlightColorMultiplier, trs.highlightAlphaMultiplier);
                }
                else
                {
                    if (trs.alphaOnly)
                    {
                        wantColor.r = trs.normalColor.r;
                        wantColor.g = trs.normalColor.g;
                        wantColor.b = trs.normalColor.b;
                    }
                }

                wantColor = (Color)(Color32)(wantColor);

                float sizediff = wantSize - nowSize;
                float colordiff = new Vector4(wantColor.r - nowColor.r, wantColor.g - nowColor.g, wantColor.b - nowColor.b, wantColor.a - nowColor.a).sqrMagnitude;

                if (sizediff == 0f)
                {
                    ;// do nothing
                }
                else if (sizediff < 1e-3)
                {
                    trs.target.transform.localScale = new Vector3(wantSize, wantSize, wantSize);
                }
                else
                {
                    nowSize = nowSize * (1 - damp) + wantSize * damp;
                    trs.target.transform.localScale = new Vector3(nowSize, nowSize, nowSize);
                    not_done_yet = true;
                }

                if (colordiff == 0f)
                {
                    ;// do nothing
                }
                else if (colordiff < 7e-5)
                {
                    if (transitions[i].target.color != wantColor)
                        transitions[i].target.color = wantColor;
                }
                else
                {
                    nowColor.r = nowColor.r * (1 - damp) + wantColor.r * damp;
                    nowColor.g = nowColor.g * (1 - damp) + wantColor.g * damp;
                    nowColor.b = nowColor.b * (1 - damp) + wantColor.b * damp;
                    nowColor.a = nowColor.a * (1 - damp) + wantColor.a * damp;

                    if (transitions[i].target.color != nowColor)
                        transitions[i].target.color = nowColor;
                    not_done_yet = true;
                }
            }
        }

        if (!not_done_yet)
            updating = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerEnter = true;
        updating = true;
        enterTime = 0;

        bool buttonActive = true;
        if (button != null)
            buttonActive = button.enabled && button.interactable;

        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].damp == 0f)
            {
                if (transitions[i].mouseoverSize > 0f)
                    transitions[i].target.transform.localScale = new Vector3(transitions[i].mouseoverSize, transitions[i].mouseoverSize, transitions[i].mouseoverSize);

                if (transitions[i].mouseoverColor != transitions[i].normalColor)
                    transitions[i].target.color = transitions[i].mouseoverColor;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerEnter = false;
        updating = true;
        enterTime = 0;
        if (tip != null)
        {
            tip.gameObject.SetActive(false);
        }

        bool buttonActive = true;
        if (button != null)
            buttonActive = button.enabled && button.interactable;

        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].damp == 0f)
            {
                if (transitions[i].mouseoverSize > 0f)
                    transitions[i].target.transform.localScale = new Vector3(1f, 1f, 1f);

                if (transitions[i].mouseoverColor != transitions[i].normalColor)
                    transitions[i].target.color = transitions[i].normalColor;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        updating = true;

        bool buttonActive = true;
        if (button != null)
            buttonActive = button.enabled && button.interactable;

        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].damp == 0f)
            {
                if (transitions[i].pressedSize > 0f)
                    transitions[i].target.transform.localScale = new Vector3(transitions[i].pressedSize, transitions[i].pressedSize, transitions[i].pressedSize);

                if (transitions[i].pressedColor != transitions[i].normalColor)
                    transitions[i].target.color = transitions[i].pressedColor;
            }
        }

        if (button == null || button.isActiveAndEnabled && button.interactable)
        {
            if (onMouseDown != null)
                onMouseDown(data, eventData.button);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        updating = true;

        bool buttonActive = true;
        if (button != null)
            buttonActive = button.enabled && button.interactable;

        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].damp == 0f)
            {
                if (transitions[i].pressedSize > 0f)
                    transitions[i].target.transform.localScale = new Vector3(1f, 1f, 1f);

                if (transitions[i].pressedColor != transitions[i].normalColor)
                    transitions[i].target.color = transitions[i].normalColor;
            }
        }
    }

    float lastClickTime = -1;

    public void OnPointerClick(PointerEventData eventData)
    {
        bool _enable = button == null || button.isActiveAndEnabled && button.interactable;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Mathf.Abs(Time.time - lastClickTime) < 0.25f)
            {
                if (_enable && onDoubleClick != null)
                    onDoubleClick(data);
                lastClickTime = -1;
            }
            else
            {
                lastClickTime = Time.time;
            }

            if (_enable && onClick != null)
                onClick(data);
            if (onClickEnable != null)
                onClickEnable(data, _enable);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_enable && onRightClick != null)
                onRightClick(data);
        }
    }

    public void MReset()
    {
        for (int i = 0; transitions != null && i < transitions.Length; i++)
        {
            if (transitions[i].target != null)
            {
                if (transitions[i].target.color != transitions[i].normalColor)
                    transitions[i].target.color = transitions[i].normalColor;
            }
        }

        if (button != null)
            prevDisabled = !button.enabled || !button.interactable;
        else
            prevDisabled = false;
        prevHighlighted = highlighted;
        updating = true;
    }

    public bool _isPointerDown { get { return isPointerDown; } }
    public bool _isPointerEnter { get { return isPointerEnter; } }
}
