using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;

public class GameStartTimer : NetworkBehaviour
{
    [SerializeField] private UnityEvent timerEndEvent;
    [SerializeField] private UnityEvent timerHideEvent;
    [SerializeField] private float startGameTimer = 5f;
    [SerializeField] private TMP_Text TMPtext;
    public NetworkVariable<float> timerValue = new NetworkVariable<float>();
    public NetworkVariable<bool> timerActive = new NetworkVariable<bool>();
    private float seconds = 0f;
    private bool tickingActive = false;
    private bool lastTickDone = false;

    private void Start()
    {
        TMPtext.text = "";
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        if (IsHost)
        {
            timerActive.Value = false;
            timerValue.Value = startGameTimer;
        }
    }

    public void StartGameTimer()
    {
        if (IsHost)
        {
            Cursor.visible = false;
            timerActive.Value = true;
            tickingActive = true;
        }
    }

    void Update()
    {
        if (timerActive.Value)
        {
            if (IsHost && tickingActive)
            {
                if ((timerValue.Value == startGameTimer - seconds) && timerValue.Value > 0f)
                {
                    tickingActive = false;
                    TMPtext.text = timerValue.Value.ToString();
                    StartCoroutine(TickingTimer());
                }
                if (timerValue.Value == 0f && !lastTickDone)
                {
                    lastTickDone = true;
                    TMPtext.text = "GO!";
                    StartCoroutine(HideTimer());
                    StartCoroutine(DisableTimer());
                    timerEndEvent.Invoke();
                }
            }
            if (IsClient)
            {
                if (timerValue.Value > 0f)
                    TMPtext.text = timerValue.Value.ToString();
                if (timerValue.Value == 0f)
                {
                    TMPtext.text = "GO!";
                    StartCoroutine(HideTimer());
                }
            }
        }
    }

    IEnumerator TickingTimer()
    {
        float tick = 1f;
        yield return new WaitForSeconds(tick);
        seconds += tick;
        timerValue.Value -= tick;
        tickingActive = true;
    }

    IEnumerator DisableTimer()
    {
        yield return new WaitForSeconds(1f);
        timerHideEvent.Invoke();
        timerActive.Value = false;
    }

    IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(2f);
        timerHideEvent.Invoke();
    }
}
