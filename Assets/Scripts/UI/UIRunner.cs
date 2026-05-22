using UnityEngine;

public class UIRunner : MonoBehaviour
{
    public UIGame uiRoot;

    private void Awake()
    {
        uiRoot._Create();
        uiRoot._Init(null);
        uiRoot._Open();
    }
}
