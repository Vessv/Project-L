using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NPCHpBar : NetworkBehaviour
{
    public NPCUnit unit;
    public Slider hpSlider;

    private void Start()
    {
        unit = transform.root.GetComponent<NPCUnit>();
        hpSlider.maxValue = unit.Stats.Value.Vitality;
        hpSlider.value = unit.CurrentHealth.Value;
        unit.CurrentHealth.OnValueChanged += OnHealthChanged;
    }

    void OnHealthChanged(int old, int newValue){
        hpSlider.maxValue = unit.Stats.Value.Vitality;
        hpSlider.value = unit.CurrentHealth.Value;
    }

    [ClientRpc]
    void UpdateHealthBarClientRpc()
    {
        hpSlider.maxValue = unit.Stats.Value.Vitality;
        hpSlider.value = unit.CurrentHealth.Value;
    }
}
