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
    bool playerInventory = true;

    bool isOpened = false;
    RectTransform rectTransform;

    [SerializeField]
    GameObject RelicUIPrefab;
    [SerializeField]
    RelicUI[] relicUIs = new RelicUI[10];

    [SerializeField]
    GameObject inventoryUI;
    [SerializeField]
    RectTransform sortTransform;

    [SerializeField]
    Vector2Int inventorySize = new Vector2Int(5, 2);

    [SerializeField]
    Vector2 cellSize = new Vector2(110, 110);

    bool isItemSelected = false;


    private void Awake()
    {
        rectTransform = inventoryUI.GetComponent<RectTransform>();
        foreach(var relic in relicUIs)
        {
            relic.change = ChangeIndex;
            relic.inventoryIndex = Array.IndexOf(relicUIs, relic);
        }
        if(playerInventory)
            inventoryUI.SetActive(false);
    }


    private void Update()
    {
        if (playerInventory)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GetNewRelic(5);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleInventory();
            }
        }
    }

    void ToggleInventory()
    {
        isOpened = !isOpened;
        inventoryUI.SetActive(isOpened);
    }

    public void GetNewRelic(int index)
    {
        ToggleInventory();
        RelicUI newRelicUI = Instantiate(RelicUIPrefab).GetComponent<RelicUI>();
        newRelicUI.transform.parent = this.transform;
    }

    void ChangeIndex(RelicUI relicUI1, RelicUI relicUI2)
    {
        int _index = relicUI2.inventoryIndex;
        relicUI2.inventoryIndex = relicUI1.inventoryIndex;
        relicUI1.inventoryIndex = _index;
        Debug.Log(relicUI1.inventoryIndex + ", " + relicUI2.inventoryIndex + ", " + _index);

        if (relicUI2.inventoryIndex == -1)
        {
            relicUI2.DropRelic();
        }
        else
        {
            SetRelicIndex(relicUI2, relicUI2.rectTransform);
        }
        SetRelicIndex(relicUI1, relicUI1.rectTransform);
    }

    public void SetRelicIndex(RelicUI relicUI, RectTransform rectTransform)
    {
        Debug.Log(gameObject.name);
        relicUI.transform.parent = sortTransform;
        rectTransform.position = new Vector3(sortTransform.position.x + (relicUI.inventoryIndex % inventorySize.x) * cellSize.x, sortTransform.position.y - Mathf.Floor(relicUI.inventoryIndex / inventorySize.x) * cellSize.y, 0);
        Debug.Log(relicUI.gameObject.name + ", " + (relicUI.inventoryIndex % inventorySize.x) + ", " + Mathf.Floor(relicUI.inventoryIndex / inventorySize.x));
        relicUIs[relicUI.inventoryIndex] = relicUI;
    }

    public void SetRelicPos(RelicUI relicUI, RectTransform rectTransform)
    {
        Vector3Int inventoryVector = new Vector3Int((int)((rectTransform.position - sortTransform.position) / cellSize.x).x,
                                                   -(int)((rectTransform.position - sortTransform.position) / cellSize.y).y,
                                                    0);

        Vector3 relicSize = new Vector3(cellSize.x * inventoryVector.x, -cellSize.y * inventoryVector.y, 0);
        Debug.Log(inventoryVector);

        if (inventoryVector.x >= 0 && inventoryVector.x < 5 && inventoryVector.y >= 0 && inventoryVector.y < 2)
        {
            if (Array.FindIndex(relicUIs, a => a == relicUI) == -1)
            {
                if (relicUIs[inventoryVector.x + inventoryVector.y * 5] != null)
                {
                    relicUIs[inventoryVector.x + inventoryVector.y * 5].rectTransform.position = relicUI.defaultPosition;
                }
                relicUI.transform.SetParent(sortTransform, true);
                rectTransform.position = sortTransform.position + relicSize;
                relicUIs[inventoryVector.x + inventoryVector.y * 5] = relicUI;
            }
            else
            {
                //relicUIs[Array.FindIndex(relicUIs, a => a == relicUI)] = null;
                //relicUIs[inventoryVector.x + inventoryVector.y * 5] = relicUI;
                //rectTransform.position = sortTransform.position + relicSize;
                rectTransform.position = relicUI.defaultPosition;
            }
        }
    }
}
