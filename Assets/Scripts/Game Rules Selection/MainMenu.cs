using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    //public GameObject MenuPanel;
    //public GameObject MatchmakingPanel;
    //public Button FindMatchButton;
    //public Button CancelMatchmakingButton;

    //[Header("Search For Game")]
    //public InputField NameField;
    //public InputField RegionField;
    //public Dropdown PlayersDropdown;
    //public Dropdown ScoreToWinDropdown;
    
    //public GameManager gameManager;

    ///// <summary>
    ///// Called by Unity when this GameObject starts.
    ///// </summary>
    //private void Start()
    //{
    //    if(gameManager == null) gameManager = FindObjectOfType<GameManager>();

    //    if (PlayerPrefs.HasKey("Name"))
    //    {
    //        NameField.text = PlayerPrefs.GetString("Name");
    //    }

    //    // Add event listeners for the menu buttons.
    //    //BackButton.onClick.AddListener(BackFromCredits);
    //    FindMatchButton.onClick.AddListener(FindMatch);
    //    CancelMatchmakingButton.onClick.AddListener(CancelMatchmaking);
    //}

    ///// <summary>
    ///// Called by Unity when this GameObject is being destroyed.
    ///// </summary>
    //private void OnDestroy()
    //{
    //    // Remove event listeners for the menu buttons.
    //    //BackButton.onClick.RemoveListener(BackFromCredits);
    //    FindMatchButton.onClick.RemoveListener(FindMatch);
    //    CancelMatchmakingButton.onClick.RemoveListener(CancelMatchmaking);
    //}
    
    ///// <summary>
    ///// Enables the Find Match button.
    ///// </summary>
    //public void EnableFindMatchButton()
    //{
    //    FindMatchButton.interactable = true;
    //}

    ///// <summary>
    ///// Disables the Find Match button.
    ///// </summary>
    //public void DisableFindMatchButton()
    //{
    //    FindMatchButton.interactable = false;
    //}


    ///// <summary>
    ///// Change State
    ///// </summary>
    ///// <param name="state"></param>
    //public void ChangeGameObjectState(bool state)
    //{
    //    gameObject.SetActive(state);
    //}

    ///// <summary>
    ///// Hides the main menu.
    ///// </summary>
    //public void DeactivateMenu()
    //{
    //    MenuPanel.SetActive(true);
    //    MatchmakingPanel.SetActive(false);
    //    gameObject.SetActive(false);
    //}

    /////// <summary>
    /////// Begins the matchmaking process.
    /////// </summary>
    ////public async void FindMatch()
    ////{
    ////    //MenuPanel.SetActive(false);
    ////    //MatchmakingPanel.SetActive(true);

    ////    //Filling Dummy Data
    ////    PlayerPersonalData.playerName = NameField.text;
    ////    PlayerPersonalData.avatarURL = "https://i.pravatar.cc/300"; //Dummy Image URL
    ////    PlayerPersonalData.location = "USA";

    ////    PlayerPrefs.SetString("Name", NameField.text);
    ////    gameManager.SetDisplayName(NameField.text);

    ////    gameManager.NakamaConnection.UpdatePlayerDataOnNakama();

    ////    await gameManager.NakamaConnection.FindMatch(RegionField.text, double.Parse(ScoreToWinDropdown.options[ScoreToWinDropdown.value].text), int.Parse(PlayersDropdown.options[PlayersDropdown.value].text));
    ////}


    ///// <summary>
    ///// Opens the credits screen.
    ///// </summary>
    //public void GoToCredits()
    //{
    //    MenuPanel.SetActive(false);
    //    MatchmakingPanel.SetActive(false);
    //}

    /////// <summary>
    /////// Goes back to the main menu.
    /////// </summary>
    ////public void BackFromCredits()
    ////{
    ////    MenuPanel.SetActive(true);
    ////    MatchmakingPanel.SetActive(false);
    ////}
}
