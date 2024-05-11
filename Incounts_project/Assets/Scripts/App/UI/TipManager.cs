using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    [Header("UIElements")]
    public GameObject tipPrefab;
    public RectTransform tipPanel;

    public static TipManager Instance;

    private Queue<string> tipsString = new Queue<string>();
    private Stack<GameObject> tipsPool = new Stack<GameObject>();
    private List<GameObject> tipsShowing = new List<GameObject>();

    private float tipsShowGap = 0.3f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void AddTipToShow(string tipInfo)
    {
        tipsString.Enqueue(tipInfo);
        StartShowTips();
    }

    private void StartShowTips()
    {
        if (tipsShowing.Count <= 0)
        {
            StartCoroutine(DelayShowTips());
        }
    }

    IEnumerator DelayShowTips()
    {
        while (tipsString.Count > 0)
        {
            ShowTip(tipsString.Dequeue());
            yield return new WaitForSeconds(tipsShowGap);
        }
    }

    private void ShowTip(string tipInfo)
    {
        GameObject tipGo = GetFromPoolOrDefault(tipsPool, tipPrefab);
        tipGo.transform.SetParent(tipPanel);
        tipGo.GetComponent<TipItem>().ShowTip(tipInfo);
        tipGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        MoveUpShowingTips();
        tipsShowing.Add(tipGo);
        StartCoroutine(DelayRecycle(tipGo));
    }

    private void MoveUpShowingTips()
    {
        for (int i = 0; i < tipsShowing.Count; i++)
        {
            RectTransform rect = tipsShowing[i].GetComponent<RectTransform>();
            AnimManager.instance.GentleMove(rect, rect.anchoredPosition, rect.anchoredPosition + new Vector2(0, 80), 0.2f, 1);
        }
    }

    IEnumerator DelayRecycle(GameObject go)
    {
        yield return new WaitForSeconds(1.5f);
        RecycleObject(go);
    }

    #region Object pool
    void RecycleObject(GameObject go)
    {
        go.SetActive(false);
        tipsShowing.Remove(go);
        tipsPool.Push(go);
    }

    GameObject GetFromPoolOrDefault(Stack<GameObject> pool, GameObject defaultGo)
    {
        if (pool.Count > 0)
        {
            return pool.Pop();
        }
        return Instantiate(defaultGo, defaultGo.transform.parent);
    }
    #endregion
}