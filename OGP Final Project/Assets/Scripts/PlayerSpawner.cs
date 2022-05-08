using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : MonoBehaviour
{
    public NetworkVariable<float> playerLimit = new NetworkVariable<float>(4f, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<float> playerCount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone);
    List<float> playerList = new List<float>();

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

    void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            SpawnPlayer(NetworkManager.Singleton.LocalClientId);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisConnectCallback;
        }
    }

    void OnClientConnectedCallback(ulong clientID)
    {
        SpawnPlayer(clientID);
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
                    NetworkObject no = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
                    no.GetComponent<CustomID>().id = id;
                    //no.GetComponent<OGPA_PlayerMover>().originalColor.Value = GameObject.Find($"Environment/SpawnPoint{(int)id}/Cube").GetComponent<MeshRenderer>().material.color;
                    no.transform.position = GameObject.Find($"Environment/SpawnPoint{(int)id}").transform.position;
                    break;
                }
            }
        }
    }

    void OnClientDisConnectCallback(ulong clientID)
    {
        NetworkObject no = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
        float id = no.GetComponent<CustomID>().id;
        playerList.Remove(id);
    }
}
