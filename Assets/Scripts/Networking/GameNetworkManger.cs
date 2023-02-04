using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManger : NetworkBehaviour

{
    public GameObject testPrefebs;

    public NetworkVariable<int> CurrentPlayerLive = new NetworkVariable<int>();

    [SerializeField]
    [Tooltip("A collection of locations for spawning players")]
    private Transform[] m_PlayerSpawnPoints;

    public bool InitialSpawnDone { get; private set; }

    public override void OnNetworkSpawn() {
        CurrentPlayerLive.OnValueChanged += OnSomeValueChanged;

        // Player Spawner Manger
        //NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        //NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += OnSynchronizeComplete;   
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        CurrentPlayerLive.OnValueChanged -= OnSomeValueChanged;

    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("NetworkManager.Singleton.IsServer: "+ NetworkManager.Singleton.IsServer.ToString());
        if (NetworkManager.Singleton.IsServer)
        {
            //PlayerServerCharacter

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                Debug.Log(client.ClientId);
                // TODO :: replace it with index
                TestSpawn(client.ClientId, m_PlayerSpawnPoints[client.ClientId]);
                //var playerData = ServerGameNetPortal.Instance.GetPlayerData(client.ClientId);
                //NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
                //Debug.Log(client.OwnedObjects[0]);
                //var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(client.ClientId);
                //Debug.Log(playerNetworkObject);
                //Debug.Log(playerData.Value.PlayerName);
            }

            CurrentPlayerLive.Value = 4;
        }
        else {
        }
        //Debug.Log(NetworkManager.LocalClient.PlayerObject);
        //TestSpawn(NetworkManager.Singleton.LocalClientId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSomeValueChanged(int previous, int current)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previous} | Current: {current}");
    }

    //void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    //{
    //    if (!InitialSpawnDone && loadSceneMode == LoadSceneMode.Single)
    //    {
    //        InitialSpawnDone = true;
    //        foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
    //        {
    //            SpawnPlayer(kvp.Key, false);
    //        }
    //    }
    //}

    //void OnSynchronizeComplete(ulong clientId)
    //{
    //    if (InitialSpawnDone && !PlayerServerCharacter.GetPlayerServerCharacter(clientId))
    //    {
    //        //somebody joined after the initial spawn. This is a Late Join scenario. This player may have issues
    //        //(either because multiple people are late-joining at once, or because some dynamic entities are
    //        //getting spawned while joining. But that's not something we can fully address by changes in
    //        //ServerBossRoomState.
    //        SpawnPlayer(clientId, true);
    //    }
    //}

    void SpawnPlayer(ulong clientId, bool lateJoin) {
    }

    void TestSpawn(ulong clientId, Transform pos) {
        GameObject go = Instantiate(testPrefebs,pos.position, pos.rotation);
        Debug.Log("Spawned PlayerID:" + clientId.ToString());
        go.GetComponent<Serv>().SpawnAsPlayerObject(clientId);
    }
}
