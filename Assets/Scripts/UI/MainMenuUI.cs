using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField displayNameInputField;
    public TMP_InputField keyCodeInputField;

    public Button createButton;
    public Button joinButton;
    public GameObject[] AllCharacterModel;

    private int selectedCharacterID = 0;

    public void Start()
    {
        selectedCharacterID = UnityEngine.Random.Range(0, 8);
        AllCharacterModel[selectedCharacterID].SetActive(true);

        if (PlayerPrefs.HasKey("PlayerName"))
        {
            displayNameInputField.text = PlayerPrefs.GetString("PlayerName");
        }

        if (PlayerPrefs.HasKey("RoomKey"))
        {
            keyCodeInputField.text = PlayerPrefs.GetString("RoomKey");
        }
    }

    public void OnHostClicked()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                PlayerPrefs.SetString("ServerIP", ip.ToString());
                break;
            }
        }

        PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        PlayerPrefs.SetInt("SeletedCharacterId", selectedCharacterID);
        GameNetPortal.Instance.StartHost();
    }

    public void OnClientClicked()
    {
        var ips = new List<string>();
        for (var i = 0; i < 8; i += 2)
        {
            string hex = keyCodeInputField.text[i].ToString() + keyCodeInputField.text[i + 1].ToString();
            int decValue = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            ips.Add(decValue.ToString());
        }

        string ip = string.Join(".", ips);
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ip;
        
        Debug.Log("IP to join: " + ip);

        PlayerPrefs.SetString("RoomKey", keyCodeInputField.text);
        PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        PlayerPrefs.SetInt("SeletedCharacterId", selectedCharacterID);
        ClientGameNetPortal.Instance.StartClient();
    }

    public void OnClientClickPlusCharacterID()
    {
        AllCharacterModel[selectedCharacterID++].SetActive(false);
        if (selectedCharacterID == AllCharacterModel.Length) selectedCharacterID = 0;
        AllCharacterModel[selectedCharacterID].SetActive(true);
    }

    public void OnClientClickMinusCharacterID()
    {
        AllCharacterModel[selectedCharacterID--].SetActive(false);
        if (selectedCharacterID < 0) selectedCharacterID = AllCharacterModel.Length - 1;
        AllCharacterModel[selectedCharacterID].SetActive(true);
    }
}
