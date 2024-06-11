using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM_LogsOnly : MonoBehaviour
{
    public Text logText;
    public RectTransform logInfoContent;
    public ScrollRect logScrollView;

    int count = 0;

    // Start is called before the first frame update
    void Awake()
    {
        Application.logMessageReceived += HandleLog;
        //DontDestroyOnLoad(this.transform.parent.gameObject);
    }

    void HandleLog(string condition, string stackTrace, LogType type)
    {
        //string message = string.Format("condition = {0} \n stackTrace = {1} \n type = {2}", condition, stackTrace, type);
        string message = condition;
        if (type == LogType.Error || type == LogType.Exception)
            message = "<color=#FF5151>" + message + "</color>";
        else if (type == LogType.Warning)
            message = "<color=#FFDE40>" + message + "</color>";
        message = $"{message} {stackTrace}";
        LogAdd(message);
    }

    void LogAdd(string info)
    {
        count++;
        if (count > 15)
        {
            logText.text = "";
            count = 0;
        }
        logText.text = logText.text + "\n" + info;
        logInfoContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, logText.preferredHeight + 30);
        logScrollView.verticalNormalizedPosition = 0;
    }
}
