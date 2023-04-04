using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableImage : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ActionSO ActionSO;
    Transform parentObject;
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentObject = transform.parent;
        transform.SetParent(transform.root.GetChild(0));
        transform.SetAsLastSibling();
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentObject);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        GetComponent<Image>().raycastTarget = true;

    }
}
