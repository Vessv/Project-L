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
    public static GameHandler Instance { get; private set; }

    [SerializeField]
    GameObject _playerPrefab;

    [SerializeField]
    GameObject _enemyPrefab;

    //Turn vars
    [SerializeField]
    bool[] _hasBeenRegistered = new bool[5];
    [SerializeField]
    int _currentTurnIndex;
    [SerializeField]
    bool hasPlayersCycleBeenDone;


    [SerializeField]
    MoveAction _moveAction;
    [SerializeField]
    AttackAction _attackAction;

    //All the grids needed for the game to work
    private Grid<GridObject> _gameGrid;
    private Pathfinding _pathfindingGrid;
    private Tilemap _tilemapGrid;
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;

    //Map holders to instantiate when needed
    public Transform[] EnviromentPrefabs;
    public GameObject EnviromentPrefabHolder;

    public GameObject EnemyListHolder;


    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            GameObject player = Instantiate(_playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);
        }

        GameObject enemy = Instantiate(_enemyPrefab);

        enemy.GetComponent<NetworkObject>().Spawn();

        currentUnit = NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.GetComponent<Unit>();
        currentUnit.IsMyTurn.Value = true;
        hasPlayersCycleBeenDone = false;
    }

    //Initalize singleton, grids and _currentTurnIndex
    private void Awake()
    {
        Instance = this;

        _moveAction = GetComponent<MoveAction>();
        _attackAction = GetComponent<AttackAction>();

        EnviromentPrefabs = EnviromentPrefabHolder.GetComponent<EnviromentHolder>().EnviromentPrefabs;

        _gameGrid = new Grid<GridObject>(_width, _height, 1f, new Vector3(0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(_gameGrid, x, y));
        _tilemapGrid = new Tilemap(_width, _height, 1f, new Vector3(0, 0));
        _pathfindingGrid = new Pathfinding(_width, _height, 1f, new Vector3(0, 0));

        _currentTurnIndex = 0;

    }

    void Start()
    {

        CreateWorld();
        InstantiateWorld();

        //Move to a method
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (!IsServer) return;
            _currentTurnIndex--;
            if (_currentTurnIndex < 0) _currentTurnIndex = 0;
        };

    }

    bool hasPlayers => NetworkManager.Singleton.ConnectedClientsList.Count > 0;

    [SerializeField]
    Unit currentUnit;

    // Update is called once per frame
    void Update()
    {
        if (!IsServer || !hasPlayers) return;

        if (!_hasBeenRegistered[_currentTurnIndex])
        {
            _hasBeenRegistered[_currentTurnIndex] = true;
            AddListeners(currentUnit);
        }
        currentUnit.IsMyTurn.Value = true;

    }

    void AddListeners(Unit unit)
    {
        unit.IsMyTurn.OnValueChanged += OnTurnEnd;
        unit.TargetPosition.OnValueChanged += DoAction;
    }

    public Pathfinding GetPathfindingGrid()
    {
        return _pathfindingGrid;
    }

    public Grid<GridObject> GetGrid()
    {
        return _gameGrid;
    }

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
        bool isOutOfBounds = current.x < 0 || current.y < 0 || current.x >= _width || current.y >= _height;

        if (isOutOfBounds)
        {
            Debug.Log("GameHandler.cs error DoAction() out of bounds x,y = " + current.x + "," + current.y);
            return;
        }
        currentUnit.ActionStatus.Value = Unit.ActionState.Busy;
        if (currentUnit.CanMove)
        {
            _moveAction.Setup(currentUnit);
            _moveAction.Move();
        } else 
        if(currentUnit.CanShoot)
        {
            Unit targetUnit = _gameGrid.GetGridObject(current).GetUnit();
            bool isAValidTarget = targetUnit != null && targetUnit != currentUnit;
            if (!isAValidTarget)
            {
                currentUnit.ActionStatus.Value = Unit.ActionState.Normal;
                Debug.Log("GameHandler.cs error DoAction() no valid target at x,y =" + current.x + "," + current.y);
                return;
            }
            _attackAction.Setup(currentUnit);
            _attackAction.Attack(_gameGrid.GetGridObject(currentUnit.TargetPosition.Value).GetUnit());
        }

    }

    void NextTurn()
    {
        if (hasPlayersCycleBeenDone)
        {
            //Enemy turn
            //Gets the enemies from a enemy holder does one turn for each enemy ends enemycycle
            //On cycle done, player cycle goes
            //add speed stat? would need to put all units on an array and do turns order depending on the speed stat
            //add actions points and actions consumes certain amounts of them, turns ends when u run out of it or can't do anything more.
            hasPlayersCycleBeenDone = false;
        }
        else
        {

            ulong[] turnIds = NetworkManager.Singleton.ConnectedClients.Keys.ToArray();
            _currentTurnIndex++;

            if (_currentTurnIndex >= turnIds.Length)
            {
                _currentTurnIndex = 0;
                Debug.Log("current turn: " + _currentTurnIndex + " maxTurn: " + turnIds.Length);
                hasPlayersCycleBeenDone = true;
                return;
            }
            Debug.Log("current turn: " + _currentTurnIndex + " maxTurn: " + turnIds.Length);
            currentUnit = NetworkManager.Singleton.ConnectedClientsList[_currentTurnIndex].PlayerObject.GetComponent<Unit>();
        }

    }

    public void CreateWorld()
    {
        for (int x = 0; x < _tilemapGrid.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < _tilemapGrid.GetGrid().GetHeight(); y++)
            {
                _tilemapGrid.spriteRaycast(x, y);
            }
        }
    }

    public void InstantiateWorld()
    {
        for (int x = 0; x < _tilemapGrid.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < _tilemapGrid.GetGrid().GetHeight(); y++)
            {
                if ((int)_tilemapGrid.GetGrid().GetGridObject(x, y).GetTilemapSprite() != 0)
                    Instantiate(EnviromentPrefabs[(int)_tilemapGrid.GetGrid().GetGridObject(x, y).GetTilemapSprite() - 1], _tilemapGrid.GetGrid().GetWorldPosition(x, y) + new Vector3(1f, 1f) * .5f, Quaternion.identity, this.transform);
                if ((int)_tilemapGrid.GetGrid().GetGridObject(x, y).GetTilemapSprite() != 1)
                {
                    _pathfindingGrid.GetGrid().GetGridObject(x, y).SetWalkable(false);
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
            Instance.GetPathfindingGrid().GetGrid().GetGridObject(x, y).SetWalkable(false);
        }

        public Unit GetUnit()
        {
            return this.unit;
        }

        public void RemoveUnit()
        {
            this.unit = null;
            Instance.GetPathfindingGrid().GetGrid().GetGridObject(x, y).SetWalkable(true);
        }

        public override string ToString()
        {
            return x + ":" + y + unit;
        }

    }
}
