using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InvenController : MonoBehaviour {
    

    // Start is called before the first frame update
    [SerializeField] private RectTransform itemList;
    [SerializeField] private RectTransform itemObject;
     private void Update() {
        if(Input.GetKeyDown(KeyCode.T)){
            CreateItem();
            
        }
     }

    void CreateItem(){
        Instantiate(itemObject,itemList.anchoredPosition ,Quaternion.identity,itemList);
    }
}
