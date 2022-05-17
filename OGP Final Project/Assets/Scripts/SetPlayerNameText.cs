using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class SetPlayerNameText : NetworkBehaviour
{
    [SerializeField] private Camera thisPlayerCamera;
    public List<GameObject> players = new();

    public void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            float playerID = gameObject.GetComponentInChildren<PlayerAttributes>().playerID.Value;
            TMP_Text playerName = gameObject.GetComponentInChildren<TMP_Text>();
            playerName.text = $"Player{playerID}";
        }
        if (IsOwner)
        {
            GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in playersArray)
            {
                players.Add(player);
            }

            foreach (GameObject player in players)
            {
                player.GetComponentInChildren<PlayerNameLookAtCamera>()._camera = thisPlayerCamera;
            }
        }
    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        if (IsOwner)
        {
            NetworkObject no = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
            no.GetComponentInChildren<PlayerNameLookAtCamera>()._camera = thisPlayerCamera;
        }
    }

}
