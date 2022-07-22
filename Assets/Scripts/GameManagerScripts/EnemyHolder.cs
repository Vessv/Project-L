using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyHolder : NetworkBehaviour
{
    public List<BaseUnit> EnemyList = new();

    public void InstantiateEnemies(BaseUnit enemy)
    {
        EnemyList.Add(enemy);   
    }
}
