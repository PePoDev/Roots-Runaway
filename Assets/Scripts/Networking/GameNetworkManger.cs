using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameNetworkManger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Debug.Log(client.ClientId);
            var playerData = ServerGameNetPortal.Instance.GetPlayerData(client.ClientId);
            Debug.Log(playerData.Value.PlayerName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
