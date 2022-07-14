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
    private Grid<GridObject> gameGrid;
    private Pathfinding pathfindingGrid;
    private Tilemap tilemapGrid;

    //Map holders to instantiate when needed
    public Transform[] enviromentPrefabs;
    public GameObject enviromentPrefabHolder;

    //Turn vars
    bool[] hasBeenRegistered = new bool[5];
    private NetworkVariable<ulong> CurrentTurn = new NetworkVariable<ulong>();
    int turnIndex;

    [SerializeField]
    MoveAction moveAction;
    [SerializeField]
    AttackAction attackAction;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);
        }

        currentUnit = NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.GetComponent<Unit>();
        currentUnit.IsMyTurn.Value = true;
    }

    //Initalize singleton, grids and turnIndex
    private void Awake()
    {
        instance = this;

        moveAction = GetComponent<MoveAction>();
        attackAction = GetComponent<AttackAction>();

        enviromentPrefabs = enviromentPrefabHolder.GetComponent<EnviromentHolder>().enviromentPrefabs;

        gameGrid = new Grid<GridObject>(18, 10, 1f, new Vector3(0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(gameGrid, x, y));
        tilemapGrid = new Tilemap(18, 10, 1f, new Vector3(0, 0));
        pathfindingGrid = new Pathfinding(18, 10, 1f, new Vector3(0, 0));

        turnIndex = 0;

    }

    void Start()
    {

        CreateWorld();
        InstantiateWorld();

        //Move to a method
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (!IsServer) return;
            turnIndex--;
            if (turnIndex < 0) turnIndex = 0;
        };

    }

    bool hasPlayers => NetworkManager.Singleton.ConnectedClientsList.Count > 0;

    [SerializeField]
    Unit currentUnit;
    // Update is called once per frame
    void Update()
    {
        if (!IsServer || !hasPlayers) return;

        if (!hasBeenRegistered[turnIndex])
        {
            hasBeenRegistered[turnIndex] = true;
            AddListeners(currentUnit);
        }
        currentUnit.IsMyTurn.Value = true;

    }

    void AddListeners(Unit unit)
    {
        unit.IsMyTurn.OnValueChanged += OnTurnEnd;
        unit.TargetPosition.OnValueChanged += DoAction;
        //actionsUI.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(SetSelectedAction);
    }

    /*public void SetSelectedAction()
    {
        Unit unit = NetworkManager.Singleton.ConnectedClientsList[turnIndex].PlayerObject.GetComponent<Unit>();
        unit.SetSelectedActionServerRpc("Move");
    }*/

    public Pathfinding GetPathfindingGrid()
    {
        return pathfindingGrid;
    }

    public Grid<GridObject> GetGrid()
    {
        return gameGrid;
    }

    //OnTurnEnd remove the listener and do NextTurn();
    void OnTurnEnd(bool previous, bool current)
    {
        if (!current)
        {
            currentUnit.SelectedAction.Value = UnitAction.Action.None;
            NextTurn();
        }
    }
    void DoAction(Vector3 previous, Vector3 current)
    {
        if (currentUnit.CanMove)
        {
            moveAction.Setup(currentUnit);
            moveAction.Move();
        } else 
        if(currentUnit.CanShoot)
        {
            attackAction.Setup(currentUnit);
            attackAction.Shoot(gameGrid.GetGridObject(currentUnit.TargetPosition.Value).GetUnit());
        }

    }

    //Next turn logic
    void NextTurn()
    {
        ulong[] turnIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray();
        turnIndex++;
        if (turnIndex >= turnIds.Length) turnIndex = 0;
        CurrentTurn.Value = turnIds[turnIndex];
        currentUnit = NetworkManager.Singleton.ConnectedClientsList[turnIndex].PlayerObject.GetComponent<Unit>();
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

    //GridObject needed to keep track of the players in the grid
    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        Unit unit;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetUnit(Unit unit)
        {
            this.unit = unit;
            instance.GetPathfindingGrid().GetGrid().GetGridObject(x, y).SetWalkable(false);
        }

        public Unit GetUnit()
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
