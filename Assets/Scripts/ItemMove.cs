using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
public class ItemMove : MonoBehaviour, IDragHandler,IEndDragHandler
{
    private InventoryGenerator inventoryGenerater;
    private RectTransform _transform;

    private RectTransform inventory;
 
    void Start()
    {
        
        inventoryGenerater = FindObjectOfType<InventoryGenerator>();
        _transform = GetComponent<RectTransform>();

        //인벤토리의 RectTransform 가져옴
        inventory = GameObject.Find("Inventory").GetComponent<RectTransform>();
       
    }
    

    //현재 포지션 += 마우스입력값 / 스케일    
    //크기에 따라 달라지는 이동범위를 평준화함
    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchoredPosition += eventData.delta / inventory.localScale;

    }
   
    // 드래그 종료시 인벤토리에 위치 지정 
    public void OnEndDrag(PointerEventData eventData)
    {
        int x = Mathf.RoundToInt(_transform.anchoredPosition.x / 100);
        int y = Mathf.RoundToInt(_transform.anchoredPosition.y / 100);
        x = Mathf.Clamp(x, 0, inventoryGenerater.x-1);
        y = Mathf.Clamp(y, 0, inventoryGenerater.y-1);

        _transform.anchoredPosition = inventoryGenerater.InventoryTileArray[x, y] ;

        Debug.Log(inventoryGenerater.InventoryTileArray[x, y].ToString()); ;
    }

}