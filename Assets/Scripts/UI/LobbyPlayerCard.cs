using TMPro;
using UnityEngine;
using UnityEngine.UI;
public struct PlayerData
{
    public ulong ClientId { get; private set; }
    public string PlayerName { get; private set; }
    public ulong SeletedCharacterId { get; private set; }

    public PlayerData(ulong clientId, string playerName, ulong seletedCharacterId)
    {
        ClientId = clientId;
        PlayerName = playerName;
        SeletedCharacterId = seletedCharacterId;
    }
}

public class LobbyPlayerCard : MonoBehaviour
{
    public GameObject PlayerPositionTemplate;
    public GameObject PlayerParent;
    public TMP_Text playerName;
    public ulong seletedCharacterId;
    public LobbyUI lobby;

    private GameObject player = null;

    public void UpdateDisplay(LobbyPlayerState lobbyPlayerState)
    {
        playerName.text = lobbyPlayerState.PlayerName.ToString();
        playerName.gameObject.SetActive(true);
        playerName.color = lobbyPlayerState.IsReady ? Color.green : Color.white;
        seletedCharacterId = lobbyPlayerState.SeletedCharacterId;

        if (player == null)
        {
            player = Instantiate(lobby.AllCharacterModel[seletedCharacterId], PlayerPositionTemplate.transform.position, PlayerPositionTemplate.transform.rotation, PlayerParent.transform);
            player.transform.localScale = PlayerPositionTemplate.transform.localScale;
        }

        Debug.Log("Name: " + playerName.text + ", SeletedCharacterId: " + seletedCharacterId.ToString() + ", Ready: " + lobbyPlayerState.IsReady);
    }

    public void DisableDisplay()
    {
        playerName.gameObject.SetActive(false);
        if (player != null)
        {
            Destroy(player);
            player = null;
        }
    }
}
