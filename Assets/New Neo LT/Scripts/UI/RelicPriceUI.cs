using UnityEngine;
using TMPro;
using New_Neo_LT.Scripts.Relic;
using LegalThieves;

public class RelicPriceUI : MonoBehaviour
{
    [SerializeField]
    GameObject goldPointLabel;
    [SerializeField]
    GameObject renownPointLabel;

    TMP_Text relicName;
    TMP_Text goldPoint;
    TMP_Text renownPoint;

    bool isWinGold = false;

    private void Awake()
    {
        //goldPoint = goldPointLabel.transform.GetChild(0).GetComponent<TMP_Text>();
        //renownPoint = renownPointLabel.transform.GetChild(0).GetComponent<TMP_Text>();
        relicName = GetComponent<TMP_Text>();
        goldPoint = goldPointLabel.GetComponent<TMP_Text>();
        renownPoint = renownPointLabel.GetComponent<TMP_Text>();
    }

    public void SetWinPoint(bool iswin = false)
    {
        isWinGold = iswin;
        if (!isWinGold)
            goldPointLabel.transform.SetSiblingIndex(0);
        else
            goldPointLabel.transform.SetSiblingIndex(1);
    }

    public void SetUIPoint(int relicIndex)
    {
        if(relicIndex == -1)
        {
            relicName.text = "";
            goldPoint.text = "";
            renownPoint.text = "";
        }
        else
        {
            var relic = RelicManager.Instance.GetRelicData(relicIndex);
            relicName.text = RelicManager.Instance.GetRelicName(relic.GetTypeIndex());
            goldPoint.text = "골드 포인트   " + relic.GetGoldPoint().ToString();
            renownPoint.text = "리나운 포인트   " + relic.GetRenownPoint().ToString();
        }
    }
}
