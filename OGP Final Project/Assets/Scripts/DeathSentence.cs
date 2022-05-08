using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class DeathSentence : MonoBehaviour
{
    [Tooltip("Elevation on which the player will die")]
    [SerializeField] private float deathElevation = -10f;
    [SerializeField] private float deathLength = 2f;
    private GameObject gates;
    private Vector3 defaultPosition;

    private void Start()
    {
        defaultPosition = transform.position;
        gates = GameObject.Find($"Environment/Gates1");
        MoveGates();
    }

    private void FixedUpdate()
    {
        if (transform.position.y < deathElevation)
        {
            transform.position = defaultPosition;
            MoveGates();
        }
    }

    public void MoveGates()
    {
        gates.GetComponent<GateMover>().StartGatesCoroutine();
    }
}
