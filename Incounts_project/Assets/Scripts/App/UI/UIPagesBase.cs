using UnityEngine;

public class UIPagesBase : MonoBehaviour
{
    [Header("BaseUIElements")]
    public RectTransform mainPanel;
    public CanvasGroup mainCanvasGroup;

    protected bool isShowing;
    protected bool isDirty = true;

    public void ShowPageUI()
    {
        mainPanel.gameObject.SetActive(true);
        isShowing = true;
        //AnimManager.instance.CanvasGroupAlphaChange(mainCanvasGroup, 0, 1, 0.2f, 1);
        if (isDirty)
        {
            RefreshAllUI();
            isDirty = false;
        }
    }

    public void ClosePageUI()
    {
        isShowing = false;
        mainPanel.gameObject.SetActive(false);
        //AnimManager.instance.CanvasGroupAlphaChange(mainCanvasGroup, 1, 0, 0.2f, 1);
        //AnimManager.instance.DelayActive(mainPanel.gameObject, false, 0.3f);
    }

    public virtual void RefreshAllUI() { }
}
