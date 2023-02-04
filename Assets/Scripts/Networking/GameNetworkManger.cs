using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManger : NetworkBehaviour

{
    public GameObject testPrefebs;
    public LobbyUI lobby;

    public NetworkVariable<int> CurrentPlayerLive = new NetworkVariable<int>();

    [SerializeField]
    [Tooltip("A collection of locations for spawning players")]
    private Transform[] m_PlayerSpawnPoints;

    public bool InitialSpawnDone { get; private set; }

    public override void OnNetworkSpawn() {
        CurrentPlayerLive.OnValueChanged += OnSomeValueChanged;

        if (NetworkManager.Singleton.IsServer)
        {
            //PlayerServerCharacter

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                SpawnPlayer(client.ClientId, m_PlayerSpawnPoints[client.ClientId]);
                //var playerData = ServerGameNetPortal.Instance.GetPlayerData(client.ClientId);
                //NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
                //Debug.Log(client.OwnedObjects[0]);
                //var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(client.ClientId);
                //Debug.Log(playerNetworkObject);
                //Debug.Log(playerData.Value.PlayerName);
            }

            CurrentPlayerLive.Value = 4;
        }
        else
        {
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        CurrentPlayerLive.OnValueChanged -= OnSomeValueChanged;

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSomeValueChanged(int previous, int current)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");
    }

    void SpawnPlayer(ulong clientId, Transform pos) {
        GameObject go = Instantiate(testPrefebs,pos.position, pos.rotation);
        Debug.Log("Spawned PlayerID:" + clientId.ToString());
        go.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}
