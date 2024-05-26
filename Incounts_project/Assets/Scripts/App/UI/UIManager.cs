using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private UIStatus _currentStatus;
    public UIStatus CurrentStatus
    {
        get { return _currentStatus; }
    }

    public AddAccountUI addAccountUI;
    public AddWalletUI addWalletUI;
    public List<UIPagesBase> pages;
    public List<Image> bottomButtonImages;

    int currentPageIndex = -1;

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        int walletCount = DataManager.Instance.GetWalletCount();
        if (walletCount > 0)
        {
            OnPageButtonClick(0);
            return;
        }
        OnPageButtonClick(1);
    }

    public void SetUIStatus(UIStatus uIStatus)
    {
        _currentStatus = uIStatus;
    }

    public void OnPageButtonClick(int btnIndx)
    {
        if (_currentStatus != UIStatus.NoWindow) return;
        if (btnIndx == currentPageIndex) return;

        if (currentPageIndex >= 0 && currentPageIndex < pages.Count)
        {
            AnimManager.instance.ImageAlphaChange(bottomButtonImages[currentPageIndex], 1, 0.5f, 0.3f, 1);
            pages[currentPageIndex].ClosePageUI();
        }
        pages[btnIndx].ShowPageUI();
        AnimManager.instance.ImageAlphaChange(bottomButtonImages[btnIndx], 0.5f, 1, 0.3f, 1);
        currentPageIndex = btnIndx;
    }

    public void OnAddAccountClick()
    {
        if (DataManager.Instance.GetWalletCount() <= 0)
        {
            TipManager.Instance.AddTipToShow("先加入钱包才能记账");
            return;
        }
        addAccountUI.OpenPanel();
    }

    public void OnAddWalletClick()
    {
        addWalletUI.OpenPanel();
    }

    public void OnEditAccountClick(int pKey)
    {
        addAccountUI.OpenPanel(true, pKey);
    }

    public void OnEditWalletClick(int pKey)
    {
        addWalletUI.OpenPanel(true, pKey);
    }
}

public enum UIStatus
{
    NoWindow,
    IWindowOpened,
}
