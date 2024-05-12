using System.Collections;
using AppConst;
using UnityEngine;
using UnityEngine.UI;

public class ItemAccount : MonoBehaviour
{
    [Header("UIElements")]
    public Text titleText;
    public Text countText;
    public Image icon;
    public RectTransform thisTransform;
    public RectTransform messageBg;
    public Text messageText;
    public Text typeText;
    public CanvasGroup messageCanvasGroup;

    private int primaryKey;

    private bool ispoened = false;

    #region settings
    public int originalHeight = AccountListConst.Height_Account;
    public int aimShowingHeight = 400;
    public float transformTimeGap = 0.5f;
    #endregion

    public void RefreshAccountData(int pKey, string title, int isOut, double count, string iconUrl)
    {
        primaryKey = pKey;
        titleText.text =title;
        string head = isOut<=0 ? $"<color={BasicConsts.outgoColor}>-" : $"<color={BasicConsts.incomeColor}>+";
        countText.text = $"{head}{count}</color>";
    }

    public void OnItemClicked()
    {
        if (ispoened)
        {
            OnCloseMessageShow();
            return;
        }
        ispoened = true;
        StartCoroutine(ShowDetails());
        EventCenter.RegisterListener(EventNamesConst.SingleClick, ListenClick);
    }

    void OnCloseMessageShow()
    {
        ispoened = false;
        StartCoroutine(HideDetails());
        EventCenter.RemoveListener(EventNamesConst.SingleClick, ListenClick);
    }

    IEnumerator ShowDetails()
    {
        float timeCount = 0;

        DataManager.Instance.ShowDetailsOfAccount(this, primaryKey);
        while (timeCount <= transformTimeGap)
        {
            yield return null;
            timeCount += Time.deltaTime;
            float progress = timeCount / transformTimeGap;
            thisTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(thisTransform.rect.height, aimShowingHeight, progress));
            messageCanvasGroup.alpha = Mathf.Lerp(messageCanvasGroup.alpha, 1, progress);
        }
    }

    IEnumerator HideDetails()
    {
        float timeCount = 0;
        while (timeCount <= transformTimeGap)
        {
            yield return null;
            timeCount += Time.deltaTime;
            float progress = timeCount / transformTimeGap;
            thisTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(thisTransform.rect.height, originalHeight, progress));
            messageCanvasGroup.alpha = Mathf.Lerp(messageCanvasGroup.alpha, 0, progress);
        }
        HardReset();
    }

    /// <summary>
    /// 强制设置为初始状态为
    /// </summary>
    public void HardReset()
    {
        thisTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalHeight);
        messageCanvasGroup.alpha = 0;
    }

    private void ListenClick()
    {
        Vector2 clickPos = PlatformedInput.ClickedPosition();
        if (!RectTransformUtility.RectangleContainsScreenPoint(thisTransform, clickPos))
        {
            OnCloseMessageShow();
        }
    }
}
