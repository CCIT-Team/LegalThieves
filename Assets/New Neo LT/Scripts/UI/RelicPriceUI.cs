using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TMPro;

public class RelicPriceUI : MonoBehaviour
{
    [SerializeField]
    GameObject goldPointLabel;
    [SerializeField]
    GameObject renownPointLabel;

    TMP_Text goldPoint;
    TMP_Text renownPoint;

    bool isWinGold = false;

    private void Awake()
    {
        goldPoint = goldPointLabel.transform.GetChild(0).GetComponent<TMP_Text>();
        renownPoint = renownPointLabel.transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void SetWinPoint(bool iswin = false)
    {
        isWinGold = iswin;
        if (isWinGold)
            goldPointLabel.transform.SetSiblingIndex(0);
        else
            goldPointLabel.transform.SetSiblingIndex(1);
    }

    public void SetUIPoint(int gold, int renown)
    {
        goldPoint.text = gold.ToString();
        renownPoint.text = renown.ToString();
    }
}
