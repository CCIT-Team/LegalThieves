using System.Collections;
using System.Collections.Generic;
using LegalThieves;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class ItemGroup : MonoBehaviour
{
    [SerializeField] Transform LocalItemGroup;
    [SerializeField] ItemBase[] PlayerItems;
    [SerializeField] Transform CamObject;
    [SerializeField] GameObject LightObject;
    void Start()
    {
        PlayerItems = GetComponentsInChildren<ItemBase>();
    }
    public void HideMultiItem()
    {
        foreach (var item in PlayerItems)
        {
            Transform[] childTransforms = item.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in childTransforms)
            {
                child.gameObject.layer = 8;
            }
        }
        SetCamObject();
    }
    public void HideLocalItem()
    {
        Transform[] childTransforms = LocalItemGroup.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in childTransforms)
        {
            child.gameObject.layer = 8;
        }

        var temp = CamObject.GetComponentsInChildren<Transform>();
        foreach(var g in temp){
            g.gameObject.SetActive(false);
        }
     
    }

    public void SetCamObject()
    {
        CamObject.parent = CameraFollow.Singleton.Target;
        CamObject.localPosition = Vector3.zero;      
        CamObject.localRotation = Quaternion.identity; 
        transform.localScale = Vector3.one;   
    }
    public void SetLocalItemGroup()
    {
        LocalItemGroup.transform.parent = PlayerCharacter.Local.GetLocalItemHolder();
        LocalItemGroup.transform.localPosition = Vector3.zero;
        LocalItemGroup.transform.localRotation = Quaternion.identity;

    }


    public ItemBase GetItemClass(int itemIndex)
    {
        return PlayerItems[itemIndex];
    }

}
