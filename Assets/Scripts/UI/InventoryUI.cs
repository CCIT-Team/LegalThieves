using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    RectTransform RectTransform;
    [SerializeField]
    Vector2Int inventorySize = new Vector2Int(5,2);

    bool Toggleinventory()
    {
        return false;
    }

    void SetUI()
    {

    }
}
