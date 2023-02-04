using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManger : NetworkBehaviour

{
    public GameObject playerPrefeb;
    public GameObject[] AllCharacterModel;

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
            //Debug.Log("ServerGameNetPortal: "+ServerGameNetPortal.Instance.AllCharacterModel.Length.ToString());

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
        var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);
        var model = AllCharacterModel[playerData.Value.SeletedCharacterId];


        var newPlayer = Instantiate(playerPrefeb, pos.position, pos.rotation);
        var newPlayerCharacter = Instantiate(model, newPlayer.transform);
        newPlayer.GetComponent<anim>().Anim = newPlayerCharacter.GetComponent<Animator>();
        //newPlayerCharacter.transform.parent = newPlayer.transform;
        Debug.Log("Spawned PlayerID:" + clientId.ToString());
        newPlayerCharacter.GetComponentInParent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}
