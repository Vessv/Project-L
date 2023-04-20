using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class LobbyPlayerCard : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject waitingForPlayerPanel;
    [SerializeField] private GameObject playerDataPanel;

    [Header("Data Display")]
    [SerializeField] private TMP_Text playerDisplayNameText;
    [SerializeField] private Image selectedCharacterImage;
    [SerializeField] private Toggle isReadyToggle;

    [SerializeField] private GameObject heroButtons;
    [SerializeField] private UnitSO[] unitSO;


    public void UpdateDisplay(LobbyPlayerState lobbyPlayerState)
    {
        playerDisplayNameText.text = lobbyPlayerState.PlayerName.ToString();
        isReadyToggle.isOn = lobbyPlayerState.IsReady;

        waitingForPlayerPanel.SetActive(false);
        playerDataPanel.SetActive(true);

        selectedCharacterImage.sprite = unitSO[lobbyPlayerState.SelectedHero].UnitSprite;

    }

    public void DisableDisplay()
    {
        waitingForPlayerPanel.SetActive(true);
        playerDataPanel.SetActive(false);
    }

    public void ActiveButtons()
    {
        heroButtons.SetActive(true);
    }

    public void OnLeftButton()
    {
        transform.root.gameObject.GetComponent<LobbyUI>().OnLeftClicked();
    }

    public void OnRightButton()
    {
        transform.root.gameObject.GetComponent<LobbyUI>().OnRightClicked();
    }

}

