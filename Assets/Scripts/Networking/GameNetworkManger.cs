using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManger : NetworkBehaviour

{
    public GameObject testPrefebs;

    public GameObject[] itemsPrefebs;
    public Transform[] itemsSpawnPoints;
    private int maxItem;

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
            maxItem = itemsPrefebs.Length;
            StartCoroutine(SpawnItem());

            StartCoroutine(waitPlayer());
            IEnumerator waitPlayer()
            {
                yield return new WaitForSeconds(3f);
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
                InitialSpawnDone = true;
                CurrentPlayerLive.Value = 4;
            }

            

            //NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += OnSynchronizeComplete;
        }
        else
        {
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            CurrentPlayerLive.OnValueChanged -= OnSomeValueChanged;
        }
        base.OnNetworkDespawn();

    }

    ///// <summary>
    ///// Client and Server side
    ///// INetworkPrefabInstanceHandler.Destroy implementation
    ///// </summary>
    //public void Destroy(NetworkObject networkObject)
    //{
    //   Debug.Log("Destroy");    
    //}
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

    private IEnumerator SpawnItem(){
        while (true) {
            yield return new WaitForSeconds(5f + Random.Range(0, 5f));
            TestSpawnItemsClientRpc(Random.Range(0, maxItem), Random.Range(0, itemsSpawnPoints.Length));
        }
    }

    [ClientRpc]
    private void TestSpawnItemsClientRpc(int  item, int sp) {
        GameObject obect = Instantiate(itemsPrefebs[item], itemsSpawnPoints[sp].transform.position, itemsSpawnPoints[sp].transform.rotation);

        Debug.Log(obect.tag);
        //obect.GetComponent<NetworkObject>().Spawn();
    }

    void OnSynchronizeComplete(ulong clientId)
    {
        if (InitialSpawnDone && !ServerGameNetPortal.Instance.GetPlayerData(clientId).HasValue)
        {
            //somebody joined after the initial spawn. This is a Late Join scenario. This player may have issues
            //(either because multiple people are late-joining at once, or because some dynamic entities are
            //getting spawned while joining. But that's not something we can fully address by changes in
            //ServerBossRoomState.
            SpawnPlayer(clientId, m_PlayerSpawnPoints[clientId]);
        }
    }

}
