using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LobbyUI : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private LobbyPlayerCard[] lobbyPlayerCards;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI joinCodeString;
    private int selectedHeroIndex = 0;
    private int maxHeroes = 2;

    private NetworkList<LobbyPlayerState> lobbyPlayers;

    private void Awake()
    {
        lobbyPlayers = new NetworkList<LobbyPlayerState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyPlayers.OnListChanged += HandleLobbyPlayersStateChanged;
        }

        if (IsServer)
        {
            startGameButton.gameObject.SetActive(true);

            joinCodeString.text = RelayManager.Instance.joinCodeString;

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        lobbyPlayers.OnListChanged -= HandleLobbyPlayersStateChanged;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    private bool IsEveryoneReady()
    {
        if (lobbyPlayers.Count < 1)
        {
            return false;
        }

        foreach (var player in lobbyPlayers)
        {
            if (!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    private void HandleClientConnected(ulong clientId)
    {
        var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);

        if (!playerData.HasValue) { return; }

        lobbyPlayers.Add(new LobbyPlayerState(
            clientId,
            playerData.Value.PlayerName,
            false,
            0
        ));
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == clientId)
            {
                lobbyPlayers.RemoveAt(i);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                lobbyPlayers[i] = new LobbyPlayerState(
                    lobbyPlayers[i].ClientId,
                    lobbyPlayers[i].PlayerName,
                    !lobbyPlayers[i].IsReady,
                    lobbyPlayers[i].SelectedHero
                );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId != NetworkManager.Singleton.LocalClientId) { return; }

        if (!IsEveryoneReady()) { return; }

        ServerGameNetPortal.Instance.StartGame();
    }

    public void OnLeaveClicked()
    {
        GameNetPortal.Instance.RequestDisconnect();
    }

    public void OnReadyClicked()
    {
        ToggleReadyServerRpc();
    }

    public void OnStartGameClicked()
    {
        StartGameServerRpc();
    }

    public void OnLeftClicked()
    {
        selectedHeroIndex -= 1;
        if(selectedHeroIndex < 0)
        {
            selectedHeroIndex = maxHeroes;
        }
        UpdateHeroImageServerRpc((int)NetworkManager.Singleton.LocalClientId, selectedHeroIndex);
    }

    public void OnRightClicked()
    {
        selectedHeroIndex += 1;
        if(selectedHeroIndex > maxHeroes)
        {
            selectedHeroIndex = 0;
        }
        UpdateHeroImageServerRpc((int)NetworkManager.Singleton.LocalClientId, selectedHeroIndex);
    }


    [ServerRpc(RequireOwnership = false)]
    private void UpdateHeroImageServerRpc(int clientId,int index)
    {
        lobbyPlayers[clientId] = new LobbyPlayerState(
                    lobbyPlayers[clientId].ClientId,
                    lobbyPlayers[clientId].PlayerName,
                    lobbyPlayers[clientId].IsReady,
                    index
                );
        ServerGameNetPortal.Instance.choosenHero[clientId] = lobbyPlayers[clientId].SelectedHero;

    }


    private void HandleLobbyPlayersStateChanged(NetworkListEvent<LobbyPlayerState> lobbyState)
    {
        for (int i = 0; i < lobbyPlayerCards.Length; i++)
        {
            if (lobbyPlayers.Count > i)
            {
                lobbyPlayerCards[i].UpdateDisplay(lobbyPlayers[i]);
                if(i == (int)NetworkManager.Singleton.LocalClientId)
                {
                    lobbyPlayerCards[i].ActiveButtons();
                }
            }
            else
            {
                lobbyPlayerCards[i].DisableDisplay();
            }
        }

        if (IsHost)
        {
            startGameButton.interactable = IsEveryoneReady();
        }
    }

    private void UpdatePlayerCards()
    {
        for (int i = 0; i < lobbyPlayerCards.Length; i++)
        {
            if (lobbyPlayers.Count > i)
            {
                lobbyPlayerCards[i].UpdateDisplay(lobbyPlayers[i]);
                if (i == (int)NetworkManager.Singleton.LocalClientId)
                {
                    lobbyPlayerCards[i].ActiveButtons();
                }
            }
            else
            {
                lobbyPlayerCards[i].DisableDisplay();
            }
        }
    }
}