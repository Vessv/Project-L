using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class UIManager : NetworkBehaviour
{
    [SerializeField]
    private Button _startHostButton;

    [SerializeField]
    private Button _startClientButton;

    [SerializeField]
    private TMP_InputField _joinCodeInput;

    [SerializeField] 
    private TMP_InputField _displayNameInputField;




    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.GetString("PlayerName");

        _startHostButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.isRelayEnabled) await RelayManager.Instance.SetupRelay();
            //PlayerPrefs.SetString("PlayerName", _displayNameInputField.text);
            if (NetworkManager.Singleton.StartHost()) NetworkLog.LogInfoServer("Host Started");
            //GameNetPortal.Instance.StartHost();
        });

        _startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.isRelayEnabled && !string.IsNullOrEmpty(_joinCodeInput.text)) 
                await RelayManager.Instance.JoinRelay(_joinCodeInput.text);
            //PlayerPrefs.SetString("PlayerName", _displayNameInputField.text);

            //ClientGameNetPortal.Instance.StartClient();

            if (NetworkManager.Singleton.StartClient()) NetworkLog.LogInfoServer("Client Joined");
        });
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
