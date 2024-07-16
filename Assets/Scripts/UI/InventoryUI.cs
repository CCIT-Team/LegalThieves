using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using LegalThieves;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    GameObject relicUIPrefab;

    [SerializeField]
    GameObject inventory;
    RectTransform inventoryRectTransform;

    [SerializeField]
    Vector2Int inventorySize = new Vector2Int(5,2);

    [SerializeField]
    RelicUI[] relics = new RelicUI[10];

    int selectedID = -1;

    private void Awake()
    {
        inventoryRectTransform = inventory.GetComponent<RectTransform>();
        for (int i = 0; i < 10; i++)
        {
            relics[i].inventoryIndex = i;
            relics[i].swap = SwapID;
            relics[i].select = SelectID;
        }
    }

    bool Toggleinventory()
    {
        return false;
    }

    public void SetUI()
    {

    }

    public void GetRelic(int ID)
    {
        Instantiate(relicUIPrefab, inventory.transform.GetChild(0)).GetComponent<RelicUI>().relicID = ID;
    }

    public void SetItem(int ID, bool isAdd)
    {
        if (isAdd)
            AddItem(ID);
        else
            RemoveItem(ID);
    }

    void AddItem(int ID)
    {
        int index = Array.FindIndex(relics, a => a.relicID == -1);
        if (index < 0)
            return;
        relics[index].relicID = ID;
    }
    void RemoveItem(int ID)
    {
        int index = Array.FindIndex(relics, a => a.relicID == ID);
        if (index < 0)
            return;
        relics[index].relicID = -1;
    }

    void SwapID(RelicUI relicUI1, RelicUI relicUI2)
    {
        int ID = relicUI1.relicID;
        relicUI1.relicID = relicUI2.relicID;
        relicUI2.relicID = ID;
    }

    void SelectID(int i)
    {
        selectedID = i;
    }
}
