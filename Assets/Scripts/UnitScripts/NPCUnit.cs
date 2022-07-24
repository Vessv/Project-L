using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class NPCUnit : BaseUnit
{
    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) return;

        //Test starting positions remember to remove
        if (UnitScriptableObject.UnitFaction == UnitSO.Faction.Demon)
        {
            Debug.Log("Spawneado enemigo");
            SubmitPositionServerRpc(new Vector3(2.5f, 2.5f));
            transform.position = new Vector3(2.5f, 2.5f);
            GameHandler.Instance.GetGrid().GetGridObject(new Vector3(2.5f, 2.5f)).SetUnit(this);
            IsMyTurn.OnValueChanged += GameHandler.Instance.TurnHandler.OnTurnEnd;
            TargetPosition.OnValueChanged += GameHandler.Instance.DoAction;

            return;
        }
    }

    bool IsMeleeRange(int distance)
    {
        if(distance >= 1 && 2 >= distance) return true;
        return false;

    }

    bool isRangedRange(int distance)
    {
        if (distance >= 3 && 6 >= distance) return true;
        return false;

    }

    private void Update()
    {
        if (!IsMyTurn.Value || !IsServer) return;


        if (Input.GetKeyDown(KeyCode.K))
        {

            PlayerUnit targetUnit = GetPlayerWithHighestThreat();

            List<Vector3> pathVectorList = new List<Vector3>();


            //Cambiar esto ponerle el node mas cercano al player, se podría hacer comparando la x e y de las dos unidades
            pathVectorList = Pathfinding.Instance.FindPath(transform.position, targetUnit.transform.position + new Vector3(1f, 0f));

            Debug.Log(pathVectorList);
            if (pathVectorList != null)
            {
                switch (pathVectorList.Count)
                {
                    case 0:
                        Debug.Log("Error findingpath NPCUnit script, path count = 0");
                        break;

                    case int d when (IsMeleeRange(d)):
                        SubmitUnitActionServerRpc(UnitAction.Action.Shoot);
                        TargetPosition.Value = targetUnit.transform.position;
                        Debug.Log("Melee Attack");
                        break;

                    case int d when (isRangedRange(d)):
                        SubmitUnitActionServerRpc(UnitAction.Action.Shoot);
                        TargetPosition.Value = targetUnit.transform.position;
                        Debug.Log("Ranged Attack");
                        break;

                    default:
                        SubmitUnitActionServerRpc(UnitAction.Action.Move);
                        TargetPosition.Value = NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.transform.position + new Vector3(1f, 0f);
                        break;
                }

            }
            else
            {
                Debug.Log("NPC distance between this and target is null");
            }
        }
    }

    PlayerUnit GetPlayerWithHighestThreat()
    {
        List<int> playersThreat = new List<int>();
        playersThreat.Add(0);
        foreach (NetworkClient playerClient in NetworkManager.Singleton.ConnectedClientsList)
        {
            int Threat = playerClient.PlayerObject.GetComponent<PlayerUnit>().Threat;
            playersThreat.Add(Threat);
        }
        int maxThreat = playersThreat.Max();
        int maxIndex = playersThreat.IndexOf(maxThreat);

        return NetworkManager.Singleton.ConnectedClients[(ulong)maxIndex].PlayerObject.GetComponent<PlayerUnit>();
    }

}
