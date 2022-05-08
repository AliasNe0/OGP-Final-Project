using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSentence : MonoBehaviour
{
    [Tooltip("Elevation on which the player will die")]
    [SerializeField] private float deathElevation = -10f;
    [SerializeField] private float deathLength = 2f;
    [SerializeField] public PlayerState playerState = PlayerState.Alive;
    private Vector3 defaultPosition;

    public enum PlayerState
    {
        Alive,
        Dead
    }

    private void Start()
    {
        defaultPosition = transform.position;
        playerState = PlayerState.Dead;
    }

    private void Update()
    {
        if (transform.position.y < deathElevation)
        {
            playerState = PlayerState.Dead;
            transform.position = defaultPosition;
        }
    }
}
