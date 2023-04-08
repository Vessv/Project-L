using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipPopUp : MonoBehaviour
{
    public GameObject popupCanvasObject;
    public Canvas popupCanvas;
    public RectTransform popupObject;
    public TextMeshProUGUI infoText;
    public float padding;
    public Vector3 offset;

    private void Update()
    {
        FollowCursor();
    }

    private void FollowCursor()
    {
        if (!popupCanvasObject.activeSelf) { return; }

        Vector3 newPos = Input.mousePosition + offset;
        newPos.z = 0f;
        float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + popupObject.rect.width * popupCanvas.scaleFactor / 2) - padding;
        if (rightEdgeToScreenEdgeDistance < 0)
        {
            newPos.x += rightEdgeToScreenEdgeDistance;
        }
        float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - popupObject.rect.width * popupCanvas.scaleFactor / 2) + padding;
        if (leftEdgeToScreenEdgeDistance > 0)
        {
            newPos.x += leftEdgeToScreenEdgeDistance;
        }
        float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + popupObject.rect.height * popupCanvas.scaleFactor) - padding;
        if (topEdgeToScreenEdgeDistance < 0)
        {
            newPos.y += topEdgeToScreenEdgeDistance;
        }
        popupObject.transform.position = newPos;
    }
    public void DisplayInfo(Item item)
    {
        if (item == null)
        {
            return;
        }
        StringBuilder builder = new StringBuilder();
        builder.Append("<size=35>").Append(item.ColouredName()).Append("</size>").AppendLine();
        builder.Append(item.GetTooltipInfoText());
        infoText.text = builder.ToString();
        popupCanvasObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
        popupCanvasObject.GetComponentInParent<Canvas>().sortingOrder = 1;
    }
    public void HideInfo()
    {
        popupCanvasObject.SetActive(false);
        popupCanvasObject.GetComponentInParent<Canvas>().sortingOrder = 0;
        //Debug.Log("desactivar" + popupCanvasObject.activeInHierarchy);
    }
}
