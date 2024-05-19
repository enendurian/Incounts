using UnityEngine;

public class UIPagesBase : MonoBehaviour
{
    [Header("BaseUIElements")]
    public RectTransform mainPanel;
    public CanvasGroup mainCanvasGroup;

    protected bool isShowing;

    public void ShowPageUI()
    {
        mainPanel.gameObject.SetActive(true);
        isShowing = true;
        AnimManager.instance.CanvasGroupAlphaChange(mainCanvasGroup, 0, 1, 0.2f, 1);
        RefreshAllUI();
    }

    public void ClosePageUI()
    {
        isShowing = false;
        AnimManager.instance.CanvasGroupAlphaChange(mainCanvasGroup, 1, 0, 0.2f, 1);
        AnimManager.instance.DelayActive(mainPanel.gameObject, false, 0.3f);
    }

    public virtual void RefreshAllUI() { }
}
