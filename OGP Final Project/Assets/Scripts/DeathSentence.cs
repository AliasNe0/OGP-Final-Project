using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Samples;

public class DeathSentence : NetworkBehaviour
{
    [Tooltip("Elevation on which the player will die")]
    [SerializeField] private float deathElevation = -10f;
    [SerializeField] public PlayerState playerState = PlayerState.Alive;
    public Transform defaultTransform;

    public enum PlayerState
    {
        Alive,
        Dead
    }

    public override void OnNetworkSpawn()
    {
        playerState = PlayerState.Dead;
    }

    private void Update()
    {
        if (gameObject.GetComponentInParent<NetworkObject>().IsOwner)
        {
            if (transform.position.y < deathElevation)
            {
                float id = gameObject.GetComponent<PlayerAttributes>().playerID.Value;
                defaultTransform = GameObject.Find($"Environment/SpawnPoint{(int)id}").transform;
                playerState = PlayerState.Dead;
                gameObject.GetComponent<ClientNetworkTransform>().Teleport(defaultTransform.position, defaultTransform.localRotation, defaultTransform.localScale);
            }
        }
    }
}
