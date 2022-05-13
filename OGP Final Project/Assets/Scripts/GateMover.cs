using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMover : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    private Vector3 defaultPosition; // default position of the gates
    [Tooltip("How far gates will move")]
    [SerializeField] private Vector3 targetPosition = new Vector3(0f, -4f, 0f); // realtive target position of the gates


    private void Start()
    {
        defaultPosition = transform.position;
        targetPosition = defaultPosition + targetPosition;
    }

    public void OpenGates()
    {
        Vector3 startPosition = defaultPosition;
        Vector3 endPosition = targetPosition;
        StartCoroutine(FollowPath(startPosition, endPosition));
    }
    IEnumerator FollowPath(Vector3 startPosition, Vector3 endPosition)
    {
        float travelPercent = 0f;
        while (travelPercent < 1f)
        {
            travelPercent += Time.deltaTime * movementSpeed;
            transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
            yield return new WaitForEndOfFrame();
        }
    }
}
