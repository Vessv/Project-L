using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionHolderUI : MonoBehaviour
{
    public Image ActionSprite;
    public TMP_Text ActionDescription;
    public ActionSO ActionSO;
    // Start is called before the first frame update
    void OnEnable()
    {
        ActionSprite.sprite = ActionSO.actionSprite;
        ActionDescription.text = ActionSO.actionDescription;
    }
}
