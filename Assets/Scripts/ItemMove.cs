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
    public RectTransform parent;

    void Start()
    {
        
        inventoryGenerater = FindObjectOfType<InventoryGenerator>();
        _transform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        parent = GetComponentInParent<RectTransform>();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    
    int x =Mathf.RoundToInt(_transform.position.x/10); 
    int y = Mathf.RoundToInt(_transform.position.y/10); 
    x = Mathf.Clamp(x, 0, inventoryGenerater.x -1);
    y = Mathf.Clamp(y, 0, inventoryGenerater.y-1);
        Debug.Log(x + " " + y);
    _transform.anchoredPosition = new Vector2(x * inventoryGenerater.cellSize, y * inventoryGenerater.cellSize);

    }
}