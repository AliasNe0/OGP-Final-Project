using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using CMF;

public class MenuButtonController : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject connectedUI;
    [SerializeField] private GameObject disconnectedUI;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !menuUI.active)
        {
            ShowMenu(true);
            NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponent<AdvancedWalkerController>().enabled = false;
            NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponentInChildren<CameraController>().enabled = false;
            NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponent<Rigidbody>().useGravity = true;
        }
    }

    private void ShowMenu(bool showMenu)
    {
        menuUI.SetActive(showMenu);
        Cursor.visible = showMenu;
    }

    public void ContinueGame()
    {
        ShowMenu(false);
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponent<AdvancedWalkerController>().enabled = true;
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponentInChildren<CameraController>().enabled = true;
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponent<Rigidbody>().useGravity = false;
    }

    public void QuitGame()
    {
        Disconnect();
        Debug.Log("QUIT!");
        Application.Quit();
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
        disconnectedUI.SetActive(true);
        connectedUI.SetActive(false);
        ShowMenu(true);
    }

    private void OnClientDisconnectCallback(ulong clientID)
    {
        if (NetworkManager.Singleton.LocalClientId == clientID)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        ShowMenu(false);
        disconnectedUI.SetActive(false);
        connectedUI.SetActive(true);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        ShowMenu(false);
        disconnectedUI.SetActive(false);
        connectedUI.SetActive(true);
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }
}
#endif