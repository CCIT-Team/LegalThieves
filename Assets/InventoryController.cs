using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] ItemGrid selectedItemGrid;
    // Start is called before the first frame update
    
    // Update is called once per frame
    void Update()
    {
        if (selectedItemGrid==null){return;}
        Debug.Log(selectedItemGrid.GetTileGridPosition(Input.mousePosition));
    }
}
