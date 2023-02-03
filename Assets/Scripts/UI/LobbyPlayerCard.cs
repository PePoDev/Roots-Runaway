using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerCard : MonoBehaviour
{
    public GameObject Player;
    public TMP_Text playerName;

    public void UpdateDisplay(LobbyPlayerState lobbyPlayerState)
    {
        Debug.Log(lobbyPlayerState.IsReady);
        playerName.text = lobbyPlayerState.PlayerName.ToString();
        Player.SetActive(true);
        playerName.gameObject.SetActive(true);
        playerName.color = lobbyPlayerState.IsReady ? Color.green : Color.white;
    }

    public void DisableDisplay()
    {
        Player.SetActive(false);
        playerName.gameObject.SetActive(false);
    }
}
