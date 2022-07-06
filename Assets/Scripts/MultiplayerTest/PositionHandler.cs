using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PositionHandler : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move()
    {

    }

    [ServerRpc]
    void PositionChangeServerRpc()
    {

    }
}
