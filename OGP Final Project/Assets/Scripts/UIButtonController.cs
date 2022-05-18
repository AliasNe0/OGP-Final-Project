using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using CMF;
using UnityEngine.Events;
using TMPro;

public class UIButtonController : NetworkBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject connectedUI;
    [SerializeField] private GameObject disconnectedUI;
    [SerializeField] private GameObject firstTimerUI;
    [SerializeField] private GameObject firstTimerButton;
    [SerializeField] private TMP_Text waitingText;
    [SerializeField] private GameObject secondTimerUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private UnityEvent timerButtonEvent;
    [SerializeField] private GameObject menuAudio;
    [SerializeField] private GameObject gameplayAudio;

    public NetworkVariable<bool> gameEnded = new NetworkVariable<bool>();

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !menuUI.activeSelf)
        {
            ShowMenu(true);
            DisableMovements(true);
            firstTimerButton.SetActive(false);
        }
    }

    private void DisableMovements(bool disable)
    {
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponent<AdvancedWalkerController>().enabled = !disable;
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponentInChildren<CameraController>().enabled = !disable;
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponent<Rigidbody>().isKinematic = disable;
    }

    private void ShowMenu(bool showMenu)
    {
        if (!gameEnded.Value)
        {
            menuUI.SetActive(showMenu);
            playerUI.SetActive(!showMenu);
            Cursor.visible = showMenu;
        }
    }

    public void StartGame()
    {
        timerButtonEvent.Invoke();
        firstTimerButton.SetActive(false);
        DisableMovements(false);
        if (IsHost)
            DisableMenuAudioClientRpc();
    }

    [ClientRpc]
    private void DisableMenuAudioClientRpc()
    {
        waitingText.enabled = false;
        menuAudio.SetActive(false);
    }

    public void StartGameplayAudio()
    {
        gameplayAudio.SetActive(true);
    }

    public void HideFirstTimerUI()
    {
        firstTimerUI.SetActive(false);
    }
    public void HideSecondTimerUI()
    {
        secondTimerUI.SetActive(false);
        if (IsHost)
            gameEnded.Value = true;
    }

    public void ContinueGame()
    {
        ShowMenu(false);
        DisableMovements(false);
        firstTimerButton.SetActive(true);
    }

    public void QuitGame()
    {
        Disconnect();
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void ShowResults()
    {
        UpdateWinnerText();
        gameplayUI.SetActive(true);
        DisableMovements(true);
        menuAudio.SetActive(true);
        gameplayAudio.SetActive(false);
        Cursor.visible = true;
        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId).GetComponentInChildren<AudioControl>().enabled = false;
    }

    private void UpdateWinnerText()
    {
        float id = ScoreBoard.Singleton.FindWinner().Key;
        float score = ScoreBoard.Singleton.FindWinner().Value;
        winnerText.text = $"Winner is Player{id} with score {score}";
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
        gameplayAudio.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        disconnectedUI.SetActive(true);
        connectedUI.SetActive(false);
        firstTimerUI.SetActive(false);
        gameplayUI.SetActive(false);
        waitingText.enabled = false;
        ShowMenu(true);
        gameEnded.Value = false;
        playerNameText.text = "";
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
        firstTimerUI.SetActive(true);
        playerUI.SetActive(true);
        DisableMovements(true);
        NetworkObject no = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId);
        playerNameText.text = $"Player{no.GetComponent<PlayerAttributes>().playerID.Value}";
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        disconnectedUI.SetActive(false);
        connectedUI.SetActive(true);
        firstTimerButton.SetActive(false);
        firstTimerUI.SetActive(true);
        waitingText.enabled = true;
        ShowMenu(false);
        StartCoroutine(PlayerNameDelayTimer());
    }

    IEnumerator PlayerNameDelayTimer()
    {
        yield return new WaitForSeconds(0.5f);
        NetworkObject no = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(NetworkManager.Singleton.LocalClientId);
        playerNameText.text = $"Player{no.GetComponent<PlayerAttributes>().playerID.Value}";
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(GameObject.Find("NetworkManager"));
        SceneManager.LoadScene(0);
    }
}
#endif