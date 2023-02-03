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
    public Button backButton;
    
    public Toggle readyToggle;
    
    public TMP_Text codeText;

    public GameObject lobbyManager;

    public void OnHostClicked()
    {
        PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        GameNetPortal.Instance.StartHost();

        displayNameInputField.gameObject.SetActive(false);
        keyCodeInputField.gameObject.SetActive(false);
        
        createButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        quickPlayButton.gameObject.SetActive(false);

        readyToggle.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        codeText.gameObject.SetActive(true);

        lobbyManager.SetActive(true);

        // TODO: assign code to text
    }

    public void OnClientClicked()
    {
        PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        ClientGameNetPortal.Instance.StartClient();

        displayNameInputField.gameObject.SetActive(false);
        keyCodeInputField.gameObject.SetActive(false);

        createButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        quickPlayButton.gameObject.SetActive(false);

        readyToggle.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        codeText.gameObject.SetActive(true);

        lobbyManager.SetActive(true);
    }

    public void OnBackClicked()
    {
        displayNameInputField.gameObject.SetActive(true);
        keyCodeInputField.gameObject.SetActive(true);

        createButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
        quickPlayButton.gameObject.SetActive(true);

        readyToggle.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        codeText.gameObject.SetActive(false);

        lobbyManager.SetActive(false);
    }
}


