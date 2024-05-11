using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipItem : MonoBehaviour
{
    public Animator anim;
    public Text text;

    public void ShowTip(string tipInfo)
    {
        text.text = tipInfo;
        this.gameObject.SetActive(true);
        anim.Play("tipsAnim");
    }
}
