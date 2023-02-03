using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Unity.Collections;
using TMPro;

public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
{
    public ulong ClientId;
    public bool IsReady;
    public FixedString32Bytes PlayerName;
    public ulong SeletedCharacterId;

    public LobbyPlayerState(ulong clientId, bool isReady, FixedString32Bytes playerName, ulong seletedCharacterId)
    {
        ClientId = clientId;
        IsReady = isReady;
        PlayerName = playerName;
        SeletedCharacterId = seletedCharacterId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref IsReady);
        serializer.SerializeValue(ref PlayerName);
    }

    public bool Equals(LobbyPlayerState other)
    {
        return ClientId == other.ClientId && IsReady == other.IsReady && PlayerName.Equals(other.PlayerName);
    }
}


public class LobbyUI : NetworkBehaviour
{
    public LobbyPlayerCard[] lobbyPlayerCards;
    public Toggle isReadyToggle;
    public TMP_Text timeCounter;

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
        if (lobbyPlayers.Count < 4)
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
            false,
            playerData.Value.PlayerName,
            playerData.Value.SeletedCharacterId
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
                    !lobbyPlayers[i].IsReady,
                    lobbyPlayers[i].PlayerName,
                    lobbyPlayers[i].SeletedCharacterId
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

    private void HandleLobbyPlayersStateChanged(NetworkListEvent<LobbyPlayerState> lobbyState)
    {
        for (int i = 0; i < lobbyPlayerCards.Length; i++)
        {
            if (lobbyPlayers.Count > i)
            {
                lobbyPlayerCards[i].UpdateDisplay(lobbyPlayers[i]);
            }
            else
            {
                lobbyPlayerCards[i].DisableDisplay();
            }
        }

        if (IsEveryoneReady())
        {
            isReadyToggle.interactable = false;
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        for (int i = 0; i < 5; i++)
        {
            timeCounter.text = (5-i).ToString();
            yield return new WaitForSeconds(1f);
        }

        StartGameServerRpc();
    }
}

