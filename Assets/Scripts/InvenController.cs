using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InvenController : MonoBehaviour {
    

    // Start is called before the first frame update
    [SerializeField] private Transform itemList;
    [SerializeField] private Transform itemObject;
     private void Update() {
        if(Input.GetKeyDown(KeyCode.T)){
            CreateItem();
            
        }
     }

    void CreateItem(){
        Instantiate(itemObject,itemList.position,Quaternion.identity,itemList);
    }
}
