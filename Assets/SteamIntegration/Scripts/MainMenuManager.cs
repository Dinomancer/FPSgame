using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using GameKit.Dependencies.Utilities;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance;

    [SerializeField] private GameObject menuScreen, lobbyScreen;
    [SerializeField] private TMP_InputField lobbyInput;

    [SerializeField] private TextMeshProUGUI lobbyTitle, lobbyIDText;
    [SerializeField] private Button startGameButton;
    // Add a reference to the copy button
    [SerializeField] private Button copyButton;
    private void Awake() => instance = this;

    private void Start()
    {
        OpenMainMenu();

    // Add click listener for copy button if it exists
        if (copyButton != null)
        {
            copyButton.onClick.AddListener(CopyLobbyIDToClipboard);
        }
    }

      // Add copy functionality
    public void CopyLobbyIDToClipboard()
    {
        if (!string.IsNullOrEmpty(lobbyIDText.text))
        {
            GUIUtility.systemCopyBuffer = lobbyIDText.text;
            Debug.Log("Lobby ID copied to clipboard: " + lobbyIDText.text);
        }
    }


    public void CreateLobby()
    {
        BootstrapManager.CreateLobby();
    }

    public void OpenMainMenu()
    {
        CloseAllScreens();
        menuScreen.SetActive(true);
    }

    public void OpenLobby()
    {
        CloseAllScreens();
        lobbyScreen.SetActive(true);
    }

    public static void LobbyEntered(string lobbyName, bool isHost)
    {
        instance.lobbyTitle.text = lobbyName;
        // temporarilly i set isHost to true
        instance.startGameButton.gameObject.SetActive(isHost); //isHost
        //check isHost
        Debug.Log(isHost);
        if (isHost)
            Debug.Log("Now, u r the Server");
        else Debug.Log("you are client");
        instance.lobbyIDText.text = BootstrapManager.CurrentLobbyID.ToString();
        instance.OpenLobby();
    }

    void CloseAllScreens()
    {
        menuScreen.SetActive(false);
        lobbyScreen.SetActive(false);
    }

    public void JoinLobby()
    {
        CSteamID steamID = new CSteamID(Convert.ToUInt64(lobbyInput.text));
        BootstrapManager.JoinByID(steamID);
    }

    public void LeaveLobby()
    {
        BootstrapManager.LeaveLobby();
        OpenMainMenu();
    }

    public void StartGame()
    {
        string[] scenesToClose = new string[] { "MenuSceneSteam" };
        BootstrapNetworkManager.ChangeNetworkScene("SampleScene", scenesToClose);


        print("switched to samplescene");
    }
}
