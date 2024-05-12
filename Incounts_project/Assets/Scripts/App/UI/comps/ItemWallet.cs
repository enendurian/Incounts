using System.Collections;
using System.Collections.Generic;
using AppConst;
using UnityEngine;
using UnityEngine.UI;

public class ItemWallet : MonoBehaviour
{
    [Header("UIElements")]
    public Text nameText;
    public Text balanceText;
    public RectTransform thisTransform;
    public CanvasGroup editCanvasGroup;

    private int primaryKey;

    private bool ispoened = false;

    #region settings
    public int originalHeight =WalletListConst.Height_Wallet;
    public int aimShowingHeight = 260;
    public float transformTimeGap = 0.5f;
    #endregion

    public void RefreshWalletData(int pKey, string name, float balance)
    {
        primaryKey = pKey;
        nameText.text = name;
        balanceText.text = $"{balance}";
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
        while (timeCount <= transformTimeGap)
        {
            yield return null;
            timeCount += Time.deltaTime;
            float progress = timeCount / transformTimeGap;
            thisTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(thisTransform.rect.height, aimShowingHeight, progress));
            editCanvasGroup.alpha = Mathf.Lerp(editCanvasGroup.alpha, 1, progress);
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
            editCanvasGroup.alpha = Mathf.Lerp(editCanvasGroup.alpha, 0, progress);
        }
        HardReset();
    }

    /// <summary>
    /// 强制设置为初始状态为
    /// </summary>
    public void HardReset()
    {
        thisTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalHeight);
        editCanvasGroup.alpha = 0;
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
