using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    [SerializeField] private GameObject gates;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<DeathSentence>().playerState == DeathSentence.PlayerState.Dead)
        {
            other.GetComponent<DeathSentence>().playerState = DeathSentence.PlayerState.Alive;
            MoveGates();
        }
    }

    public void MoveGates()
    {
        gates.GetComponent<GateMover>().StartGatesCoroutine();
    }
}
