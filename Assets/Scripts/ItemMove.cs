using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
public class ItemMove : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private InventoryGenerator inventoryGenerater;
    private RectTransform _transform;
    private Canvas _canvas;
    

    void Start()
    {
        
        inventoryGenerater = FindObjectOfType<InventoryGenerator>();
        _transform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        int x, y;
 

x =Mathf.RoundToInt(_transform.position.x)/100;

y = Mathf.RoundToInt(_transform.position.y)/100;
_transform.position =   new Vector3(x * inventoryGenerater.cellSize+60, y * inventoryGenerater.cellSize+40,0);

    }
}