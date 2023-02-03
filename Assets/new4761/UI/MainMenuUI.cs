using TMPro;
using UnityEngine;

    // TODO Maybe this one is not need 
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
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);

            GameNetPortal.Instance.StartHost();
        }

        public void OnClientClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);

            ClientGameNetPortal.Instance.StartClient();
        }
    }


