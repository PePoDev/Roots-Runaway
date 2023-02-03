using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField displayNameInputField;
    public TMP_InputField keyCodeInputField;

    public Button createButton;
    public Button joinButton;
    public Button quickPlayButton;

    public GameObject lobbyManager;

    public void OnHostClicked()
    {
        PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        GameNetPortal.Instance.StartHost();
    }

    public void OnClientClicked()
    {
        PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        ClientGameNetPortal.Instance.StartClient();
    }
}


