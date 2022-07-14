using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public override ActionType GetActionType() => ActionType.Move;

    //Pathfinding vars
    List<Vector3> pathVectorList = new List<Vector3>();
    public float speed = 4f;
    int currentPathIndex;

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        if (unit == null) return;
        if (unit.ActionStatus.Value != Unit.ActionState.Busy && unit.SelectedAction.Value != UnitAction.Action.Move) return;

        if (pathVectorList != null && pathVectorList.Count > 0)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(unit.transform.position, targetPosition) > 0.05f)
            {
                Vector3 moveDir = (targetPosition - unit.transform.position).normalized;

                unit.transform.position = unit.transform.position + moveDir * speed * Time.deltaTime;
                if (Vector3.Distance(unit.transform.position, targetPosition) < 0.05f)
                {
                    unit.transform.position = targetPosition;
                }
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList.Clear();
                    GameHandler.instance.GetGrid().GetGridObject(unit.TargetPosition.Value).SetUnit(unit);
                    //unit.SetStateServerRpc(State.Normal);
                    unit.ActionStatus.Value = Unit.ActionState.Normal;
                    //EndTurnServerRpc();
                    unit.IsMyTurn.Value = false;
                    //onPositionReached?.Invoke();
                }
            }

        }
        else
        {
            //notReachable?.Invoke();
            Debug.Log("Not rechable");
            unit.ActionStatus.Value = Unit.ActionState.Normal;
        }
    }

    public void Move()
    {
        currentPathIndex = 0;

        pathVectorList.Clear();
        pathVectorList = Pathfinding.Instance.FindPath(unit.transform.position, unit.TargetPosition.Value);

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
            GameHandler.instance.GetGrid().GetGridObject(unit.transform.position).RemoveUnit();
            //OnPositionReachedServerRpc();
        }
        else
        {
            //notReachableServerRpc();
            unit.ActionStatus.Value = Unit.ActionState.Normal;
            Debug.Log("path vector list is lower than 1");
        }
    }
}
