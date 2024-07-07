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
    public Image icon;

    [Header("Settings")]
    public Animator anim;

    private AccountTypes[] currentTypes
    {
        get { return isOut ? AppConst.BasicConsts.outTypes : AppConst.BasicConsts.inTypes; }
    }
    private List<WalletDataItem> walletList;

    private int walletListIndex;
    private bool isOut;
    private int typeIndex;

    public bool isOpened;

    private bool isEditMode;
    private int pKey;
    private decimal originalCount = 0;
    private int originalWalletId;
    private int currentIconIndex = 0;
    private List<string> currentIconList;

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
        mainBalanceText.text = $"当前总余额：{DataManager.Instance.GetWalletRemains()}";
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
        currentIconList = AppConst.BasicConsts.iconListDict[(int)currentTypes[typeIndex]];
        currentIconIndex = 0;
        RefreshIcon(currentIconList[currentIconIndex]);
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
        currentIconList = AppConst.BasicConsts.iconListDict[(int)currentTypes[typeIndex]];
        RefreshIcon(reader.GetString(6));
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
        int typeNum = (int)currentTypes[typeIndex];
        typeText.text = BasicConsts.TypeNames[typeNum];
    }

    private void RefreshIcon(string iconUrl)
    {
        string url = string.Format(AppConst.BasicConsts.addressIcon, iconUrl);
        AddressableManager.LoadAssetAsync<Sprite>(url, (texture) => { icon.sprite = texture; });
    }

    #region button func
    public void OnClickChangeInOut()
    {
        isOut = !isOut;
        typeIndex = 0;
        RefreshInOutText();
        RefreshTypeText();
        currentIconList = AppConst.BasicConsts.iconListDict[(int)currentTypes[typeIndex]];
        currentIconIndex = 0;
        RefreshIcon(currentIconList[currentIconIndex]);
    }

    public void OnClickChangeType()
    {
        typeIndex++;
        if (typeIndex >= currentTypes.Length)
        {
            typeIndex = 0;
        }
        currentIconList = AppConst.BasicConsts.iconListDict[(int)currentTypes[typeIndex]];
        currentIconIndex = 0;
        RefreshTypeText();
        RefreshIcon(currentIconList[currentIconIndex]);
    }

    public void OnClickChangeIcon()
    {
        currentIconIndex++;
        if (currentIconIndex >= currentIconList.Count)
        {
            currentIconIndex = 0;
        }
        RefreshIcon(currentIconList[currentIconIndex]);
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
        string iconId = currentIconList[currentIconIndex];

        if (isEditMode)
        {
            DataManager.Instance.UpdateAccount(titleInput.text, realDay ? dayi : DataManager.Instance.today, isOut ? 0 : 1, countf, (int)currentTypes[typeIndex], iconId, messageInput.text, walletList[walletListIndex].index, pKey);

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
            DataManager.Instance.AddAccount(titleInput.text, realDay ? dayi : DataManager.Instance.today, isOut ? 0 : 1, countf, (int)currentTypes[typeIndex], iconId, messageInput.text, walletList[walletListIndex].index);
            DataManager.Instance.UpdateWallet(isOut ? -countf : countf, walletList[walletListIndex].index);
        }
        anim.Play("confirmed");
        EventCenter.TriggerEvent(AppConst.EventNamesConst.RefreshWalletData);
        EventCenter.TriggerEvent(AppConst.EventNamesConst.RefreshWalletList);
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
