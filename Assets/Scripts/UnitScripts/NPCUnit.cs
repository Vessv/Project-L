using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class NPCUnit : BaseUnit
{
    PlayerUnit _targetUnit;
    List<Vector3> _pathVectorList;
    Vector3 _targetUnitPosition;

    bool IsTargetLeft => transform.position.x > _targetUnit.transform.position.x; 
    bool IsTargetRight => _targetUnit.transform.position.x > transform.position.x;
    bool IsTargetUp => _targetUnit.transform.position.y > transform.position.y;
    bool IsTargetDown => transform.position.y > _targetUnit.transform.position.y;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) return;

        //Test starting positions remember to remove
        if (UnitScriptableObject.UnitFaction == UnitSO.Faction.Demon)
        {
            Debug.Log("Spawneado enemigo");
            transform.position = new Vector3(2.5f, 2.5f);
            GameHandler.Instance.GetGrid().GetGridObject(new Vector3(2.5f, 2.5f)).SetUnit(this);
            IsMyTurn.OnValueChanged += GameHandler.Instance.TurnHandler.OnIsMyTurnValueChanged;
            ActionPoints.OnValueChanged += GameHandler.Instance.TurnHandler.OnUnitActionPointsChanged;
            return;
        }
    }

    bool IsMeleeRange(int distance)
    {
        if(distance >= 0 && 1 >= distance) return true;
        return false;

    }

    bool isRangedRange(int distance)
    {
        if (distance >= 1 && 2 >= distance) return true;
        return false;

    }


    private void Update()
    {
        if (!IsMyTurn.Value || !IsServer || IsBusy) return;
        Debug.Log("doing ai cosas");
        _targetUnit = GetPlayerWithHighestThreat();

        _pathVectorList = new List<Vector3>();

        _targetUnitPosition = _targetUnit.transform.position + GetOffSetVector();

        PathNode targetNode = Pathfinding.Instance.GetGrid().GetGridObject(_targetUnitPosition);

        if (!targetNode.isWalkable)
        {
            SetWalkableTargetPosition();
        }

        _pathVectorList = Pathfinding.Instance.FindPath(transform.position, _targetUnitPosition);


        if (_pathVectorList != null)
        {
            _pathVectorList.RemoveAt(0);
            switch (_pathVectorList.Count)
            {
                case int d when (IsMeleeRange(d)):
                    SelectedAction.Value = UnitAction.Action.Shoot;
                    TargetPosition.Value = _targetUnit.transform.position;
                    Debug.Log("Melee Attack");
                    break;

                case int d when (isRangedRange(d)):
                    SelectedAction.Value = UnitAction.Action.Shoot;
                    TargetPosition.Value = _targetUnit.transform.position;
                    Debug.Log("Ranged Attack");
                    break;

                default:
                    SelectedAction.Value = UnitAction.Action.Move;
                    TargetPosition.Value = _targetUnitPosition;
                    break;
            }
            GameHandler.Instance.DoAction(Vector3.zero,TargetPosition.Value);
        }
        else
        {
            Debug.Log("NPC distance between this and target is null, ending npc turn");
            IsMyTurn.Value = false;
        }
    }

    PlayerUnit GetPlayerWithHighestThreat()
    {
        List<int> playersThreat = new List<int>();
        foreach (NetworkClient playerClient in NetworkManager.Singleton.ConnectedClientsList)
        {
            int Threat = playerClient.PlayerObject.GetComponent<PlayerUnit>().Threat;
            playersThreat.Add(Threat);
        }
        int maxThreat = playersThreat.Max();
        int maxIndex = playersThreat.IndexOf(maxThreat);

        return NetworkManager.Singleton.ConnectedClients[(ulong)maxIndex].PlayerObject.GetComponent<PlayerUnit>();
    }

    Vector3 GetOffSetVector()
    {
        Vector3 offSetVector = Vector3.zero;
        if (IsTargetLeft) offSetVector = offSetVector + new Vector3(1f, 0f);
        if (IsTargetRight) offSetVector = offSetVector + new Vector3(-1f, 0f);
        if (IsTargetUp) offSetVector = offSetVector + new Vector3(0f, -1f);
        if (IsTargetDown) offSetVector = offSetVector + new Vector3(0f, 1f);

        return offSetVector;
    }

    void SetWalkableTargetPosition()
    {
        int whileCount = 0;

        List<PathNode> neighbourPathsNodeList = Pathfinding.Instance.GetNeighbourList(Pathfinding.Instance.GetGrid().GetGridObject(_targetUnit.transform.position));
        List<int> pathsVectorCount = new List<int>();

        foreach (PathNode pathNodePosition in neighbourPathsNodeList)
        {
            if (Pathfinding.Instance.FindPath(transform.position, pathNodePosition.GetPosition()) != null)
                pathsVectorCount.Add(Pathfinding.Instance.FindPath(transform.position, pathNodePosition.GetPosition()).Count);
        }


        while (!Pathfinding.Instance.GetGrid().GetGridObject(_targetUnitPosition).isWalkable)
        {
            whileCount++;
            int minCount = pathsVectorCount.Min();
            int minIndex = pathsVectorCount.IndexOf(minCount);
            pathsVectorCount.Remove(minIndex);
            _targetUnitPosition = neighbourPathsNodeList[minIndex].GetPosition();
            if (pathsVectorCount.Count < 1 || whileCount > 10)
            {
                Debug.Log("Whilecount break");
                break;
            }
        }
    }

}
