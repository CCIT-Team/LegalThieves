using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInventory : MonoBehaviour
{
    bool isOpened = false;

    ItemUI[] itemUIs = new ItemUI[10];

    [SerializeField]
    GameObject relicUI;

    [SerializeField]
    GameObject inventoryUI;
    [SerializeField]
    GameObject backGroundUI;
    [SerializeField]
    RectTransform soltTransform;

    [SerializeField]
    RectTransform DumItem;

    [SerializeField]
    int inventorySize = 10;

    private void Awake()
    {
        RectTransform inventoryTransform = inventoryUI.GetComponent<RectTransform>();
        Debug.Log(soltTransform.position);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpened = !isOpened;
        backGroundUI.SetActive(isOpened);
        inventoryUI.SetActive(isOpened);
    }

    public void GetNewItem(int index)
    {
        ToggleInventory();
        ItemUI newItemUI = Instantiate(relicUI).GetComponent<ItemUI>();
        newItemUI.inventory = this;
        SetItemPos(newItemUI, newItemUI.rectTransform);
    }

    void DropItem()
    {

    }

    public void SetItemPos(ItemUI itemUI, RectTransform rectTransform)
    {
        Vector3Int inventoryVector = new Vector3Int((int)((rectTransform.position - soltTransform.position) / 70).x,
                                                    -(int)((rectTransform.position - soltTransform.position) / 70).y,
                                                    (int)((rectTransform.position - soltTransform.position) / 70).z);

        Vector3 itemSize = new Vector3(70 * inventoryVector.x, -70 * inventoryVector.y, 0);
        Debug.Log(inventoryVector);

        if(inventoryVector.x >= 0 && inventoryVector.x < 5 && inventoryVector.y >= 0 && inventoryVector.y < 2)
        {
            if (Array.FindIndex(itemUIs, a => a == itemUI) == -1)
            {
                if (itemUIs[inventoryVector.x + inventoryVector.y * 5] != null)
                {
                    itemUIs[inventoryVector.x + inventoryVector.y * 5].rectTransform.position = itemUI.defaultPosition;   
                }
                rectTransform.position = soltTransform.position + itemSize;
                itemUIs[inventoryVector.x + inventoryVector.y * 5] = itemUI;

            }
            else
            {
                //itemUIs[Array.FindIndex(itemUIs, a => a == itemUI)] = null;
                rectTransform.position = itemUI.defaultPosition;
            }
        }
        else
        {
            DropItem();
            //rectTransform.position = itemUI.defaultPosition;
        }
    }
}
