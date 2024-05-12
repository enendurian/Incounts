using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AppConst;

public class AddAccountUI : MonoBehaviour
{
    [Header("UIElements")]
    public RectTransform uimainRect;
    public InputField titleInput;
    public InputField countInput;
    public InputField messageInput;
    public InputField dateInput;
    public Text monthText;
    public Text walletText;
    public Text inoutText;
    public Text typeText;

    [Header("Settings")]
    public Animator anim;

    private AccountTypes[] outTypes = BasicConsts.outTypes;
    private AccountTypes[] inTypes = BasicConsts.inTypes;
    private List<WalletDataItem> walletList;

    private int walletIndex;
    private bool isOut;
    private int typeIndex;
    private string iconId;

    public bool isOpened;

    public void OpenPanel()
    {
        if (isOpened)
            return;
        isOpened = true;
        uimainRect.gameObject.SetActive(true);
        anim.Play("in");
        InitUI();
    }

    private void InitUI()
    {
        walletList = DataManager.Instance.walletDataList;

        monthText.text = $"{DataManager.Instance.currentShowingYear}-{DataManager.Instance.currentShowingMonth}-";
        isOut = true;
        dateInput.text = $"{DataManager.Instance.today}";
        RefreshInOutText();
        typeIndex = 0;
        RefreshTypeText();
        //iconId
        walletIndex = 0;
        walletText.text = walletList[walletIndex].name;
    }

    private void RefreshInOutText()
    {
        inoutText.text = isOut ? BasicConsts.OutAccount : BasicConsts.InAccount;
    }

    private void RefreshTypeText()
    {
        AccountTypes[] availableTypes = isOut ? outTypes : inTypes;
        int typeNum = (int)availableTypes[typeIndex];
        typeText.text = BasicConsts.TypeNames[typeNum];
    }

    #region button func
    public void OnClickChangeInOut()
    {
        isOut = !isOut;
        typeIndex = 0;
        RefreshInOutText();
        RefreshTypeText();
    }

    public void OnClickChangeType()
    {
        typeIndex++;
        AccountTypes[] availableTypes = isOut ? outTypes : inTypes;
        if (typeIndex >= availableTypes.Length)
        {
            typeIndex = 0;
        }
        RefreshTypeText();
    }

    public void OnClickChangeWallet()
    {
        walletIndex++;
        if (walletIndex >= walletList.Count)
        {
            walletIndex = 0;
        }
        walletText.text = walletList[walletIndex].name;
    }

    public void OnCancelClicked()
    {
        if (!isOpened) return;
        anim.Play("canceled");
        StartCoroutine(DelayActiveAndSetClosed());
    }

    public void OnConfirmClicked()
    {
        if (!isOpened) return;
        bool realCount = float.TryParse(countInput.text, out float countf);
        bool realDay = int.TryParse(dateInput.text, out int dayi);
        //临时数据
        iconId = "0_0";
        DataManager.Instance.AddAccount(titleInput.text, realDay ? dayi : DataManager.Instance.today, isOut ? 0 : 1, realCount ? countf : 0f, typeIndex, iconId, messageInput.text, walletList[walletIndex].index);
        //对钱包进行更新
        anim.Play("confirmed");
        EventCenter.TriggerEvent(AppConst.EventNamesConst.AddAccount);
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
