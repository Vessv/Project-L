using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class UnitHandler : NetworkBehaviour
{
    //UnitSO and stats
    public UnitSO unitSO;
    HealthSystem healthSystem;
    public int vitality;

    //Turn network variables
    private NetworkVariable<Vector3> targetPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> playerPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<State> state = new NetworkVariable<State>();
    public NetworkVariable<bool> isMyTurn = new NetworkVariable<bool>();
    public NetworkVariable<int> onPositionReached = new NetworkVariable<int>(); 
    public NetworkVariable<int> notReachable = new NetworkVariable<int>();

    private void Awake()
    {
       pathVectorList = new NetworkList<Vector3>();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthSystem = new HealthSystem(unitSO.vitality * 10);
        if(IsLocalPlayer)SetStateServerRpc(State.Normal);
        if (IsLocalPlayer)SumbitPositionServerRpc(new Vector3(0.5f, 0.5f));

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerPosition.Value;
        if (!isMyTurn.Value || !IsLocalPlayer) return;

        
        if(state.Value == State.Moving) Move();

        if (Input.GetMouseButtonDown(0) && state.Value == State.Normal)
        {
            SetStateServerRpc(State.Moving);
            SetTargetPositionTo(Utils.GetMouseWorldPosition());
            
        }

        /*if (Input.GetMouseButtonDown(1))
        {
            GameHandler.instance.GetGrid().GetGridObject(transform.position).SetUnit(this);
            if (transform.position.x < 1)
            {
                Attack(GameHandler.instance.GetGrid().GetGridObject(Utils.GetMouseWorldPosition()).GetUnit());
            }
        }*/        

    }

    [ServerRpc]
    void SetStateServerRpc(State statev)
    {
        state.Value = statev;
    }

    [ServerRpc]
    void SetTargetPoisitionToServerRpc(Vector3 position)
    {
        targetPosition.Value = position;
    }

    [ServerRpc]
    void EndTurnServerRpc()
    {
        isMyTurn.Value = false;
    }
    [ServerRpc]
    void OnPositionReachedServerRpc()
    {
        onPositionReached.Value++;
    }
    [ServerRpc]
    void notReachableServerRpc()
    {
        notReachable.Value++;
    }

    NetworkList<Vector3> pathVectorList;

    int currentPathIndex;
    public float speed;

    private void Move()
    {
        if (pathVectorList != null && pathVectorList.Count > 0)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(playerPosition.Value, targetPosition) > 0.05f)
            {
                Vector3 moveDir = (targetPosition - playerPosition.Value).normalized;

                SumbitPositionServerRpc(playerPosition.Value + moveDir * speed * Time.deltaTime);
                //transform.position = playerPosition.Value;
                if (Vector3.Distance(playerPosition.Value, targetPosition) < 0.05f)
                {
                    SumbitPositionServerRpc(targetPosition);
                }
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList.Clear();
                    SetStateServerRpc(State.Normal);
                    EndTurnServerRpc();
                    //onPositionReached?.Invoke();
                }
            }

        }
        else
        {
            //notReachable?.Invoke();
            Debug.Log("Not rechable");
            SetStateServerRpc(State.Normal);
        }
    }

    private void Move2()
    {
        if (pathVectorList != null && pathVectorList.Count > 0)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                SumbitPositionServerRpc(transform.position + moveDir * speed * Time.deltaTime);
                //transform.position = playerPosition.Value;
                if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
                {
                    SumbitPositionServerRpc(targetPosition);
                }
                transform.position = playerPosition.Value;
                Debug.Log(playerPosition.Value);
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList.Clear();
                    SetStateServerRpc(State.Normal);
                    EndTurnServerRpc();
                    //onPositionReached?.Invoke();
                }
            }

        }
        else
        {
            //notReachable?.Invoke();
            Debug.Log("Not rechable");
            SetStateServerRpc(State.Normal);
        }
    }

    private void MoveCopy()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
                {
                    transform.position = targetPosition;

                }
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList.Clear();
                    SetStateServerRpc(State.Normal);
                    //onPositionReached?.Invoke();
                }
            }

        }
        else
        {
            //notReachable?.Invoke();
            Debug.Log("Not rechable");
            SetStateServerRpc(State.Normal);
        }
    }

    public void SetTargetPositionTo(Vector3 _targetPosition)
    {
        Debug.Log("path count 1: " + pathVectorList.Count);
        SetTargetPoisitionToServerRpc(_targetPosition);

        currentPathIndex = 0;

        pathVectorList.Clear();
        List<Vector3> pathList = Pathfinding.Instance.FindPath(transform.position, _targetPosition);
        foreach (Vector3 vector in pathList)
        {
            AddPathVectorListServerRpc(vector);
        }
        Debug.Log("path count 2: " + pathVectorList.Count);
        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
            //OnPositionReachedServerRpc();
        }
        else
        {
            //notReachableServerRpc();
            SetStateServerRpc(State.Normal);
            Debug.Log("path vector list is lower than 1");
        }
    }

    [ServerRpc]
    void AddPathVectorListServerRpc(Vector3 vector)
    {
        pathVectorList.Add(vector);
    }

    [ServerRpc]
    void RemoveVectorPathVectorListServerRpc()
    {
        pathVectorList.RemoveAt(0);
    }

    [ServerRpc]
    void ClearPathVectorListServerRpc()
    {
        pathVectorList.Clear();
    }

    [ServerRpc]
    void SumbitPositionServerRpc(Vector3 position, ServerRpcParams rpcParams = default)
    {
        playerPosition.Value = position;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public enum State
    {
        Normal,
        Moving,
        Attacking
    }

}
