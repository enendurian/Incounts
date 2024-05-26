using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AppConst;
using Mono.Data.Sqlite;

public class AddAccountUI : MonoBehaviour
{
    [Header("UIElements")]
    public RectTransform uimainRect;
    public InputField titleInput;
    public InputField countInput;
    public InputField messageInput;
    public InputField dateInput;
    public Text mainBalanceText;
    public Text confirmButtonText;
    public Text monthText;
    public Text walletText;
    public Text inoutText;
    public Text typeText;

    [Header("Settings")]
    public Animator anim;

    private AccountTypes[] outTypes = BasicConsts.outTypes;
    private AccountTypes[] inTypes = BasicConsts.inTypes;
    private List<WalletDataItem> walletList;

    private int walletListIndex;
    private bool isOut;
    private int typeIndex;
    private string iconId;

    public bool isOpened;

    private bool isEditMode;
    private int pKey;
    private decimal originalCount = 0;
    private int originalWalletId;

    public void OpenPanel(bool isEditMode = false, int pKey = -1)
    {
        if (isOpened)
            return;
        isOpened = true;
        uimainRect.gameObject.SetActive(true);
        anim.Play("in");
        walletList = DataManager.Instance.walletDataList;
        this.isEditMode = isEditMode;
        this.pKey = pKey;
        if (isEditMode)
        {
            InitInEditModeUI();
        }
        else
        {
            InitUI();
        }
        mainBalanceText.text = $"总余额：{DataManager.Instance.GetWalletRemains()}";
        RefreshInOutText();
        RefreshTypeText();
        UIManager.Instance.SetUIStatus(UIStatus.IWindowOpened);
    }

    private void InitUI()
    {
        monthText.text = $"{DataManager.Instance.currentShowingYear}-{DataManager.Instance.currentShowingMonth}-";
        titleInput.text = "";
        messageInput.text = "";
        countInput.text = "";
        isOut = true;
        dateInput.text = $"{DataManager.Instance.today}";
        typeIndex = 0;
        //iconId
        walletListIndex = 0;
        walletText.text = walletList[walletListIndex].name;
        confirmButtonText.text = "确认添加";
    }

    private void InitInEditModeUI()
    {
        confirmButtonText.text = "应用修改";
        DataManager.Instance.ShowDetailsOfAccount(pKey, ShowEditDatas);
    }

    private void ShowEditDatas(SqliteDataReader reader)
    {
        titleInput.text = reader.GetString(1);
        monthText.text = $"{DataManager.Instance.currentShowingYear}-{DataManager.Instance.currentShowingMonth}-";
        isOut = reader.GetInt32(3) <= 0;
        dateInput.text = $"{reader.GetInt32(2)}";
        originalCount = reader.GetDecimal(4);
        countInput.text = $"{originalCount}";
        originalCount = isOut ? -originalCount : originalCount;
        typeIndex = reader.GetInt32(5);
        //iconId
        messageInput.text = reader.GetString(7);
        originalWalletId = reader.GetInt32(8);
        walletListIndex = DataManager.Instance.WalletIndex2ListIndex(originalWalletId);
        walletText.text = walletList[walletListIndex].name;
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
        walletListIndex++;
        if (walletListIndex >= walletList.Count)
        {
            walletListIndex = 0;
        }
        walletText.text = walletList[walletListIndex].name;
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
        bool realCount = decimal.TryParse(countInput.text, out decimal countf);
        bool realDay = int.TryParse(dateInput.text, out int dayi);
        if (!realCount)
        {
            TipManager.Instance.AddTipToShow("无效的账目");
            return;
        }
        if (!realDay)
        {
            TipManager.Instance.AddTipToShow("非法日期");
            return;
        }
        if (titleInput.text == null || titleInput.text == "")
        {
            TipManager.Instance.AddTipToShow("缺少标题");
            return;
        }
        if (countf <= 0 || dayi <= 0)
        {
            TipManager.Instance.AddTipToShow("非法数据：非正数");
            return;
        }

        //临时数据
        iconId = "0_0";

        if (isEditMode)
        {
            DataManager.Instance.UpdateAccount(titleInput.text, realDay ? dayi : DataManager.Instance.today, isOut ? 0 : 1, countf, typeIndex, iconId, messageInput.text, walletList[walletListIndex].index, pKey);

            if (originalWalletId == walletList[walletListIndex].index)
            {
                decimal sumOfChange = isOut ? -countf : countf;
                sumOfChange -= originalCount;
                DataManager.Instance.UpdateWallet(sumOfChange, walletList[walletListIndex].index);
                Debug.Log($"钱包动账：{sumOfChange}, 在{walletList[walletListIndex].name}");
            }
            else
            {
                DataManager.Instance.UpdateWallet(-originalCount, originalWalletId);
                DataManager.Instance.UpdateWallet(isOut ? -countf : countf, walletList[walletListIndex].index);
            }
        }
        else
        {
            DataManager.Instance.AddAccount(titleInput.text, realDay ? dayi : DataManager.Instance.today, isOut ? 0 : 1, countf, typeIndex, iconId, messageInput.text, walletList[walletListIndex].index);
            DataManager.Instance.UpdateWallet(isOut ? -countf : countf, walletList[walletListIndex].index);
        }
        anim.Play("confirmed");
        EventCenter.TriggerEvent(AppConst.EventNamesConst.RefreshWalletData);
        EventCenter.TriggerEvent(AppConst.EventNamesConst.RefreshAccountList);
        StartCoroutine(DelayActiveAndSetClosed());
    }
    #endregion

    IEnumerator DelayActiveAndSetClosed()
    {
        yield return new WaitForSeconds(0.35f);
        isOpened = false;
        UIManager.Instance.SetUIStatus(UIStatus.NoWindow);
        uimainRect.gameObject.SetActive(false);
    }
}
