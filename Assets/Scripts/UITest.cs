using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UITest : MonoBehaviour
{
    [SerializeField] Button startClientButton;
    [SerializeField] Button startHostButton;
    // Start is called before the first frame update
    void Start()
    {
        startClientButton.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); });
        startHostButton.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); });
    }
}
