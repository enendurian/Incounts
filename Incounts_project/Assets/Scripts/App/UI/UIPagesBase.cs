using UnityEngine;

public class UIPagesBase : MonoBehaviour
{
    [Header("BaseUIElements")]
    public RectTransform mainPanel;
    public CanvasGroup mainCanvasGroup;

    public void ShowPageUI()
    {
        mainPanel.gameObject.SetActive(true);
        AnimManager.instance.CanvasGroupAlphaChange(mainCanvasGroup, 0, 1, 0.3f, 1);
        RefreshAllUI();
    }

    public void ClosePageUI()
    {
        AnimManager.instance.CanvasGroupAlphaChange(mainCanvasGroup, 1, 0, 0.3f, 1);
        AnimManager.instance.DelayActive(mainPanel.gameObject, false, 0.5f);
    }

    public virtual void RefreshAllUI() { }
}
