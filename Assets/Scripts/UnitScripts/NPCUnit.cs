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

    public int[] OwnedActionArray;

    bool IsTargetLeft => transform.position.x > _targetUnit.transform.position.x;
    bool IsTargetRight => _targetUnit.transform.position.x > transform.position.x;
    bool IsTargetUp => _targetUnit.transform.position.y > transform.position.y;
    bool IsTargetDown => transform.position.y > _targetUnit.transform.position.y;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsServer) return;
        //Test starting positions remember to remove
        if (UnitScriptableObject.UnitFaction != UnitSO.Faction.Hero)
        {
            Debug.Log("Spawneado enemigo");
            IsMyTurn.OnValueChanged += GameHandler.Instance.TurnHandler.OnIsMyTurnValueChanged;
            IsMyTurn.OnValueChanged += MyTurn;
            ActionPoints.OnValueChanged += GameHandler.Instance.TurnHandler.OnUnitActionPointsChanged;
            TargetPosition.OnValueChanged += GameHandler.Instance.DoAction;

            return;
        }
    }

    void MyTurn(bool previous, bool current)
    {
        if (!current || !IsServer || IsBusy) return;
        Debug.Log("MyTurn npc logic");

        //Getting target unit
        _targetUnit = GetPlayerWithHighestThreat();
        int randomNumber = (int)Mathf.Floor(Random.Range(0f, 9.9f));
        if (randomNumber <= 1)
        {
            int randomPlayerIndex = (int)Mathf.Floor(Random.Range(0f, NetworkManager.Singleton.ConnectedClientsList.Count - 0.1f));
            _targetUnit = NetworkManager.Singleton.ConnectedClients[(ulong)randomPlayerIndex].PlayerObject.GetComponent<PlayerUnit>();
        }

        StartCoroutine(DoTurn());

    }

    IEnumerator DoTurn()
    {
        yield return new WaitForSeconds(0.3f);
        int whilebreak = 0;
        while (IsMyTurn.Value)
        {
            yield return new WaitForSeconds(0.15f);
            whilebreak++;
            if(whilebreak > 50)
            {
                Debug.Log("while break");
                ActionPoints.Value = 0;
                yield break;
            }
            if (IsBusy)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            //Getting the path
            _pathVectorList = new List<Vector3>();
            _pathVectorList = Pathfinding.Instance.FindPathToNotWalkable(transform.position, _targetUnit.transform.position);
            _pathVectorList.RemoveAt(0);

            if (_pathVectorList == null)
            {
                ActionPoints.Value -= 1;
                yield break;
            }

            if (IsRangedRange(_pathVectorList.Count))
            {
                switch (ActionPoints.Value)
                {
                    case 1:
                        if (OwnedActionArray.Contains(3))
                        {
                            //do ranged attack
                            SelectedAction.Value = UnitAction.Action.Ranged;
                            TargetPosition.Value = _targetUnit.transform.position;
                            GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                            Debug.Log("Ranged Attack");
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                        break;
                    case >= 2:
                        if (OwnedActionArray.Contains(4) || OwnedActionArray.Contains(5))
                        {
                            if (OwnedActionArray.Contains(4))
                            {
                                SelectedAction.Value = UnitAction.Action.Magic;
                                TargetPosition.Value = _targetUnit.transform.position;
                                GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                                Debug.Log("Magic Attack");
                                yield return new WaitForSeconds(0.1f);
                                continue;
                            }
                            else if (OwnedActionArray.Contains(5))
                            {
                                SelectedAction.Value = UnitAction.Action.ShieldBash;
                                TargetPosition.Value = _targetUnit.transform.position;
                                GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                                Debug.Log("Shield Attack");
                                yield return new WaitForSeconds(0.1f);
                                continue;
                            }
                            if (OwnedActionArray.Contains(3))
                            {
                                //do ranged attack
                                SelectedAction.Value = UnitAction.Action.Ranged;
                                TargetPosition.Value = _targetUnit.transform.position;
                                GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                                Debug.Log("Ranged Attack");
                                yield return new WaitForSeconds(0.1f);
                                continue;
                            }
                        }
                        break;
                    case <= 0:
                        Debug.Log("se me acabaron los action pero sigue siendo mi turno");
                        break;
                }
            }

            if (IsMeleeRange(_pathVectorList.Count))
            {
                switch (ActionPoints.Value)
                {
                    case 1:
                        if (OwnedActionArray.Contains(2))
                        {
                            SelectedAction.Value = UnitAction.Action.Meele;
                            TargetPosition.Value = _targetUnit.transform.position;
                            GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                            Debug.Log("Melee Attack");
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                        break;
                    case >= 2:
                        if (OwnedActionArray.Contains(16))
                        {
                            SelectedAction.Value = UnitAction.Action.Cleave;
                            TargetPosition.Value = _targetUnit.transform.position;
                            GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                            Debug.Log("Cleave Attack");
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                        if (OwnedActionArray.Contains(2))
                        {
                            SelectedAction.Value = UnitAction.Action.Meele;
                            TargetPosition.Value = _targetUnit.transform.position;
                            GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                            Debug.Log("Melee Attack");
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                        break;
                    case <= 0:
                        Debug.Log("se me acabaron los action pero sigue siendo mi turno");
                        break;

                }
            }

            if (IsHeadButtRange(_pathVectorList.Count))
            {
                switch (ActionPoints.Value)
                {
                    case 1:
                        if (OwnedActionArray.Contains(7))
                        {
                            SelectedAction.Value = UnitAction.Action.Headbutt;
                            TargetPosition.Value = _targetUnit.transform.position;
                            GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                            Debug.Log("Melee Attack");
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                        break;
                    case >= 2:
                        if (OwnedActionArray.Contains(9))
                        {
                            SelectedAction.Value = UnitAction.Action.Poison;
                            TargetPosition.Value = _targetUnit.transform.position;
                            GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                            Debug.Log("Melee Attack");
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                        if (OwnedActionArray.Contains(7))
                        {
                            SelectedAction.Value = UnitAction.Action.Headbutt;
                            TargetPosition.Value = _targetUnit.transform.position;
                            GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);

                            Debug.Log("Melee Attack");
                            yield return new WaitForSeconds(0.1f);
                            continue;
                        }
                        break;
                }
            }
            if(ActionPoints.Value >= 1)
            {

                _targetUnitPosition = _targetUnit.transform.position + GetOffSetVector();

                PathNode targetNode = Pathfinding.Instance.GetGrid().GetGridObject(_targetUnitPosition);

                if (!targetNode.isWalkable)
                {
                    SetWalkableTargetPosition();
                }


                SelectedAction.Value = UnitAction.Action.Move;
                TargetPosition.Value = _targetUnitPosition;
                GameHandler.Instance.DoAction(Vector3.zero, TargetPosition.Value);
                yield return new WaitForSeconds(1f);
                continue;
            }  
        }

        yield break;
    }

    bool IsHeadButtRange(int distance) {
        if (distance == 1) return true;
        return false;
    }

    bool IsMeleeRange(int distance)
    {
        if (distance >= 1 && 1 + ((int)Mathf.Floor(Stats.Value.Dexterity / 2)) >= distance) return true;
        return false;

    }

    bool IsRangedRange(int distance)
    {
        if (distance >= 1 && 2 + ((int)Mathf.Floor(Stats.Value.Dexterity / 2)) >= distance) return true;
        return false;

    }

    public override void Die()
    {
        base.Die();
        GameHandler.Instance.GetGrid().GetGridObject(transform.position).RemoveUnit();
        GameHandler.Instance.RemoveEnemyFromList(this);
        ActionPoints.Value = 0;
        StopCoroutine(DoTurn());
        //playsound?
        Destroy(this.gameObject);
    }

    /* private void Update()
    {
        if (!IsMyTurn.Value || !IsServer || IsBusy) return;
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
                    SelectedAction.Value = UnitAction.Action.Meele;
                    TargetPosition.Value = _targetUnit.transform.position;
                    Debug.Log("Melee Attack");
                    break;

                case int d when (isRangedRange(d)):
                    SelectedAction.Value = UnitAction.Action.Ranged;
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
    }*/

    PlayerUnit GetPlayerWithHighestThreat() //cambiar esto a tipo rail
    {
        List<int> playersThreat = new List<int>();
        foreach (NetworkClient playerClient in NetworkManager.Singleton.ConnectedClientsList)
        {
            int Threat = playerClient.PlayerObject.GetComponent<PlayerUnit>().Threat.Value;
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
