using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class UnitCombatGrid : NetworkBehaviour
{
    public UnitSO unitSO;
    Movement movement;

    public HealthSystem healthSystem;

    public int strenght;
    public int vitality;
    public int agility;

    //healthSystem.OnDamaged += TestfucnOnDamage;


    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        healthSystem = new HealthSystem(unitSO.vitality * 10);
        //unitSO.state = UnitSO.State.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) /*&& unitSO.state == UnitSO.State.Normal*/)
        {
            //instantiateSprite();
            MoveTo(Utils.GetMouseWorldPosition());
        }
        /*if(unitSO.state == UnitSO.State.Moving)
        {
            movement.HandleMovement();
        }*/
        if (Input.GetMouseButtonDown(1))
        {
            //GameHandler_GridCombatSystem.instance.GetGrid().GetGridObject(transform.position).SetUnit(this);
            if (transform.position.x < 1)
            {
                //Attack(GameHandler_GridCombatSystem.instance.GetGrid().GetGridObject(Utils.GetMouseWorldPosition()).GetUnit());
            }
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        /*unitSO.state = UnitSO.State.Moving;
        movement.SetTargetPosition(targetPosition, 
        () => {
            unitSO.state = UnitSO.State.Normal;
        }, 
        () => {
            unitSO.state = UnitSO.State.Normal;
            Debug.Log("Not Reachable");
        });*/
    }

    public void Attack(UnitCombatGrid targetUnit)
    {
        /*if (targetUnit == null || targetUnit == this) return;
        unitSO.state = UnitSO.State.Attacking;
        GameHandler_GridCombatSystem.instance.GetGrid().GetGridObject(targetUnit.GetPosition()).GetUnit().GetHealthSystem().Damage(1);
        Debug.Log("attacked unit hp at:" + targetUnit.GetHealthSystem().GetHealth());*/
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public HealthSystem GetHealthSystem()
    {
        return healthSystem;
    }
}
