using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;

public class DeathSentence : MonoBehaviour
{
    [Tooltip("Elevation on which the player will die")]
    [SerializeField] private float deathElevation = -10f;
    [SerializeField] private float deathLength = 2f;
    private AdvancedWalkerController controllerScript;
    private Vector3 defaultPosition;
    private bool dead = false;

    private void Start()
    {
        defaultPosition = transform.position;
        controllerScript = GetComponent<AdvancedWalkerController>();
    }

    private void FixedUpdate()
    {
        if (transform.position.y < deathElevation && dead == false)
        {
            dead = true;
            transform.position = defaultPosition;
            //controllerScript.enabled = false;
            StartCoroutine("DeathTimer");
        }
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(deathLength);
        //controllerScript.enabled = true;
        dead = false;
    }
}
