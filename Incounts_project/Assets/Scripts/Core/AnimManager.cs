using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimManager : MonoBehaviour
{
    public static AnimManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    #region IEnum_Animations
    //移动的动画，所计算的是本地坐标，因此试用前请先确定好父物体
    public void ObjectMove(Transform t, Vector3 oriPos, Vector3 pos, Quaternion oriQua, Quaternion qua, float time, float inch)
    {
        StartCoroutine(ObjectMoveIE(t, oriPos, pos, oriQua, qua, time, inch));
    }
    IEnumerator ObjectMoveIE(Transform t, Vector3 oriPos, Vector3 pos, Quaternion oriQua, Quaternion qua, float time, float inch)
    {
        float count = 0;
        while (count < time && t != null)
        {
            count += Time.deltaTime;
            float progress = count / time;
            float inchedp = inch == 1 ? progress : Mathf.Pow(progress, inch);
            t.localPosition = Vector3.Lerp(oriPos, pos, inchedp);
            t.localRotation = Quaternion.Lerp(oriQua, qua, inchedp);
            yield return null;
        }
        if (t != null)
        {
            t.localPosition = pos;
            t.localRotation = qua;
        }
    }

    public void ObjectMove(Transform t, Vector3 oriPos, Vector3 pos, float time, float inch)
    {
        StartCoroutine(ObjectMoveIE(t, oriPos, pos, time, inch));
    }
    IEnumerator ObjectMoveIE(Transform t, Vector3 oriPos, Vector3 pos, float time, float inch)
    {
        float count = 0;
        while (count < time && t != null)
        {
            count += Time.deltaTime;
            float progress = count / time;
            t.localPosition = Vector3.Lerp(oriPos, pos, Mathf.Pow(progress, inch));
            yield return null;
        }
        if (t != null)
            t.localPosition = pos;
    }

    public void ObjectMove(Transform t, Quaternion oriQua, Quaternion qua, float time, float inch)
    {
        StartCoroutine(ObjectMoveIE(t, oriQua, qua, time, inch));
    }
    IEnumerator ObjectMoveIE(Transform t, Quaternion oriQua, Quaternion qua, float time, float inch)
    {
        float count = 0;
        while (count < time && t != null)
        {
            count += Time.deltaTime;
            float progress = count / time;
            t.localRotation = Quaternion.Lerp(oriQua, qua, Mathf.Pow(progress, inch));
            yield return null;
        }
        if (t != null)
            t.localRotation = qua;
    }

    //变形的动画
    public void ObjectSize(Transform t, Vector3 oriScale, Vector3 aimScale, float time, float inch)
    {
        StartCoroutine(ObjectSizeE(t, oriScale, aimScale, time, inch));
    }
    IEnumerator ObjectSizeE(Transform t, Vector3 oriScale, Vector3 aimScale, float time, float inch)
    {
        //this.transform.localScale
        float count = 0;
        while (count < time && t != null)
        {
            count += Time.deltaTime;
            float progress = count / time;
            t.localScale = Vector3.Lerp(oriScale, aimScale, Mathf.Pow(progress, inch));
            yield return null;
        }
        if (t != null)
            t.localScale = aimScale;
    }

    //图片颜色变化
    public void SpriteAlphaChange(SpriteRenderer sr, float originAlpha, float endAlpha, float time, float inch)
    {
        StartCoroutine(SpriteAlphaChangeE(sr, originAlpha, endAlpha, time, inch));
    }
    IEnumerator SpriteAlphaChangeE(SpriteRenderer sr, float originAlpha, float endAlpha, float time, float inch)
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, originAlpha);
        sr.gameObject.SetActive(true);
        float count = 0;
        float progress = 0;
        while (count < time && sr != null)
        {
            count += Time.deltaTime;
            progress = count / time;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.Lerp(originAlpha, endAlpha, Mathf.Pow(progress, inch)));
            yield return null;
        }
        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, endAlpha);
            if (endAlpha <= 0)
                sr.gameObject.SetActive(false);
        }
    }

    //UI动画：rectTransform移动
    public void GentleMove(RectTransform rect, Vector2 oriPos, Vector2 aimPos, float time, float inch)
    {
        StartCoroutine(GentleMoveE(rect, oriPos, aimPos, time, inch));
    }
    IEnumerator GentleMoveE(RectTransform rect, Vector2 oriPos, Vector2 aimPos, float time, float inch)
    {
        float count = 0;
        float progress = 0;
        while (count < time && rect != null)
        {
            count += Time.deltaTime;
            progress = count / time;
            rect.anchoredPosition = Vector2.Lerp(oriPos, aimPos, Mathf.Pow(progress, inch));
            yield return null;
        }
        if (rect != null)
            rect.anchoredPosition = aimPos;
    }

    //UI动画：Text移动和变色
    public void TextColorMove(Text text, Vector2 oriPos, Vector2 aimPos, float oriAlpha, float aimAlpha, float time, float inch)
    {
        StartCoroutine(TextColorMoveE(text, oriPos, aimPos, oriAlpha, aimAlpha, time, inch));
    }
    IEnumerator TextColorMoveE(Text text, Vector2 oriPos, Vector2 aimPos, float oriAlpha, float aimAlpha, float time, float inch)
    {
        float count = 0;
        float progress = 0;
        float colorGap = 0;
        Color textColor = text.color;
        text.color = new Color(textColor.r, textColor.g, textColor.b, oriAlpha);
        RectTransform rect = text.GetComponent<RectTransform>();
        text.gameObject.SetActive(true);
        while (count < time && text != null)
        {
            count += Time.deltaTime;
            progress = count / time;
            colorGap = Mathf.Lerp(oriAlpha, aimAlpha, progress);
            text.color = new Color(textColor.r, textColor.g, textColor.b, colorGap);
            rect.anchoredPosition = Vector2.Lerp(oriPos, aimPos, Mathf.Pow(progress, inch));
            yield return null;
        }
        if (text != null)
        {
            text.color = new Color(textColor.r, textColor.g, textColor.b, aimAlpha);
            rect.anchoredPosition = aimPos;
        }
    }

    //UI动画：CanvasGroup变色
    public void CanvasGroupAlphaChange(CanvasGroup cg, float oriA, float endA, float time, float inch)
    {
        StartCoroutine(CanvasGroupAlphaChangeE(cg, oriA, endA, time, inch));
    }

    IEnumerator CanvasGroupAlphaChangeE(CanvasGroup cg, float oriA, float endA, float time, float inch)
    {
        float count = 0;
        float progress = 0;
        cg.gameObject.SetActive(true);
        while (count < time && cg != null)
        {
            count += Time.deltaTime;
            progress = count / time;
            cg.alpha = Mathf.Lerp(oriA, endA, Mathf.Pow(progress, inch));
            yield return null;
        }
        if (cg != null)
            cg.alpha = endA;
    }

    //UI动画：Image变色
    public void ImageAlphaChange(Image img, float oriA, float endA, float time, float inch)
    {
        StartCoroutine(ImageAlphaChangeE(img, oriA, endA, time, inch));
    }

    IEnumerator ImageAlphaChangeE(Image img, float oriA, float endA, float time, float inch)
    {
        float count = 0;
        float progress = 0;
        float alphaGap;
        Color imgColor = img.color;
        img.color = new Color(imgColor.r, imgColor.g, imgColor.b, oriA);
        img.gameObject.SetActive(true);
        while (count < time && img != null)
        {
            count += Time.deltaTime;
            progress = count / time;
            alphaGap = Mathf.Lerp(oriA, endA, Mathf.Pow(progress, inch));
            img.color = new Color(imgColor.r, imgColor.g, imgColor.b, alphaGap);
            yield return null;
        }
        if (img != null)
            img.color = new Color(imgColor.r, imgColor.g, imgColor.b, endA);
    }

    public void DelayActive(GameObject unActiveGo, bool aimState, float time)
    {
        if (time > 0)
            StartCoroutine(DelayActiveE(unActiveGo, aimState, time));
        else
            unActiveGo.SetActive(aimState);
    }

    IEnumerator DelayActiveE(GameObject unActiveGo, bool aimState, float time)
    {
        yield return new WaitForSeconds(time);
        if (unActiveGo != null)
            unActiveGo.SetActive(aimState);
    }

    public void DelayDestroy(GameObject desObject, float time)
    {
        if (time > 0)
            StartCoroutine(DelayDestroyE(desObject, time));
        else
            Destroy(desObject);
    }

    IEnumerator DelayDestroyE(GameObject desObject, float time)
    {
        yield return new WaitForSeconds(time);
        if (desObject != null)
            Destroy(desObject);
    }

    #endregion
}
