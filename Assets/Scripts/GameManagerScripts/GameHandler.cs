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

    bool hasPlayers => NetworkManager.Singleton.ConnectedClientsList.Count > 0;


    [SerializeField]
    MoveAction _moveAction;
    [SerializeField]
    AttackAction _attackAction;

    [SerializeField]
    ActionSO[] _actionsSOArray;

    [SerializeField]
    Item[] _itemsSOArray;


    public UnitSO[] UnitSOArray;

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

    public GameObject ActionUIHolder;

    public List<BaseUnit> EnemyList;

    public TurnHandler TurnHandler;

    public Vector2 GetWidthAndHeight()
    {
        return new Vector2(_width, _height);
    }

    public ActionSO[] GetActionsSOArray()
    {
        return _actionsSOArray;
    }

    public Item[] GetItemsSOArray()
    {
        return _itemsSOArray;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        TurnHandler = GetComponent<TurnHandler>();
        EnemyList = GetComponent<EnemyHolder>().EnemyList;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Debug.Log(ServerGameNetPortal.Instance.choosenHero);
            GameObject player = Instantiate(_playerPrefab); //aca cambiar el prefab o algo pero eso como al final cuando este todo
            
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);
            player.GetComponent<PlayerUnit>().UnitScriptableObject = UnitSOArray[ServerGameNetPortal.Instance.choosenHero[(int)client.ClientId]];
            player.GetComponent<SpriteRenderer>().sprite = UnitSOArray[ServerGameNetPortal.Instance.choosenHero[(int)client.ClientId]].UnitSprite;
            player.GetComponent<PlayerUnit>().LoadUnitStats();
            player.GetComponent<PlayerUnit>().UpdateUnitSOClientRpc(ServerGameNetPortal.Instance.choosenHero[(int)client.ClientId]);
            //player.GetComponent<PlayerUnit>().ActionInventory.GetComponent<NetworkObject>().SpawnWithOwnership(client.ClientId);
            player.transform.position = player.transform.position + new Vector3((float)(client.ClientId + 8f),1f);
            _gameGrid.GetGridObject(player.transform.position).SetUnit(player.GetComponent<BaseUnit>());

            //Dandole skills base o algo
            player.GetComponent<PlayerUnit>().ownedActionList.Add(1);

            //habilidades inciailes de cada heore e items si es necesario
            switch (ServerGameNetPortal.Instance.choosenHero[client.ClientId])
            {
                case 0:
                    player.GetComponent<PlayerUnit>().ownedActionList.Add(2);
                    break;
                case 1:
                    player.GetComponent<PlayerUnit>().ownedActionList.Add(2);
                    break;
                case 2:
                    player.GetComponent<PlayerUnit>().ownedActionList.Add(3);
                    break;
            }

        }

        //Enemy spawn placeholder change later
        GameObject enemy = Instantiate(_enemyPrefab);
        enemy.GetComponent<NetworkObject>().Spawn();
        EnemyList.Add(enemy.GetComponent<NPCUnit>());
        enemy.transform.position = new Vector3(10.5f, 10.5f);
        Instance.GetGrid().GetGridObject(new Vector3(10.5f, 10.5f)).SetUnit(enemy.GetComponent<NPCUnit>());
        GameObject enemy2 = Instantiate(_enemyPrefab);
        enemy2.GetComponent<NetworkObject>().Spawn();
        EnemyList.Add(enemy2.GetComponent<NPCUnit>());
        enemy2.transform.position = new Vector3(12.5f, 12.5f);
        Instance.GetGrid().GetGridObject(new Vector3(12.5f, 12.5f)).SetUnit(enemy2.GetComponent<NPCUnit>());


        FloorEnd(); //quitar de aca 
    }


    private void FloorStart()
    {
        //remove black screen?
    }

    private void FloorEnd()
    {
        //enable ui wtih blessing so they choose
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.gameObject.GetComponent<PlayerUnit>().DisplayBlessingSelectionClientRpc();
        }
        //after everyone has choosen black screen algo como player.changeblackscreenclientrpc
        //move to initial position
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList) //mirar si esto si funciona
        {
            GameObject player = client.PlayerObject.gameObject;
            player.transform.position = player.transform.position + new Vector3((float)(client.ClientId + 8f), 1f);
            _gameGrid.GetGridObject(player.transform.position).SetUnit(player.GetComponent<BaseUnit>());
        }
            
        //change map spawnear cosas, tenerlos en una lista, updatear el pathfinding si ese necesario, remover los anteriores, updatear el pathfinding
        //spawn new enemies, de una lista que tenga waves de enemigos o algo 
        //floorstart es necesario floor start?
    }

    //Initalize singleton, grids and TurnHandler.CurrentTurnIndex
    private void Awake()
    {
        Instance = this;

        _moveAction = GetComponent<MoveAction>();
        _attackAction = GetComponent<AttackAction>();

        EnviromentPrefabs = EnviromentPrefabHolder.GetComponent<EnviromentHolder>().EnviromentPrefabs;

        _gameGrid = new Grid<GridObject>(_width, _height, 1f, new Vector3(0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(_gameGrid, x, y));
        _tilemapGrid = new Tilemap(_width, _height, 1f, new Vector3(0, 0));
        _pathfindingGrid = new Pathfinding(_width, _height, 1f, new Vector3(0, 0));

        //not walkable by default
        _pathfindingGrid.GetGrid().GetGridObject(7, 0).SetWalkable(false);
        _pathfindingGrid.GetGrid().GetGridObject(8, 0).SetWalkable(false);
        _pathfindingGrid.GetGrid().GetGridObject(9, 0).SetWalkable(false);
        _pathfindingGrid.GetGrid().GetGridObject(10, 0).SetWalkable(false);
        _pathfindingGrid.GetGrid().GetGridObject(6, 2).SetWalkable(false);
        _pathfindingGrid.GetGrid().GetGridObject(11, 2).SetWalkable(false);
    }

    void Start()
    {

        //CreateWorld();
        //InstantiateWorld();
      
    }


    // Update is called once per frame
    void Update()
    {
        if (!IsServer || !hasPlayers) return;

        if (!_hasBeenRegistered[TurnHandler.CurrentTurnIndex] && TurnHandler.CurrentUnit.UnitScriptableObject.UnitFaction != UnitSO.Faction.Demon)
        {
            _hasBeenRegistered[TurnHandler.CurrentTurnIndex] = true;
            AddListeners(TurnHandler.CurrentUnit);
        }

    }

    void AddListeners(BaseUnit unit)
    {
        unit.IsMyTurn.OnValueChanged += TurnHandler.OnIsMyTurnValueChanged;
        unit.TargetPosition.OnValueChanged += DoAction;
        unit.ActionPoints.OnValueChanged += TurnHandler.OnUnitActionPointsChanged;
        unit.SelectedAction.OnValueChanged += ShowVisualMap;
    }

    

    public Pathfinding GetPathfindingGrid()
    {
        return _pathfindingGrid;
    }

    public Grid<GridObject> GetGrid()
    {
        return _gameGrid;
    }

    private void ShowVisualMap(UnitAction.Action previousValue, UnitAction.Action newValue)
    {
        if (TurnHandler.hasPlayersCycleBeenDone)
        {
            return;
        }
        PlayerUnit currentPlayer = (PlayerUnit)TurnHandler.CurrentUnit;
        currentPlayer.InitializeMapHolderClientRpc();
        currentPlayer.HideAllMapVisualTileClientRpc();
        switch (newValue)
        {
            case UnitAction.Action.Move:
                _moveAction.Setup(TurnHandler.CurrentUnit);
                _moveAction.ShowMoveTiles();
                break;
        }
    }

    public void DoAction(Vector3 previous, Vector3 targetPoisition)
    {
        bool isOutOfBounds = targetPoisition.x < 0 || targetPoisition.y < 0 || targetPoisition.x >= _width || targetPoisition.y >= _height;

        if (isOutOfBounds)
        {
            Debug.Log("GameHandler.cs error DoAction() out of bounds x,y = " + targetPoisition.x + "," + targetPoisition.y);
            return;
        }
        TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;
        if (TurnHandler.CurrentUnit.CanMove)
        {
            _moveAction.Setup(TurnHandler.CurrentUnit);
            TurnHandler.CurrentUnit.UpdateWalkVariableClientRpc();
            _moveAction.Move();
        } 
        else 
        if (TurnHandler.CurrentUnit.CanShoot)
        {
            _attackAction.Setup(TurnHandler.CurrentUnit);
            _attackAction.Attack();
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
        BaseUnit unit;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetUnit(BaseUnit unit)
        {
            this.unit = unit;
            Instance.GetPathfindingGrid().GetGrid().GetGridObject(x, y).SetWalkable(false);
        }

        public BaseUnit GetUnit()
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
