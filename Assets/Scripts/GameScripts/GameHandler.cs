using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class GameHandler : NetworkBehaviour
{
    //Singleton instance
    public static GameHandler instance { get; private set; }

    public GameObject playerPrefab;

    //All the grids needed for the game to work
    private Grid<GridObject> grid;
    private Pathfinding pathfindingGrid;
    private Tilemap tilemapGrid;

    //Map holders to instantiate when needed
    public Transform[] enviromentPrefabs;
    public GameObject enviromentPrefabHolder;

    //Turn vars
    bool[] hasBeenRegistered = new bool[5];
    private NetworkVariable<ulong> currentTurn = new NetworkVariable<ulong>();
    int turnIndex;

    //Pathfinding vars
    //List<Vector3> pathVectorList = new List<Vector3>();
    //public float speed = 4f;
    //int currentPathIndex;

    //public GameObject actionsUI;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);
        }
    }

    //Initalize singletonm, grids and turnIndex
    void Start()
    {
        instance = this;
        enviromentPrefabs = enviromentPrefabHolder.GetComponent<EnviromentHolder>().enviromentPrefabs;
        grid = new Grid<GridObject>(18, 10, 1f, new Vector3(0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(grid, x, y));
        tilemapGrid = new Tilemap(18, 10, 1f, new Vector3(0, 0));
        pathfindingGrid = new Pathfinding(18, 10, 1f, new Vector3(0, 0));
        CreateWorld();
        InstantiateWorld();
        turnIndex = 0;
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (!IsServer) return;
            turnIndex--;
            if (turnIndex < 0) turnIndex = 0;
        };

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer || turnIndex < 0) return;
        UnitO unit = NetworkManager.Singleton.ConnectedClientsList[turnIndex].PlayerObject.GetComponent<UnitO>();

        /*foreach (NetworkObject networkObject in NetworkManager.Singleton.ConnectedClientsList[turnIndex].OwnedObjects)
        {
            if (networkObject.tag == "ActionsUI")
            {
                actionsUI = networkObject.gameObject;
            }
        }*/

        if (hasBeenRegistered[turnIndex] == false)
        {
            hasBeenRegistered[turnIndex] = true;
            AddListeners(unit);
        }
        unit.isMyTurn.Value = true;
        //actionsUI.SetActive(true);
        //if (unit.state.Value == UnitO.State.Moving) Move(unit);

    }

    void AddListeners(UnitO unit)
    {
        unit.isMyTurn.OnValueChanged += OnTurnEnd;
        unit.targetPositionRpc.OnValueChanged += MoveAction;
        //actionsUI.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(SetSelectedAction);
    }

    /*public void SetSelectedAction()
    {
        UnitO unit = NetworkManager.Singleton.ConnectedClientsList[turnIndex].PlayerObject.GetComponent<UnitO>();
        unit.SetSelectedActionServerRpc("Move");
    }*/

    public Pathfinding GetPathfindingGrid()
    {
        return pathfindingGrid;
    }

    public Grid<GridObject> GetGrid()
    {
        return grid;
    }

    //OnTurnEnd remove the listener and do NextTurn();
    void OnTurnEnd(bool previous, bool current)
    {
        if (!current)
        {
            UnitO unit = NetworkManager.Singleton.ConnectedClientsList[turnIndex].PlayerObject.GetComponent<UnitO>();
            unit.selectedAction.Value = "";
            //actionsUI.SetActive(false);
            NextTurn();
        }
    }

    /*void MoveAction(Vector3 previous, Vector3 current)
    {
        UnitO unit = NetworkManager.Singleton.ConnectedClientsList[turnIndex].PlayerObject.GetComponent<UnitO>();
        SetTargetPositionTo(unit);
    }*/

    void MoveAction(Vector3 previous, Vector3 current)
    {
        UnitO unit = NetworkManager.Singleton.ConnectedClientsList[turnIndex].PlayerObject.GetComponent<UnitO>();
        GetComponent<MoveAction>().Setup(unit);
        GetComponent<MoveAction>().Move(unit);
    }

    //Next turn logic
    void NextTurn()
    {
        ulong[] turnIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray();
        turnIndex++;
        if (turnIndex >= turnIds.Length) turnIndex = 0;
        currentTurn.Value = turnIds[turnIndex];
        Debug.Log("current turn: " + turnIndex + " maxTurn: " + turnIds.Length);

    }

    public void CreateWorld()
    {
        for (int x = 0; x < tilemapGrid.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < tilemapGrid.GetGrid().GetHeight(); y++)
            {
                tilemapGrid.spriteRaycast(x, y);
            }
        }
    }

    public void InstantiateWorld()
    {
        for (int x = 0; x < tilemapGrid.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < tilemapGrid.GetGrid().GetHeight(); y++)
            {
                if ((int)tilemapGrid.GetGrid().GetGridObject(x, y).GetTilemapSprite() != 0)
                    Instantiate(enviromentPrefabs[(int)tilemapGrid.GetGrid().GetGridObject(x, y).GetTilemapSprite() - 1], tilemapGrid.GetGrid().GetWorldPosition(x, y) + new Vector3(1f, 1f) * .5f, Quaternion.identity, this.transform);
                if ((int)tilemapGrid.GetGrid().GetGridObject(x, y).GetTilemapSprite() != 1)
                {
                    pathfindingGrid.GetGrid().GetGridObject(x, y).SetWalkable(false);
                }
            }
        }
    }

    /*public void SetTargetPositionTo(UnitO unit)
    {
        currentPathIndex = 0;

        pathVectorList.Clear();
        pathVectorList = Pathfinding.Instance.FindPath(unit.transform.position, unit.targetPositionRpc.Value);

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
            //OnPositionReachedServerRpc();
        }
        else
        {
            //notReachableServerRpc();
            unit.state.Value = UnitO.State.Normal;
            Debug.Log("path vector list is lower than 1");
        }
    }*/

    /*
    private void Move(UnitO unit)
    {
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
                    //unit.SetStateServerRpc(State.Normal);
                    unit.state.Value = UnitO.State.Normal;
                    //EndTurnServerRpc();
                    unit.isMyTurn.Value = false;
                    //onPositionReached?.Invoke();
                }
            }

        }
        else
        {
            //notReachable?.Invoke();
            Debug.Log("Not rechable");
            unit.state.Value = UnitO.State.Normal;
        }
    }
    */

    //GridObject needed to keep track of the players in the grid
    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        UnitO unit;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetUnit(UnitO unit)
        {
            this.unit = unit;
            instance.GetPathfindingGrid().GetGrid().GetGridObject(x, y).SetWalkable(false);
        }

        public UnitO GetUnit()
        {
            return this.unit;
        }

        public void RemoveUnit()
        {
            this.unit = null;
            instance.GetPathfindingGrid().GetGrid().GetGridObject(x, y).SetWalkable(true);
        }

        public override string ToString()
        {
            return x + ":" + y + unit;
        }

    }
}
