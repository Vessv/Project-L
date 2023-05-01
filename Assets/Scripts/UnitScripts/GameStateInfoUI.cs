using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStateInfoUI : MonoBehaviour
{
    public Slider hpSlider;
    public TextMeshProUGUI myTurn;
    public TextMeshProUGUI myActionPoints;
    public TextMeshProUGUI currentFloor;
    // Start is called before the first frame update
    void OnEnable()
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        GameObject playerObject = transform.root.gameObject;
        PlayerUnit unit = playerObject.GetComponent<PlayerUnit>();

        if (unit.IsMyTurn.Value)
        {
            myTurn.text = "My turn";
            myTurn.color = new Color(0.65f, 0.98f, 0.51f); // A6FA83
        } else
        {
            myTurn.text = "Not my turn";
            myTurn.color = new Color(0.98f, 0.51f, 0.51f);  

        }

        hpSlider.maxValue = unit.Stats.Value.Vitality;
        hpSlider.value = unit.CurrentHealth.Value;

        myActionPoints.text = "Action points: "  + unit.ActionPoints.Value;

        currentFloor.text = "Current floor: 0";

    }
}
