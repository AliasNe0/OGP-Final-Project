using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using CMF;
using UnityEngine.Events;

public class MenuButtonController : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject connectedUI;
    [SerializeField] private GameObject disconnectedUI;
    [SerializeField] private GameObject timerUI;
    [SerializeField] private GameObject timerButton;
    [SerializeField] private UnityEvent StartTimerEvent;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !menuUI.activeSelf)
        {
            ShowMenu(true);
            DisableMovements(true);
        }
    }

    private void DisableMovements(bool disable)
    {
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponent<AdvancedWalkerController>().enabled = !disable;
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponentInChildren<CameraController>().enabled = !disable;
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponent<Rigidbody>().useGravity = disable;
    }

    private void ShowMenu(bool showMenu)
    {
        menuUI.SetActive(showMenu);
        Cursor.visible = showMenu;
    }

    public void StartTimer()
    {
        StartTimerEvent.Invoke();
        timerButton.SetActive(false);
        DisableMovements(false);
    }

    public void HideTimerUI()
    {
        timerUI.SetActive(false);
    }

    public void ContinueGame()
    {
        ShowMenu(false);
        DisableMovements(false);
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
        timerUI.SetActive(false);
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
        menuUI.SetActive(false);
        disconnectedUI.SetActive(false);
        connectedUI.SetActive(true);
        timerUI.SetActive(true);
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponent<AdvancedWalkerController>().enabled = false;
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponentInChildren<CameraController>().enabled = false;
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