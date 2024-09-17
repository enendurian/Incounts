using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class AddWalletUI : MonoBehaviour
{
    [Header("UIElements")]
    public RectTransform uimainRect;
    public InputField walletName;
    public InputField balance;
    public Text confirmButtonText;

    [Header("Settings")]
    public Animator anim;

    public bool isOpened;

    private bool isEditMode;
    private int pKey;
    private decimal originalBalance = 0;

    public void OpenPanel(bool isEditMode = false, int pKey = -1)
    {
        if (isOpened)
            return;
        isOpened = true;
        uimainRect.gameObject.SetActive(true);
        anim.Play("in");
        this.isEditMode = isEditMode;
        this.pKey = pKey;
        if (isEditMode)
        {
            InitEditMode();
        }
        else
        {
            confirmButtonText.text = "ȷ�����";
        }
        UIManager.Instance.SetUIStatus(UIStatus.IWindowOpened);
    }

    private void InitEditMode()
    {
        confirmButtonText.text = "Ӧ���޸�";
        DataManager.Instance.ShowDetailsOfWallet(pKey, ShowEditDatas);
    }
    private void ShowEditDatas(SqliteDataReader reader)
    {
        walletName.text = reader.GetString(1);
        originalBalance = (decimal)reader.GetDouble(2);
        balance.text = $"{originalBalance}";
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
        bool realBalance =decimal.TryParse(balance.text, out decimal balancef);
        if (!realBalance)
        {
            TipManager.Instance.AddTipToShow("��Ч�����");
            return;
        }
        if (walletName.text == null || walletName.text == "")
        {
            TipManager.Instance.AddTipToShow("ȱ������");
            return;
        }

        if (isEditMode)
        {
            DataManager.Instance.UpdateWallet(walletName.text, realBalance ? balancef : 0, pKey);
            if (balancef != originalBalance)
            {
                decimal gap = balancef - originalBalance;
                int isOut = gap > 0 ? 1 : 0;
                DataManager.Instance.AddAccount($"ǿ��ͬ��Ǯ����{walletName.text}", DataManager.Instance.today, isOut, gap > 0 ? gap : -gap, (int)AppConst.AccountTypes.Others, "0_0", "ǿ��ͬ��Ǯ����������©����ʱ��ݽ�����ͬ��", pKey);
            }
        }
        else
        {
            DataManager.Instance.AddWallet(walletName.text, realBalance ? balancef : 0);
        }
        EventCenter.TriggerEvent(AppConst.EventNamesConst.RefreshWalletList);
        EventCenter.TriggerEvent(AppConst.EventNamesConst.RefreshWalletData);
        EventCenter.TriggerEvent(AppConst.EventNamesConst.RefreshAccountList);
        StartCoroutine(DelayActiveAndSetClosed());
    }
    #endregion

    IEnumerator DelayActiveAndSetClosed()
    {
        yield return new WaitForSeconds(0.35f);
        isOpened = false;
        uimainRect.gameObject.SetActive(false);
        UIManager.Instance.SetUIStatus(UIStatus.NoWindow);
    }
}
