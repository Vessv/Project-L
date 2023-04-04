using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New action", menuName = "BaseAction")]
public class ActionSO : ScriptableObject
{
    public int actionIndex;
    public string actionName;
    public Sprite actionSprite;
    public string actionDescription;
}
