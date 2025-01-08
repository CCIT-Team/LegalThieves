using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using New_Neo_LT.Scripts;
using New_Neo_LT.Scripts.UI;
using System;
using New_Neo_LT.Scripts.Game_Play;
using Unity.VisualScripting;
using LegalThieves;
using New_Neo_LT.Scripts.Relic;

public class Skill_ShamanistDefault : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform[] targets;
    [SerializeField] private GameObject fireSoulPrefab;
    [SerializeField] private GameObject GlowWhitePrefab;
    [SerializeField] private Transform objectTransform;

    private Camera mainCamera;
    private List<Transform> fireSoulVFXList;
    private List<Transform> renderingTargetList;

    public bool isActive = false;
    int type = 0;

    private void Start()
    {
        if(objectTransform == null)
            objectTransform = UIManager.Instance.transform.Find("SkillEffectUI");
        mainCamera = Camera.main;
        renderingTargetList = new List<Transform>();
        fireSoulVFXList = new List<Transform>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            isActive = false;
            foreach (var transform in fireSoulVFXList)
                Destroy(transform.gameObject);
        }

        if(isActive)
            SetRenderingTargetList();

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            isActive = true;
            StopCoroutine(nameof(InstantiateFireSoulVFX));
            targets = GetCharactersTransform(0);

            StartCoroutine(InstantiateFireSoulVFX(true));
        }

        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            isActive = true;
            StopCoroutine(nameof(InstantiateFireSoulVFX));
            targets = GetCharactersTransform(1);

            StartCoroutine(InstantiateFireSoulVFX());
        }

        
    }

    private void SetRenderingTargetList()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        renderingTargetList = new List<Transform>();

        foreach (var transform in targets)
        {
            Collider transformColider = transform.GetComponent<Collider>();
            if (GeometryUtility.TestPlanesAABB(planes, transformColider.bounds))
            {
                renderingTargetList.Add(transform);
            }
        }
    }

    private Transform[] GetCharactersTransform(int i = 0)
    {
        List<Transform> transformList = new List<Transform>();

        type = i;

        switch (i)
        {
            default:
                return null;
            case 0:
                if (PlayerRegistry.Instance != null)
                {
                    var players = PlayerRegistry.GetAllPlayers();

                    foreach (var player in players)
                    {
                        transformList.Add(player.transform);
                    }
                }

                var monsters = FindObjectsOfType(typeof(Monster));

                foreach (var monster in monsters)
                {
                    transformList.Add(monster.GameObject().transform);
                }
                break;
            case 1:
                var relics = RelicManager.Instance.transform.GetChild(0).GetComponentsInChildren<RelicObject>();

                foreach (var relic in relics)
                {
                    if(relic.GetComponent<Collider>().enabled)
                        transformList.Add(relic.transform);
                }
                break;
        }
        return transformList.ToArray();
    }

    private IEnumerator InstantiateFireSoulVFX(bool isDelay = false)
    {
        foreach (var transform in fireSoulVFXList)
            Destroy(transform.gameObject);
        fireSoulVFXList.Clear();
        listA.Clear();

        while(isActive)
        {
            if(renderingTargetList.Count != 0)
            {
                var firstTarget = renderingTargetList[0];
                foreach (var target in renderingTargetList)
                {
                    if (listA.Contains(target))
                    {
                        var effect = fireSoulVFXList[listA.IndexOf(target)];
                        fireSoulVFXList.RemoveAt(listA.IndexOf(target));
                        listA.Remove(target);
                        effect.position = mainCamera.WorldToScreenPoint(target.position);
                        fireSoulVFXList.Add(effect);
                        listA.Add(target);
                    }
                    else
                    {
                        Transform fireSoulVFX;
                        if (type == 0)
                            fireSoulVFX = Instantiate(fireSoulPrefab, mainCamera.WorldToScreenPoint(target.position), Quaternion.identity, objectTransform).transform;
                        else
                            fireSoulVFX = Instantiate(GlowWhitePrefab, mainCamera.WorldToScreenPoint(target.position), Quaternion.identity, objectTransform).transform;

                        fireSoulVFXList.Add(fireSoulVFX);
                        listA.Add(target);
                        if (isDelay)
                            yield return new WaitForSeconds(0.15f);
                    }
                }
                int removeTarget = listA.IndexOf(firstTarget);
                for (int i = 0; i < removeTarget; i++)
                {
                    Destroy(fireSoulVFXList[0].gameObject);
                    fireSoulVFXList.RemoveAt(0);
                    listA.RemoveAt(0);
                }
            }
            yield return null;
        }
    }


    List<Transform> listA = new();
    //List<Transform> listB = new();
    //List<Transform> listC = new();


    //IEnumerator AAA(Type type)
    //{
    //    var a = FindObjectsOfType(type);

    //    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

    //    foreach (var obj in a)
    //    {
    //        Collider transformColider = obj.GetComponent<Collider>();
    //        if (GeometryUtility.TestPlanesAABB(planes, transformColider.bounds))
    //        {
    //            if (listC.Contains(transformColider.transform))
    //                listA.Add(transformColider.transform);
    //            else
    //                listB.Add(transformColider.transform);
    //        }
    //    }

    //    foreach (var obj in listA)
    //    {
            
    //    }

    //    foreach(var obj in listB)
    //    {

    //    }
    //    yield return null;
    //}
}
