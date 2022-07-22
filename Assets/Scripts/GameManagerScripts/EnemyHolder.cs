using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyHolder : NetworkBehaviour
{
    public List<Unit> EnemyList = new();

    public void InstantiateEnemies(Unit enemy)
    {
        EnemyList.Add(enemy);   
    }
}
