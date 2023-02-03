using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerCard : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    //[SerializeField] private TMP_Text playerDisplayNameText;
    //[SerializeField] private Toggle isReadyToggle;

    public void UpdateDisplay(LobbyPlayerState lobbyPlayerState)
    {
        Debug.Log(lobbyPlayerState.IsReady);
        Player.SetActive(true);
        //playerDisplayNameText.text = lobbyPlayerState.ClientId.ToString();
        //isReadyToggle.isOn = lobbyPlayerState.IsReady;
    }

    public void DisableDisplay()
    {
        Player.SetActive(false);
    }
}
