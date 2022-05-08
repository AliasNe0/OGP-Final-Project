using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                GameObject go = Instantiate(playerPrefab);
                NetworkObject no = go.GetComponent<NetworkObject>();
                no.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
            }

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            //NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisConnectCallback;
        }
    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject go = Instantiate(playerPrefab);
            NetworkObject no = go.GetComponent<NetworkObject>();
            no.SpawnAsPlayerObject(clientID);
        }
    }

    //private void OnClientDisConnectCallback(ulong clientID)
    //{
    //    NetworkObject no = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
    //    Destroy(no.gameObject);
    //    no.Despawn();
    //}
}
