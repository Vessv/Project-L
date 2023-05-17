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
    GameObject [] _demonPrefabs;

    [SerializeField]
    GameObject[] _skeletonPrefabs;

    [SerializeField]
    GameObject[] _orcPrefabs;

    [SerializeField]
    GameObject[] _otherPrefabs;

    [SerializeField]
    GameObject _enemyPrefab;

    //Turn vars
    [SerializeField]
    bool[] _hasBeenRegistered = new bool[5];

    bool hasPlayers => NetworkManager.Singleton.ConnectedClientsList.Count > 0;

    public NetworkVariable<int> floorNumber;

    [SerializeField]
    MoveAction _moveAction;
    [SerializeField]
    AttackAction _attackAction;

    [SerializeField]
    ActionSO[] _actionsSOArray;

    [SerializeField]
    Item[] _itemsSOArray;

    public GameObject[] ProjectileSOArray;


    public UnitSO[] UnitSOArray;

    //floor things
    public GameObject[] GroundVariations;
    public GameObject[] TopwallVariations;
    public GameObject[] botwallVariations;

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
            player.GetComponent<PlayerUnit>().ownedActionList.Add(18);
            player.GetComponent<PlayerUnit>().ownedActionList.Add(1);

            //habilidades inciailes de cada heore e items si es necesario
            switch (ServerGameNetPortal.Instance.choosenHero[client.ClientId])
            {
                case 0:
                    player.GetComponent<PlayerUnit>().ownedActionList.Add(2);
                    player.GetComponent<PlayerUnit>().ownedActionList.Add(11);
                    client.PlayerObject.GetComponent<PlayerUnit>().ActionPoints.Value = 3;

                    break;
                case 1:
                    player.GetComponent<PlayerUnit>().ownedActionList.Add(3);
                    player.GetComponent<PlayerUnit>().ownedActionList.Add(13);
                    client.PlayerObject.GetComponent<PlayerUnit>().ActionPoints.Value = 3;


                    break;
                case 2:
                    player.GetComponent<PlayerUnit>().ownedActionList.Add(4);
                    player.GetComponent<PlayerUnit>().ownedActionList.Add(8);
                    client.PlayerObject.GetComponent<PlayerUnit>().ActionPoints.Value = 2;

                    break;
            }

        }
        StopSoundToAllPlayers("music2", true);
        PlaySoundToAllPlayers("music", true);

        GameObject enemy = Instantiate(_skeletonPrefabs[0]);
        enemy.GetComponent<NetworkObject>().Spawn();
        EnemyList.Add(enemy.GetComponent<NPCUnit>());
        enemy.transform.position = new Vector3(6.5f, 12.5f);
        Instance.GetGrid().GetGridObject(new Vector3(6.5f, 12.5f)).SetUnit(enemy.GetComponent<NPCUnit>());
        GameObject enemy2 = Instantiate(_skeletonPrefabs[0]);
        enemy2.GetComponent<NetworkObject>().Spawn();
        EnemyList.Add(enemy2.GetComponent<NPCUnit>());
        enemy2.transform.position = new Vector3(11.5f, 12.5f);
        Instance.GetGrid().GetGridObject(new Vector3(11.5f, 12.5f)).SetUnit(enemy2.GetComponent<NPCUnit>());


    }

    public void PlaySoundToAllPlayers(string name, bool instant = false)
    {
        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.GetComponent<PlayerUnit>().PlaySoundClientRpc(name, instant);
        }
    }

    public void StopSoundToAllPlayers(string name, bool instant = false)
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.GetComponent<PlayerUnit>().StopSoundClientRpc(name, instant);
        }
    }

    public void RemoveEnemyFromList(NPCUnit unit)
    {
        if (!IsServer) { return; }
        EnemyList.Remove(unit);
        if(EnemyList.Count == 0)
        {
            StartCoroutine(PlaySoundStair());
            if (TurnHandler.hasPlayersCycleBeenDone)
            {
                TurnHandler.hasPlayersCycleBeenDone = false;
                TurnHandler.CurrentTurnIndex = -1;
                TurnHandler.NextTurn();

            }
            FloorEnd();
        }
    }

    IEnumerator PlaySoundStair()
    {
        PlaySoundToAllPlayers("stair");
        yield return new WaitForSeconds(1.5f);
        StopSoundToAllPlayers("stair");
        yield break;

    }

    public void EndGame(bool previous, bool current)
    {
        if (!current) return;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (!client.PlayerObject.gameObject.GetComponent<PlayerUnit>().IsDead.Value)
            {
                return;
            }
        }

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.gameObject.GetComponent<PlayerUnit>().ActiveGameEndUIClientRpc();
        }
    }

    public void FloorEnd()
    {
        floorNumber.Value += 1;
        //enable ui wtih blessing so they choose
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.gameObject.GetComponent<PlayerUnit>().IsDead.Value)
            {
                client.PlayerObject.gameObject.GetComponent<PlayerUnit>().CurrentHealth.Value = 80;
                client.PlayerObject.gameObject.GetComponent<PlayerUnit>().IsDead.Value = false;

            }
            client.PlayerObject.gameObject.GetComponent<PlayerUnit>().NextFloorUIClientRpc();
            client.PlayerObject.gameObject.GetComponent<PlayerUnit>().DisplayBlessingSelectionClientRpc();
            client.PlayerObject.gameObject.GetComponent<PlayerUnit>().FloorNumber = floorNumber.Value;
        }
        //after everyone has choosen black screen algo como player.changeblackscreenclientrpc
        //move to initial position
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList) //mirar si esto si funciona
        {
            GameObject player = client.PlayerObject.gameObject;
            _gameGrid.GetGridObject(player.transform.position).RemoveUnit();
            player.transform.position = new Vector3((float)(client.ClientId + 8.5f), 1.5f);
            _gameGrid.GetGridObject(player.transform.position).SetUnit(player.GetComponent<BaseUnit>());
        }

        //change map spawnear cosas, tenerlos en una lista, updatear el pathfinding si ese necesario, remover los anteriores, updatear el pathfinding
        //spawn new enemies, de una lista que tenga waves de enemigos o algo, o que lo haga el turn handler mejor
        //TurnHandler.NextTurn();

        SpawnNewWave();
    }


    void UpdateBotWall(int index, bool active)
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.GetComponent<PlayerUnit>().UpdateTilesetBotWallClientRpc(index, active);
        }
    }

    void UpdateTopWall(int index, bool active)
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.GetComponent<PlayerUnit>().UpdateTilesetTopWallClientRpc(index, active);
        }
    }

    void UpdateGround(int index, bool active)
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.GetComponent<PlayerUnit>().UpdateTilesetGroundClientRpc(index, active);
        }
    }

    public void SpawnNewWave()
    {
        if(floorNumber.Value == 1)
        {
            UpdateBotWall(0, false);
            UpdateBotWall(1, true);
            //botwallVariations[0].SetActive(false);
            //botwallVariations[1].SetActive(true);
            _pathfindingGrid.GetGrid().GetGridObject(7, 0).SetMap(false);
            _pathfindingGrid.GetGrid().GetGridObject(8, 0).SetMap(false);
            _pathfindingGrid.GetGrid().GetGridObject(9, 0).SetMap(false);
            _pathfindingGrid.GetGrid().GetGridObject(10, 0).SetMap(false);
            _pathfindingGrid.GetGrid().GetGridObject(6, 2).SetMap(false);
            _pathfindingGrid.GetGrid().GetGridObject(11, 2).SetMap(false);
        }

        UpdateTopWall(0, false);
        UpdateTopWall(1, false);
        UpdateTopWall(2, false);
        //TopwallVariations[0].SetActive(false);
        //TopwallVariations[1].SetActive(false);
        //TopwallVariations[2].SetActive(false);

        for(int i = 0; i < GroundVariations.Length; i++)
        {
            UpdateGround(i,false);
        }

        /*foreach (GameObject groundVariation in GroundVariations)
        {
            groundVariation.SetActive(false);
        }*/

        for(int x = 0; x < _width; x++)
        {
            for(int y=0; y<_height; y++)
            {
                _pathfindingGrid.GetGrid().GetGridObject(x, y).SetMap(false);

            }
        }

        //activating variations and spawning enemies
        _pathfindingGrid.GetGrid().GetGridObject(6, 15).SetMap(true);
        _pathfindingGrid.GetGrid().GetGridObject(11, 15).SetMap(true);
        int floorType = (int)Mathf.Floor(UnityEngine.Random.Range(1f, 3.9f));
        int groundVariationIndex = 0;
        switch (floorType)
        {
            case 1: //Undead
                groundVariationIndex = (int)Mathf.Floor(UnityEngine.Random.Range(1f, 3.9f)) -1;
                UpdateGround(6 + groundVariationIndex, true);
                //GroundVariations[6+groundVariationIndex].SetActive(true);
                UpdateTopWall(0,true);
                //TopwallVariations[0].SetActive(true);
                _pathfindingGrid.GetGrid().GetGridObject(2, 15).SetMap(true);
                _pathfindingGrid.GetGrid().GetGridObject(5, 15).SetMap(true);
                _pathfindingGrid.GetGrid().GetGridObject(15, 15).SetMap(true);
                _pathfindingGrid.GetGrid().GetGridObject(12, 15).SetMap(true);


                switch (groundVariationIndex)
                {
                    case 0:
                        break;
                    case 1:
                        _pathfindingGrid.GetGrid().GetGridObject(3, 8).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(4, 8).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(16, 10).SetMap(true);

                        break;
                    case 2:
                        _pathfindingGrid.GetGrid().GetGridObject(1, 4).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(2, 5).SetMap(true);

                        break;
                }
                for (int i = 0; i < 2 + floorNumber.Value; i++)
                {
                    int skIndex = (int)Mathf.Floor(UnityEngine.Random.Range(0f, 1.9f));
                    float randomX = Mathf.Floor((UnityEngine.Random.Range(6f, 11.9f)));
                    float randomY = Mathf.Floor(UnityEngine.Random.Range(9f, 14.9f));
                    if(GetGrid().GetGridObject((int)randomX,(int)randomY).GetUnit() != null)
                    {
                        i--;
                        continue;
                    }
                    else
                    {
                        SpawnEnemy(_skeletonPrefabs[skIndex], new Vector3(randomX, randomY));
                    }
                }
                


                break;
            case 2: //Orc
                groundVariationIndex = (int)Mathf.Floor(UnityEngine.Random.Range(1f, 3.9f)) -1;
                UpdateGround(groundVariationIndex, true);
                //GroundVariations[groundVariationIndex].SetActive(true);
                UpdateTopWall(1, true);
                //TopwallVariations[1].SetActive(true);
                switch (groundVariationIndex)
                {
                    case 0:
                        _pathfindingGrid.GetGrid().GetGridObject(4, 9).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(5, 10).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(6, 10).SetMap(true);
                        break;
                    case 1:
                        _pathfindingGrid.GetGrid().GetGridObject(2, 8).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(16, 10).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(15, 8).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(14, 9).SetMap(true);
                        break;
                    case 2:
                        _pathfindingGrid.GetGrid().GetGridObject(0, 15).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(1, 15).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(15, 15).SetMap(true);

                        break;
                }
                for (int i = 0; i < 1 + floorNumber.Value; i++)
                {
                    int orcIndex = (int)Mathf.Floor(UnityEngine.Random.Range(0f, 3.9f));
                    float randomX = Mathf.Floor((UnityEngine.Random.Range(6f, 11.9f)));
                    float randomY = Mathf.Floor(UnityEngine.Random.Range(9f, 14.9f));
                    if (GetGrid().GetGridObject((int)randomX, (int)randomY).GetUnit() != null)
                    {
                        i--;
                        continue;
                    }
                    else
                    {
                        if(floorNumber.Value <= 4 && orcIndex >= 3)
                        {
                            i--;
                            continue;
                        }
                        SpawnEnemy(_orcPrefabs[orcIndex], new Vector3(randomX, randomY));
                    }
                }

                break;
            case 3: //Demon
                groundVariationIndex = (int)Mathf.Floor(UnityEngine.Random.Range(1f, 3.9f)) -1;
                UpdateGround(3+groundVariationIndex, true);
                //GroundVariations[3+groundVariationIndex].SetActive(true);
                UpdateTopWall(2, true);
                //TopwallVariations[2].SetActive(true);
                _pathfindingGrid.GetGrid().GetGridObject(1, 15).SetMap(true);
                _pathfindingGrid.GetGrid().GetGridObject(5, 15).SetMap(true);
                _pathfindingGrid.GetGrid().GetGridObject(16, 15).SetMap(true);
                _pathfindingGrid.GetGrid().GetGridObject(12, 15).SetMap(true);

                switch (groundVariationIndex)
                {
                    case 0:
                        _pathfindingGrid.GetGrid().GetGridObject(3, 7).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(15, 10).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(16, 11).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(14, 12).SetMap(true);

                        break;
                    case 1:
                        _pathfindingGrid.GetGrid().GetGridObject(4, 9).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(15, 11).SetMap(true);
                        _pathfindingGrid.GetGrid().GetGridObject(13, 10).SetMap(true);

                        break;
                    case 2:
                        break;
                }

                for (int i = 0; i < 1 + floorNumber.Value; i++)
                {
                    int demIndex = (int)Mathf.Floor(UnityEngine.Random.Range(0f, 3.9f));
                    float randomX = Mathf.Floor((UnityEngine.Random.Range(6f, 11.9f)));
                    float randomY = Mathf.Floor(UnityEngine.Random.Range(9f, 14.9f));
                    if (GetGrid().GetGridObject((int)randomX, (int)randomY).GetUnit() != null)
                    {
                        i--;
                        continue;
                    }
                    else
                    {
                        if (floorNumber.Value <= 4 && demIndex >= 3)
                        {
                            i--;
                            continue;
                        }
                        SpawnEnemy(_demonPrefabs[demIndex], new Vector3(randomX, randomY));
                    }
                }

                break;
        }
    }

    void SpawnEnemy(GameObject enemyPrefab, Vector3 pos)
    {
        pos += new Vector3(0.5f, 0.5f);
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.GetComponent<NetworkObject>().Spawn();
        EnemyList.Add(enemy.GetComponent<NPCUnit>());
        enemy.transform.position = pos;
        Instance.GetGrid().GetGridObject(pos).SetUnit(enemy.GetComponent<NPCUnit>());
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
        _pathfindingGrid.GetGrid().GetGridObject(7, 0).SetMap(true);
        _pathfindingGrid.GetGrid().GetGridObject(8, 0).SetMap(true);
        _pathfindingGrid.GetGrid().GetGridObject(9, 0).SetMap(true);
        _pathfindingGrid.GetGrid().GetGridObject(10, 0).SetMap(true);
        _pathfindingGrid.GetGrid().GetGridObject(6, 2).SetMap(true);
        _pathfindingGrid.GetGrid().GetGridObject(11, 2).SetMap(true);
        _pathfindingGrid.GetGrid().GetGridObject(11, 2).SetMap(true);

        _pathfindingGrid.GetGrid().GetGridObject(6, 15).SetMap(true);
        _pathfindingGrid.GetGrid().GetGridObject(11, 15).SetMap(true);

        _itemsSOArray = _itemsSOArray.OrderBy(x => x.itemID).ToArray();

    }


    // Update is called once per frame
    void Update()
    {
        if (!IsServer || !hasPlayers) return;

        if (!_hasBeenRegistered[TurnHandler.CurrentTurnIndex] && TurnHandler.CurrentUnit.UnitScriptableObject.UnitFaction == UnitSO.Faction.Hero)
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
        PlayerUnit playerUnit = (PlayerUnit)unit;
        playerUnit.IsDead.OnValueChanged += EndGame;
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
            case UnitAction.Action.Meele:
                _attackAction.Setup(TurnHandler.CurrentUnit);
                _attackAction.ShowMoveTiles();
                break;
            case UnitAction.Action.Ranged:
                GetComponent<RangedAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<RangedAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Magic:
                GetComponent<MagicAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<MagicAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.ShieldBash:
                GetComponent<ShieldBashAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<ShieldBashAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Fireball:
                GetComponent<FireballAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<FireballAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Headbutt:
                GetComponent<HeadbuttAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<HeadbuttAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Meteor:
                GetComponent<MeteorRainAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<MeteorRainAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Poison:
                GetComponent<PoisonAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<PoisonAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Stun:
                GetComponent<StunAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<StunAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Holy:
                GetComponent<HolyStrikeAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<HolyStrikeAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Heal:
                GetComponent<HealAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<HealAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Tree:
                GetComponent<HolyTreeAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<HolyTreeAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Taunt:
                GetComponent<TauntAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<TauntAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Ignite:
                GetComponent<IgniteAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<IgniteAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Cleave:
                GetComponent<CleaveAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<CleaveAction>().ShowMoveTiles();
                break;
            case UnitAction.Action.Mist:
                GetComponent<PoisonMistAction>().Setup(TurnHandler.CurrentUnit);
                GetComponent<PoisonMistAction>().ShowMoveTiles();
                break;
        }
    }

    public void DoAction(Vector3 previous, Vector3 targetPoisition)
    {
        bool isOutOfBounds = targetPoisition.x < 0 || targetPoisition.y < 0 || targetPoisition.x >= _width || targetPoisition.y >= _height;

        if (isOutOfBounds)
        {
            Debug.Log("GameHandler.cs error DoAction() out of bounds x,y = " + targetPoisition.x + "," + targetPoisition.y);
            TurnHandler.CurrentUnit.SelectedAction.Value = UnitAction.Action.None;
            return;
        }
        TurnHandler.CurrentUnit.Threat.Value += 50; //esto se puede cambiar
        if (TurnHandler.CurrentUnit.CanMove)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            _moveAction.Setup(TurnHandler.CurrentUnit);
            TurnHandler.CurrentUnit.UpdateWalkVariableClientRpc(true);
            _moveAction.Move();
        } 
        else 
        if (TurnHandler.CurrentUnit.CanMeele)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            _attackAction.Setup(TurnHandler.CurrentUnit);
            _attackAction.Attack();
        }
        else
        if (TurnHandler.CurrentUnit.CanRanged)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<RangedAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<RangedAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanMagic)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<MagicAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<MagicAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanShieldBash)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<ShieldBashAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<ShieldBashAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanFireball)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<FireballAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<FireballAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanHeadbutt)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<HeadbuttAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<HeadbuttAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanMeteor)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<MeteorRainAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<MeteorRainAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanPoison)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<PoisonAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<PoisonAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanStun)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<StunAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<StunAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanHoly)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<HolyStrikeAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<HolyStrikeAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanHeal)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<HealAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<HealAction>().Heal();
        }
        else if (TurnHandler.CurrentUnit.CanTree)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<HolyTreeAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<HolyTreeAction>().Heal();
        }
        else if (TurnHandler.CurrentUnit.CanTaunt)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<TauntAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<TauntAction>().Taunt();
        }
        else if (TurnHandler.CurrentUnit.CanIgnite)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<IgniteAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<IgniteAction>().Ignite();
        }
        else if (TurnHandler.CurrentUnit.CanCleave)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<CleaveAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<CleaveAction>().Attack();
        }
        else if (TurnHandler.CurrentUnit.CanMist)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<PoisonMistAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<PoisonMistAction>().Posion();
        }
        else if (TurnHandler.CurrentUnit.CanSkip)
        {
            TurnHandler.CurrentUnit.ActionStatus.Value = BaseUnit.ActionState.Busy;

            GetComponent<SkipAction>().Setup(TurnHandler.CurrentUnit);
            GetComponent<SkipAction>().Skip();
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
