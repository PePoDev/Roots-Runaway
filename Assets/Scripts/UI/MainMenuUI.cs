using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField displayNameInputField;
    public TMP_InputField keyCodeInputField;
    public GameObject playerObject;
    private GameObject currentModel;

    public Button createButton;
    public Button joinButton;
    public Button quickPlayButton;
    public GameObject[] AllCharacterModel;

    public GameObject lobbyManager;
    private int selectedCharacterID = 0;

    private void Start()
    {
        currentModel = Instantiate(playerObject, transform.position, transform.rotation) as GameObject;
        currentModel.transform.parent = transform;
    }

    public void OnHostClicked()
    {
        PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        PlayerPrefs.SetInt("SeletedCharacterId", selectedCharacterID);
        GameNetPortal.Instance.StartHost();
    }

    public void OnClientClicked()
    {
        PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
        PlayerPrefs.SetInt("SeletedCharacterId", selectedCharacterID);
        ClientGameNetPortal.Instance.StartClient();
    }

    public void OnClientClickPlusCharacterID() {
        if (selectedCharacterID == AllCharacterModel.Length-1) {
            selectedCharacterID = 0;
            return;
        }
        selectedCharacterID++;
        ChangeModel();
        Debug.Log(selectedCharacterID);
    }

    public void OnClientClickMinusCharacterID()
    {
        if (selectedCharacterID == 0)
        {
            selectedCharacterID = AllCharacterModel.Length - 1;
            return;
        }
        selectedCharacterID--;
        ChangeModel();
        Debug.Log(selectedCharacterID);
    }

    public void ChangeModel()
    {
        if (currentModel != AllCharacterModel[selectedCharacterID])
        {
            GameObject newModel = Instantiate(AllCharacterModel[selectedCharacterID], transform.position, transform.rotation) as GameObject;
            Destroy(currentModel);
            newModel.transform.parent = transform;
            currentModel = newModel;
        }

    }

}
