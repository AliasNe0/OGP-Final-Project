using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI;
    private bool lobbyActive = false;

    private void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ShowLobby(true);
        }
    }

    private void ShowLobby(bool showLobby)
    {
        lobbyUI.SetActive(showLobby);
        Cursor.visible = showLobby;
    }

#if UNITY_SERVER && !UNITY_EDITOR
    public void Start()
    {
        NetworkManager.Singleton.StartServer();
    }
#else
    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        Cursor.lockState = CursorLockMode.None;
        ShowLobby(true);
    }

    private void OnClientDisconnectCallback(ulong clientID)
    {
        if (NetworkManager.Singleton.LocalClientId == clientID)
        {
            ShowLobby(true);
            SceneManager.LoadScene(1);
        }
    }

    public void StartHost()
    {
        ShowLobby(false);
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        ShowLobby(false);
        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        ShowLobby(true);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(1);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
        NetworkManager.Singleton.Shutdown();
    }
}
# endif