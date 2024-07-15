using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class ItemUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    public PlayerInventory inventory;

    public int inventoryIndex = -1;
    public RectTransform rectTransform { get; private set; }
    bool isSelected = false;
    public Vector3 defaultPosition = Vector3.zero;
    Vector3 clickPosition = Vector3.zero;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        isSelected = true;
        defaultPosition = rectTransform.position;
        clickPosition = Input.mousePosition;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        isSelected = false;
        inventory.SetItemPos(this, rectTransform);
        defaultPosition = rectTransform.position;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected)
            return;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (isSelected)
            return;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        rectTransform.position = (defaultPosition - clickPosition) + Input.mousePosition;
        //Debug.Log(rectTransform.position);
    }

    IEnumerator CheckOverlap()
    {
        while(isSelected)
        {
            
        }
        yield return null;
    }
}
