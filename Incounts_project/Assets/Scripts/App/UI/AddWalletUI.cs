using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWalletUI : MonoBehaviour
{
    [Header("UIElements")]
    public RectTransform uimainRect;
    public InputField walletName;
    public InputField balance;

    [Header("Settings")]
    public Animator anim;

    public bool isOpened;

    public void OpenPanel()
    {
        if (isOpened)
            return;
        isOpened = true;
        uimainRect.gameObject.SetActive(true);
        anim.Play("in");
    }

    #region button func
    public void OnCancelClicked()
    {
        if (!isOpened) return;
        anim.Play("canceled");
        StartCoroutine(DelayActiveAndSetClosed());
    }

    public void OnConfirmClicked()
    {
        if (!isOpened) return;
        anim.Play("confirmed");
        bool realBalance = float.TryParse(balance.text, out float balancef);
        DataManager.Instance.AddWallet(walletName.text, realBalance ? balancef : 0);
        DataManager.Instance.RefreshWalletData();
        EventCenter.TriggerEvent(AppConst.EventNamesConst.AddWallet);
        StartCoroutine(DelayActiveAndSetClosed());
    }
    #endregion

    IEnumerator DelayActiveAndSetClosed()
    {
        yield return new WaitForSeconds(0.35f);
        isOpened = false;
        uimainRect.gameObject.SetActive(false);
    }
}
