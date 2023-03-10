using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectReason
{
    public ConnectStatus Reason { get; private set; } = ConnectStatus.Undefined;

    public void SetDisconnectReason(ConnectStatus reason)
    {
        Reason = reason;
    }

    public void Clear()
    {
        Reason = ConnectStatus.Undefined;
    }

    public bool HasTransitionReason => Reason != ConnectStatus.Undefined;
}


[Serializable]
public class ConnectionPayload
{
    public string clientGUID;
    public int clientScene = -1;
    public string playerName;
    public int seletedCharacterId = 0;

}

public enum ConnectStatus
{
    Undefined,
    Success,
    ServerFull,
    GameInProgress,
    LoggedInAgain,
    UserRequestedDisconnect,
    GenericDisconnect
}

public class GameNetPortal : MonoBehaviour
{
    public static GameNetPortal Instance => instance;
    private static GameNetPortal instance;

    public event Action OnNetworkReadied;

    public event Action<ConnectStatus> OnConnectionFinished;
    public event Action<ConnectStatus> OnDisconnectReasonReceived;

    public event Action<ulong, int> OnClientSceneChanged;

    public event Action OnUserDisconnectRequested;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleNetworkReady;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= HandleNetworkReady;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;

            if (NetworkManager.Singleton.SceneManager != null)
            {
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;
            }

            if (NetworkManager.Singleton.CustomMessagingManager == null) { return; }

            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ServerToClientConnectResult");
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ServerToClientSetDisconnectReason");
        }
    }

    public void StartHost()
    {
        Debug.Log("StartHost");
        
        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ServerToClientConnectResult", (senderClientId, reader) =>
        {
            reader.ReadValueSafe(out ConnectStatus status);
            OnConnectionFinished?.Invoke(status);
        });

        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ServerToClientSetDisconnectReason", (senderClientId, reader) =>
        {
            reader.ReadValueSafe(out ConnectStatus status);
            OnDisconnectReasonReceived?.Invoke(status);
        });
    }

    public void RequestDisconnect()
    {
        Debug.Log("RequestDisconnect");
        OnUserDisconnectRequested?.Invoke();
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) { return; }

        HandleNetworkReady();
        NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;
    }

    private void HandleSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;

        OnClientSceneChanged?.Invoke(sceneEvent.ClientId, SceneManager.GetSceneByName(sceneEvent.SceneName).buildIndex);
    }

    private void HandleNetworkReady()
    {
        Debug.Log("HandleNetworkReady");
        if (NetworkManager.Singleton.IsHost)
        {
            OnConnectionFinished?.Invoke(ConnectStatus.Success);
        }

        OnNetworkReadied?.Invoke();
    }

    public void ServerToClientConnectResult(ulong netId, ConnectStatus status)
    {
        var writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ServerToClientConnectResult", netId, writer);
    }

    public void ServerToClientSetDisconnectReason(ulong netId, ConnectStatus status)
    {
        var writer = new FastBufferWriter(sizeof(ConnectStatus), Allocator.Temp);
        writer.WriteValueSafe(status);
        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ServerToClientSetDisconnectReason", netId, writer);
    }
}

