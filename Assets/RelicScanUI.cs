using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using New_Neo_LT.Scripts.Relic;
using LegalThieves;

public class RelicScanUI : MonoBehaviour
{
    [SerializeField] GameObject relicScanUIGroup;
   
    [SerializeField] TMP_Text relicName;
    [SerializeField] TMP_Text goldPoint;
    [SerializeField] TMP_Text renownPoint;

  
    //public void OnScanUI()
    //{
    //    relicScanUIGroup.SetActive(true);
    //}
    //public void OffScanUI()
    //{
    //    relicScanUIGroup.SetActive(false);
    //}

    public void SetUIPoint(int relicIndex)
    {

        if (relicIndex == -1)
        {
            relicName.text = "";
            goldPoint.text = "";
            renownPoint.text = "";
            relicScanUIGroup.gameObject.SetActive(false);
        }
        else
        {
            relicScanUIGroup.gameObject.SetActive(true);
            var relic = RelicManager.Instance.GetRelicData(relicIndex);
            relicName.text = RelicManager.Instance.GetRelicName(relic.GetRelicType(), relic.GetTypeIndex());
            goldPoint.text = relic.GetGoldPoint().ToString();
            renownPoint.text = relic.GetRenownPoint().ToString();
        }
    }
}
