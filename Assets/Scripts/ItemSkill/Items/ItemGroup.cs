using System.Collections;
using System.Collections.Generic;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class ItemGroup : MonoBehaviour
{
    [SerializeField] Transform LocalItemGroup;
    [SerializeField] ItemBase[] PlayerItems;
    [SerializeField] Transform NavigationRenderer;
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
        SetNavigationRenderer();
    }
    public void HideLocalItem()
    {
        Transform[] childTransforms = LocalItemGroup.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in childTransforms)
        {
            child.gameObject.layer = 8;
        }
        SetNavigationRenderer();
        NavigationRenderer.gameObject.layer = 8;
    }

    public void SetNavigationRenderer(){
        NavigationRenderer.parent=null;
        
       NavigationRenderer.localRotation  = Quaternion.Euler(90,0,0);
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
