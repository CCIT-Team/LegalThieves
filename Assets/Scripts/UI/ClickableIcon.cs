using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ClickableIcon : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Down");
        if(transform.parent.parent.TryGetComponent<SellingUITest>( out var sellui))
        {
            Debug.Log("Pass");
            if (transform.parent.gameObject == sellui.inventoryGrid)
                sellui.SetSelectedRellicToGrid(gameObject, sellui.sellingTableGrid); 
            else
                sellui.SetSelectedRellicToGrid(gameObject, sellui.inventoryGrid);
        }
    }
}
