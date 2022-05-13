using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameStartTimer : MonoBehaviour
{
    [SerializeField] private UnityEvent timerEndEvent;
    [SerializeField] private UnityEvent timerHideEvent;
    [SerializeField] private float startGameTimer = 5f;
    [SerializeField] private TMP_Text TMPtext;
    private float currentTime;
    private bool timerActive = false;

    private void Start()
    {
        currentTime = startGameTimer + 0.5f;
        TMPtext.text = "";
    }

    void Update()
    {
        if (timerActive)
        {
            currentTime -= Time.deltaTime;
            TMPtext.text = currentTime.ToString("f0");
            if (currentTime <= 0.5f)
            {
                timerEndEvent.Invoke();
                StartCoroutine(EndTimer());
                timerActive = false;
            }
        }
    }

    IEnumerator EndTimer()
    {
        TMPtext.text = "GO!";
        yield return new WaitForSeconds(2f);
        timerHideEvent.Invoke();
        currentTime = 0f;
    }

    public void StartGameTimer()
    {
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        Cursor.visible = false;
        yield return new WaitForSeconds(1f);
        timerActive = true;
    }
}
