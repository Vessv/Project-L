using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class UIManager : NetworkBehaviour
{
    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TMP_InputField joinCodeInput;

    [SerializeField] 
    private TMP_InputField displayNameInputField;




    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.GetString("PlayerName");

        startHostButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.instance.isRelayEnabled) await RelayManager.instance.SetupRelay();
            //PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            if (NetworkManager.Singleton.StartHost()) NetworkLog.LogInfoServer("Host Started");
            //GameNetPortal.Instance.StartHost();
        });

        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.instance.isRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text)) 
                await RelayManager.instance.JoinRelay(joinCodeInput.text);
            //PlayerPrefs.SetString("PlayerName", displayNameInputField.text);

            //ClientGameNetPortal.Instance.StartClient();

            if (NetworkManager.Singleton.StartClient()) NetworkLog.LogInfoServer("Client Joined");
        });
    }

}
