using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCUnit : BaseUnit
{
    // Start is called before the first frame update
    void Start()
    {
        //Test starting positions remember to remove
        if (UnitScriptableObject.UnitFaction == UnitSO.Faction.Demon && IsOwnedByServer)
        {
            Debug.Log("Spawneado");
            SubmitPositionServerRpc(new Vector3(2.5f, 2.5f));
            transform.position = new Vector3(2.5f, 2.5f);
            //GameHandler.Instance.GetGrid().GetGridObject(new Vector3(2.5f, 2.5f)).SetUnit(this);
            return;
        }
    }

}
