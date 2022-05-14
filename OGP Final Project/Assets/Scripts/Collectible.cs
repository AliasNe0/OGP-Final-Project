using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Collectible : NetworkBehaviour
{
    [Tooltip("Rotation vector")]
    public Vector3 rotationVector = new Vector3(0f, 1f, 0f);
    [Tooltip("Rotation speed")]
    [SerializeField] private float rotationSpeed = 50f;
    [Tooltip("Movement vector")]
    [SerializeField] Vector3 movementVector = new Vector3(0f, 0.4f, 0f);
    [Tooltip("Movement period")]
    [SerializeField] float period = 5f;
    Vector3 startingPos;
    //[Tooltip("Sound played when a letter is collected")]
    //[SerializeField] GameObject collectibleFX;
    //GameObject parentGameObject;

    private void Start()
    {
        startingPos = transform.position;
        //parentGameObject = GameObject.FindWithTag("SpawnAtRuntime");
    }

    void Update()
    {
        RotateObject();
        OscillateObject();
    }

    private void RotateObject()
    {
        transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
    }

    private void OscillateObject()
    {
        if (period <= Mathf.Epsilon) { return; } // protect against zero period 
                                                 // set movement factor
        float cycles = Time.time / period; // grows contimually from 0
        const float tau = Mathf.PI * 2; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to 1
        float movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }

    // Enables a corresponding letter in UI Canvas
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    // Pick up sound played
    // Destroys the gameObject if collided with a player object
    private void Collect()
    {
        //GameObject vfx = Instantiate(collectibleFX, transform.position, Quaternion.identity);
        //vfx.transform.parent = parentGameObject.transform;
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
