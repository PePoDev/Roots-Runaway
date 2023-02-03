using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField displayNameInputField;

    private void Start()
    {
        PlayerPrefs.GetString("PlayerName");
    }

    public void OnHostClicked()
    {
        //PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        Debug.Log("OnHostClicked");
        GameNetPortal.Instance.StartHost();
    }

    public void OnClientClicked()
    {
        //PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        Debug.Log("OnClientClicked");
        ClientGameNetPortal.Instance.StartClient();
    }
}


