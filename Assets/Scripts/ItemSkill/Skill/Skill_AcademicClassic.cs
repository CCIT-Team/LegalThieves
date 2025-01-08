
using System.Collections.Generic;
using New_Neo_LT.Scripts.Relic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.Universal;
using System.Collections;
public class Skill_AcademicClassic : HC_Skill
{ 
    [SerializeField] private int radius;
    [SerializeField] LayerMask detectionLayer;
    private string relicHighlightLayer = "Skill_RelicHighlight";
    private string relicTag = "Relics";
    [SerializeField] float effectTime = 5;
     [SerializeField] private UniversalRendererData SkillAcademicClasic;

    List<Collider> topThreeRelics;
    public override void UseSkill(){}
    public override void Init(){
  
    }
    private void Start() {
              SkillAcademicClasic.rendererFeatures[3].SetActive(false);
    }
    public void ScanExpensiveRelic(){
        if(!CanUse) return;

        canUse=false;
            topThreeRelics = GetTopThreeRelics();

            Debug.Log(topThreeRelics.Count);
            foreach (var relic in topThreeRelics)
            {
              relic.gameObject.layer = LayerMask.NameToLayer(relicHighlightLayer);
                Debug.Log($"Top Relic: {relic.name}");
            }
        StartCoroutine( PlayerScan());

   }
      
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("스킬!");
        ScanExpensiveRelic();
     
        }
    }

    List<Collider> GetTopThreeRelics()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, detectionLayer);

    
        List<Collider> relics = hitColliders
            .Where(collider => collider.CompareTag(relicTag))
            .ToList();
     

       
        List<Collider> topThreeRelics = relics
            .OrderByDescending(collider => collider.GetComponent<RelicObject>().GetRenownPoint()) 
            .Take(3) 
            .ToList();

        return topThreeRelics;
    }


    private IEnumerator PlayerScan()
    {
     
        SkillAcademicClasic.rendererFeatures[3].SetActive(true);
        yield return new WaitForSeconds(effectTime);
  
        SkillAcademicClasic.rendererFeatures[3].SetActive(false);

          foreach (var relic in topThreeRelics)
            {
                relic.gameObject.layer= 7;
            }
             canUse=true;
    }

}

    
 

