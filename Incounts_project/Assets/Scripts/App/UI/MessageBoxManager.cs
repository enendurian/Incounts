using System;
using AppConst;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxManager : MonoBehaviour
{
    [Header("UIElements")]
    public GameObject messageBoxGO;
    public RectTransform thisTransform;
    public CanvasGroup messageBoxCanvasGroup;
    public Text messageText;

    public static MessageBoxManager Instance;

    private bool _isBoxShowing
    {
        get { return messageBoxGO.activeSelf; }
    }
    private Action _confirmAction;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void MessageBoxShow(string message, Action confirmAction)
    {
        if (_isBoxShowing)
            return;

        messageText.text = message;
        _confirmAction = confirmAction;
        messageBoxGO.SetActive(true);
        AnimManager.instance.CanvasGroupAlphaChange(messageBoxCanvasGroup, 0, 1, 0.2f, 1);
        UIManager.Instance.SetUIStatus(UIStatus.IWindowOpened);
        EventCenter.RegisterListener(EventNamesConst.SingleClick, ListenClick);
    }

    private void CloseMessageBox()
    {
        AnimManager.instance.CanvasGroupAlphaChange(messageBoxCanvasGroup, 1, 0, 0.2f, 1);
        AnimManager.instance.DelayActive(messageBoxGO, false, 0.2f);
        UIManager.Instance.SetUIStatus(UIStatus.NoWindow);
        EventCenter.RemoveListener(EventNamesConst.SingleClick, ListenClick);
    }

    private void ListenClick()
    {
        Vector2 clickPos = PlatformedInput.ClickedPosition();
        if (!RectTransformUtility.RectangleContainsScreenPoint(thisTransform, clickPos))
        {
            CloseMessageBox();
        }
    }

    #region ButtonFunc
    public void OnconfirmButtonClicked()
    {
        _confirmAction?.Invoke();
        CloseMessageBox();
    }

    public void OnCnacelButtonClicked()
    {
        CloseMessageBox();
    }
    #endregion
}
