using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class MainMenuUI : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField displayNameInputField;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TMP_InputField joinCodeInput;





    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.GetString("PlayerName");

        startHostButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.instance.isRelayEnabled) await RelayManager.instance.SetupRelay();
            //if (NetworkManager.Singleton.StartHost()) NetworkLog.LogInfoServer("Host Started");
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);

            GameNetPortal.Instance.StartHost();
        });

        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.instance.isRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
                await RelayManager.instance.JoinRelay(joinCodeInput.text);
            //if (NetworkManager.Singleton.StartClient()) NetworkLog.LogInfoServer("Client Joined");


            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);

            ClientGameNetPortal.Instance.StartClient();

        });
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