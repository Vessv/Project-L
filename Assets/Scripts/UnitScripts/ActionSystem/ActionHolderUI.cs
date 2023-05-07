using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionHolderUI : MonoBehaviour
{
    public GameObject DraggableActionObject;
    public Image ActionSprite;
    public Image ActionDragSprite;
    public TMP_Text ActionDescription;
    public TMP_Text Cost;
    public ActionSO ActionSO;
    // Start is called before the first frame update
    void OnEnable()
    {
        ActionSprite.sprite = ActionSO.actionSprite;
        ActionDescription.text = ActionSO.actionDescription;
        Cost.text = "AP: "+ActionSO.pointCost;
        ActionDragSprite.sprite = ActionSO.actionSprite;
    }
}
