using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

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
        OnPageButtonClick(0);
    }

    public void OnPageButtonClick(int btnIndx)
    {
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
        addAccountUI.OpenPanel();
    }

    public void OnAddWalletClick()
    {
        addWalletUI.OpenPanel();
    }
}
