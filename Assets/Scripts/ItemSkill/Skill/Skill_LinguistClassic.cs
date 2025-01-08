
using System.Collections.Generic;
using New_Neo_LT.Scripts.Relic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.Universal;
using System.Collections;
using LegalThieves;
using New_Neo_LT.Scripts.PlayerComponent;
using Unity.VisualScripting;
public class Skill_LinguistClassic : HC_Skill
{
    private string relicHighlightLayer = "Skill_RelicHighlightAlways";
  
    [SerializeField] float effectTime = 5;
    [SerializeField] private UniversalRendererData SkillAcademicClasic;
    List<RelicObject> topThreeRelics;
   
    public override void UseSkill() { }
    public override void Init()
    {

    }
    private void Start()
    {
        SkillAcademicClasic.rendererFeatures[4].SetActive(false);
    }
     void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("스킬!");
            ScanSameRelic();

        }
    }

    public void ScanSameRelic()
    {
        if(!CanUse) return;
        canUse=false;
        SearchTopThreeRelics();
        if (topThreeRelics==null) return;

        Debug.Log(topThreeRelics.Count);
        foreach (var relic in topThreeRelics)
        {
            relic.gameObject.layer = LayerMask.NameToLayer(relicHighlightLayer);

        }
        StartCoroutine(PlayerScan());

    }

   
    void SearchTopThreeRelics()
    {
        var slotRelic = PlayerCharacter.Local.GetSlotRelic();
        if (slotRelic==null) return ;

        var targetIndex = slotRelic.GetTypeIndex();
        var targetType = slotRelic.GetRelicType();

        var Relics = RelicManager.Instance.GetRelicList();

        var RelicScripts = new List<RelicObject>();

        foreach(var relic in Relics ){
           RelicScripts.Add (relic.GetComponent<RelicObject>());
        }

        topThreeRelics  = RelicScripts
            .Where(relic => relic.GetTypeIndex() == targetIndex && relic.GetRelicType() == targetType)
            .Where(relic => relic.GetIsActivated == true)
            .OrderBy(relic => Vector3.Distance(transform.position, relic.transform.position))
            .Take(3)
            .ToList();

    }


    private IEnumerator PlayerScan()
    {

        SkillAcademicClasic.rendererFeatures[4].SetActive(true);
        yield return new WaitForSeconds(effectTime);

        SkillAcademicClasic.rendererFeatures[4].SetActive(false);

        foreach (var relic in topThreeRelics)
        {
            relic.gameObject.layer = 7;
            Debug.Log($"Top Relic: {relic.name}");
        }
        canUse =true;
    }
}
