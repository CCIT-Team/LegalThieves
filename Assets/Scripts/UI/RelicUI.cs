using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class RelicUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IDropHandler
{
    public int inventoryIndex = -1;
    public int relicID = -1;
    public RectTransform rectTransform { get; private set; }
    bool isSelected = false;
    public Vector3 defaultPosition = Vector3.zero;
    Vector3 clickPosition = Vector3.zero;

    public delegate void Change(RelicUI relicUI1, RelicUI relicUI2);
    public Change change;

    public RelicUI(int id)
    {
        relicID = id;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (relicID < 0)
            return;
        isSelected = true;
        defaultPosition = rectTransform.position;
        GetComponent<RawImage>().raycastTarget = false;
        clickPosition = Input.mousePosition;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        isSelected = false;
        GetComponent<RawImage>().raycastTarget = true;
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
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        if (!isSelected && eventData.pointerPress.TryGetComponent<RelicUI>(out RelicUI relicUI))
        {
            Debug.Log(this.gameObject.name);
            Debug.Log(relicUI.gameObject.name);
            change(relicUI,this);
        }
    }

    public void DropRelic()
    {
        rectTransform.position = Vector3.zero;
    }
}
