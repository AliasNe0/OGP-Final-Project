using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Samples;

public class PlayerSpawner : MonoBehaviour
{
    public NetworkVariable<float> playerLimit = new NetworkVariable<float>(4f, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<float> playerCount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone);
    private List<float> playerList = new List<float>();

    static PlayerSpawner _instance;
    public static PlayerSpawner Singleton { get { return _instance; } }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            playerCount.Value = 0f;
            playerList.Clear();
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisConnectCallback;
        }
    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            SpawnPlayer(clientID);
        }
    }

    private void SpawnPlayer(ulong clientID)
    {
        if (playerCount.Value < playerLimit.Value)
        {
            for (float id = 1f; id < playerLimit.Value; id++)
            {
                if (!playerList.Contains(id))
                {
                    playerList.Add(id);
                    playerCount.Value++;
                    NetworkObject no = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
                    no.GetComponent<CustomID>().playerID.Value = id;
                    //no.GetComponent<OGPA_PlayerMover>().originalColor.Value = GameObject.Find($"Environment/SpawnPoint{(int)id}/Cube").GetComponent<MeshRenderer>().material.color;
                    var defaultTransform = GameObject.Find($"Environment/SpawnPoint{(int)id}").transform;
                    no.GetComponent<ClientNetworkTransform>().Teleport(defaultTransform.position, defaultTransform.localRotation, defaultTransform.localScale);
                    break;
                }
            }
        }
    }

    private void OnClientDisConnectCallback(ulong clientID)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkObject no = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
            float id = no.GetComponent<CustomID>().playerID.Value;
            playerList.Remove(id);
            playerCount.Value--;
        }
    }


}
