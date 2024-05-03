using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AppConst;

public class AddAccountUI : MonoBehaviour
{
    [Header("UIElements")]
    public InputField titleInput;
    public InputField countInput;
    public InputField messageInput;
    public InputField monthInput;
    public InputField dateInput;
    public Text walletText;
    public Text inoutText;
    public Text typeText;

    [Header("Settings")]
    public Animator anim;

    private int walletIndex;
    private bool isOut;
    private int typeIndex;

    public bool isOpened;

    #region button func
    public void OnCancelClicked()
    {
        if (!isOpened) return;
        isOpened = false;
    }

    public void OnConfirmClicked()
    {
        if (!isOpened) return;
        isOpened = false;
    }
    #endregion
}
