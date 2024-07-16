using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RelicUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
    public int inventoryIndex = -1;
    public int relicID = -1;
    public RectTransform rectTransform { get; private set; }
    bool isSelected = false;
    public Vector3 defaultPosition = Vector3.zero;
    private Vector3 clickPosition = Vector3.zero;

    public delegate void Swap(RelicUI relicUI1, RelicUI relicUI2);
    public Swap swap;
    public delegate void Select(int ID);
    public Select select;

    public RelicUI(int id)
    {
        relicID = id;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultPosition = rectTransform.position;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (relicID < 0)
            return;
        isSelected = true;
        select(relicID);
        GetComponent<Image>().raycastTarget = false;
        clickPosition = Input.mousePosition;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        isSelected = false;
        GetComponent<Image>().raycastTarget = true;
        rectTransform.position = defaultPosition;
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
            swap(this,relicUI);
        }
    }

    public void DropRelic()
    {
        rectTransform.position = Vector3.zero;
    }
}
