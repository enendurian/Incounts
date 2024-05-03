using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformedInput
{
    #region Input_On_Platform

    /// <summary>
    /// ���λ��
    /// </summary>
    /// <returns>λ��</returns>
    public static Vector2 ClickedPosition()
    {
        Vector2 p = Vector2.zero;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
            p = Input.touches[0].position;
#elif UNITY_EDITOR_WIN
        p = Input.mousePosition;
#endif
        return p;
    }

    /// <summary>
    /// �Ƿ񵥵�
    /// </summary>
    /// <returns>�Ƿ�</returns>
    public static bool SingleClick()
    {
        bool result = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
            result = (Input.touches[0].phase == TouchPhase.Began);
        else
            result = false;
#elif UNITY_EDITOR_WIN
        result = Input.GetMouseButtonDown(0);
#endif
        return result;
    }

    /// <summary>
    /// �Ƿ���
    /// </summary>
    /// <returns>�Ƿ�</returns>
    public static bool Holding()
    {
        bool result = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
            result = (Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary);
        else
            result = false;
#elif UNITY_EDITOR_WIN
        result = Input.GetMouseButton(0);
#endif
        return result;
    }

    /// <summary>
    /// �Ƿ�̧��
    /// </summary>
    /// <returns>�Ƿ�</returns>
    public static bool SingleUp()
    {
        bool result = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
            result = (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled);
        else
            result = false;
#elif UNITY_EDITOR_WIN
        result = Input.GetMouseButtonUp(0);
#endif
        return result;
    }

    /// <summary>
    /// �������ƶ��ٶ�
    /// </summary>
    /// <returns></returns>
    public static Vector2 DragForCamera()
    {
        Vector2 input = Vector2.zero;

#if UNITY_ANDROID && !UNITY_EDITOR
         if (Input.touchCount > 0)
         {
             input = new Vector2(
                 Input.GetTouch(0).deltaPosition.x,
                 Input.GetTouch(0).deltaPosition.y
             );
         }
#elif UNITY_EDITOR_WIN
        if (Input.GetMouseButton(0))
        {
            input = new Vector2
            (
                //x , y
                Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y")
            );
        }
#endif
        return input;
    }
    #endregion
}