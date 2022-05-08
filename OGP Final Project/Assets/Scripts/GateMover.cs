using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMover : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [Tooltip("Time before the gates open")]
    [SerializeField] private float closeLengh = 5f;
    [Tooltip("Time before the gates close")]
    [SerializeField] private float openLengh = 10f;
    [Tooltip("How far gates will move")]
    [SerializeField] private Vector3 targetPosition = new Vector3(0f, -4f, 0f); // realtive target position of the gates
    private Vector3 defaultPosition; // default position of the gates

    private void Start()
    {
        defaultPosition = transform.position;
    }

    public void StartGatesCoroutine()
    {
        StartCoroutine("CloseTimer");
    }

    IEnumerator CloseTimer()
    {
        yield return new WaitForSeconds(closeLengh);
        targetPosition = defaultPosition + targetPosition;
        StartCoroutine("OpenGates");
        StartCoroutine("OpenTimer");
    }

    IEnumerator OpenTimer()
    {
        yield return new WaitForSeconds(openLengh);
        StartCoroutine("CloseGates");
    }

    IEnumerator OpenGates()
    {
        Vector3 startPosition = defaultPosition;
        Vector3 endPosition = targetPosition;
        float travelPercent = 0f;
        while (travelPercent < 1f)
        {
            travelPercent += Time.deltaTime * movementSpeed;
            transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CloseGates()
    {
        Vector3 startPosition = targetPosition;
        Vector3 endPosition = defaultPosition;
        float travelPercent = 0f;
        while (travelPercent < 1f)
        {
            travelPercent += Time.deltaTime * movementSpeed;
            transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
            yield return new WaitForEndOfFrame();
        }
    }
}