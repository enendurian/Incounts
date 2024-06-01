using System.Collections;
using AppConst;
using UnityEngine;
using UnityEngine.UI;

public class ItemAccount : MonoBehaviour
{
    [Header("UIElements")]
    public Text titleText;
    public Text countText;
    public Text walletText;
    public Image icon;
    public RectTransform thisTransform;
    public RectTransform detailTransform;
    public Text messageText;
    public Text typeText;
    public CanvasGroup messageCanvasGroup;

    private int primaryKey;

    private bool isOpened = false;

    #region settings
    public int originalHeight = AccountListConst.Height_Account;
    public int aimShowingHeight = 400;
    public float transformTimeGap = 0.2f;
    #endregion

    public void RefreshAccountData(int pKey, string title, int isOut, decimal count, string walletname, string iconUrl)
    {
        primaryKey = pKey;
        titleText.text = title;
        walletText.text = walletname;
        isOpened = false;
        string head = isOut <= 0 ? $"<color={BasicConsts.outgoColor}>-" : $"<color={BasicConsts.incomeColor}>+";
        countText.text = $"{head}{count}</color>";
    }

    public void OnItemClicked()
    {
        if (isOpened)
        {
            OnCloseMessageShow();
            return;
        }
        isOpened = true;
        StartCoroutine(ShowDetails());
        EventCenter.RegisterListener(EventNamesConst.SingleClick, ListenClick);
    }

    void OnCloseMessageShow()
    {
        if (!isOpened) return;
        isOpened = false;
        StartCoroutine(HideDetails());
        EventCenter.RemoveListener(EventNamesConst.SingleClick, ListenClick);
    }

    IEnumerator ShowDetails()
    {
        float timeCount = 0;

        detailTransform.gameObject.SetActive(true);
        DataManager.Instance.ShowDetailsOfAccount(this, primaryKey);
        while (timeCount <= transformTimeGap && isOpened)
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
        while (timeCount <= transformTimeGap && !isOpened)
        {
            yield return null;
            timeCount += Time.deltaTime;
            float progress = timeCount / transformTimeGap;
            thisTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(thisTransform.rect.height, originalHeight, progress));
            messageCanvasGroup.alpha = Mathf.Lerp(messageCanvasGroup.alpha, 0, progress);
        }
        if (!isOpened)
        {
            detailTransform.gameObject.SetActive(false);
            HardReset();
        }
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

    #region ButtonFunction
    public void OnEditButtonClicked()
    {
        if (UIManager.Instance.CurrentStatus != UIStatus.NoWindow) return;
        UIManager.Instance.OnEditAccountClick(primaryKey);
        OnCloseMessageShow();
    }

    public void OnDeleteButtonClicked()
    {
        MessageBoxManager.Instance.MessageBoxShow("确定删除？相关的账目记录将被回退", DeleteAction);
    }

    private void DeleteAction()
    {
        DataManager.Instance.ShowDetailsOfAccount(primaryKey, (reader) =>
        {
            decimal originalCount = reader.GetDecimal(4);
            int originalWalletId = reader.GetInt32(8);
            bool isOut = reader.GetInt32(3) <= 0;
            originalCount = isOut ? originalCount : -originalCount;
            DataManager.Instance.UpdateWallet(originalCount, originalWalletId);
            DataManager.Instance.DeleteAccount(primaryKey);
            EventCenter.TriggerEvent(AppConst.EventNamesConst.RefreshWalletData);
            EventCenter.TriggerEvent(AppConst.EventNamesConst.RefreshAccountList);
        });
    }
    #endregion
}
