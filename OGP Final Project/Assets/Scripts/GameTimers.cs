using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Unity.Netcode;

public class GameTimers : NetworkBehaviour
{
    [SerializeField] private float firstTimerLength = 5f;
    [SerializeField] private TMP_Text firstTimerText;
    public NetworkVariable<float> firstTimerValue = new NetworkVariable<float>();
    public NetworkVariable<bool> firstTimerActive = new NetworkVariable<bool>();
    private float firstTimerSeconds = 0f;
    private bool firstTickingActive = false;
    private bool firstTimerLastTickDone = false;
    [SerializeField] private UnityEvent firstTimerEndEvent;
    [SerializeField] private UnityEvent firstTimerStartGameplayAudionEvent;
    [SerializeField] private UnityEvent firstTimerHideEvent;
    [SerializeField] private AudioSource firstTimerCountdownAudioSource;

    [SerializeField] private float secondTimerLength = 30f;
    [SerializeField] private TMP_Text secondTimerText;
    public NetworkVariable<float> secondTimerValue = new NetworkVariable<float>();
    public NetworkVariable<bool> secondTimerActive = new NetworkVariable<bool>();
    private float secondTimerSeconds = 0f;
    private bool secondTickingActive = false;
    private bool secondTimerLastTickDone = false;
    [SerializeField] private UnityEvent secondTimerEndEvent;
    [SerializeField] private UnityEvent secondTimerHideEvent;

    private void Start()
    {
        firstTimerText.text = "";
        secondTimerText.text = "";
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        if (IsHost)
        {
            firstTimerActive.Value = false;
            firstTimerValue.Value = firstTimerLength;
            secondTimerActive.Value = false;
            secondTimerValue.Value = secondTimerLength;
        }
    }

    public void StartFirstTimer()
    {
        if (IsHost)
        {
            Cursor.visible = false;
            firstTimerActive.Value = true;
            firstTickingActive = true;
        }
    }

    private void StartSecondTimer()
    {
        if (IsHost)
        {
            secondTimerActive.Value = true;
            secondTickingActive = true;
        }
    }

    void Update()
    {
        RunFirstTimer();
        RunSecondTimer();
    }

    private void RunFirstTimer()
    {
        if (firstTimerActive.Value)
        {
            if (IsHost && firstTickingActive)
            {
                if ((firstTimerValue.Value == firstTimerLength - firstTimerSeconds) && firstTimerValue.Value > 0f)
                {
                    firstTickingActive = false;
                    firstTimerText.text = firstTimerValue.Value.ToString();
                    PlayCountDownClientRpc();
                    StartCoroutine(FirstTimerTickTimer());
                }
                if (firstTimerValue.Value == 0f && !firstTimerLastTickDone)
                {
                    firstTimerLastTickDone = true;
                    firstTimerText.text = "GO!";
                    StartSecondTimer();
                    StartCoroutine(HideFirstTimer());
                    StartCoroutine(DisableFirstTimer());
                    firstTimerEndEvent.Invoke();
                }
            }
            if (IsClient)
            {
                if (firstTimerValue.Value > 0f)
                    firstTimerText.text = firstTimerValue.Value.ToString();
                if (firstTimerValue.Value == 0f)
                {
                    firstTimerText.text = "GO!";
                    firstTimerStartGameplayAudionEvent.Invoke();
                    StartCoroutine(HideFirstTimer());
                }
            }
        }
    }

    [ClientRpc]
    private void PlayCountDownClientRpc()
    {
        firstTimerCountdownAudioSource.Play();
    }

    IEnumerator FirstTimerTickTimer()
    {
        float tick = 1f;
        yield return new WaitForSeconds(tick);
        firstTimerSeconds += tick;
        firstTimerValue.Value -= tick;
        firstTickingActive = true;
    }

    IEnumerator DisableFirstTimer()
    {
        yield return new WaitForSeconds(1f);
        firstTimerEndEvent.Invoke();
        firstTimerActive.Value = false;
    }

    IEnumerator HideFirstTimer()
    {
        yield return new WaitForSeconds(2f);
        firstTimerHideEvent.Invoke();
    }

    private void RunSecondTimer()
    {
        if (secondTimerActive.Value)
        {
            if (IsHost && secondTickingActive)
            {
                if ((secondTimerValue.Value == secondTimerLength - secondTimerSeconds) && secondTimerValue.Value >= 0f)
                {
                    secondTickingActive = false;
                    secondTimerText.text = secondTimerValue.Value.ToString();
                    StartCoroutine(SecondTimerTickTimer());
                }
                if (secondTimerValue.Value == 0f && !secondTimerLastTickDone)
                {
                    secondTimerLastTickDone = true;
                    StartCoroutine(HideSecondTimer());
                    StartCoroutine(DisableSecondTimer());
                    secondTimerEndEvent.Invoke();
                }
            }
            if (IsClient)
            {
                if (secondTimerValue.Value > 0f)
                    secondTimerText.text = secondTimerValue.Value.ToString();
                if (secondTimerValue.Value == 0f)
                {
                    StartCoroutine(HideSecondTimer());
                    secondTimerEndEvent.Invoke();
                }
            }
        }
    }

    IEnumerator SecondTimerTickTimer()
    {
        float tick = 1f;
        yield return new WaitForSeconds(tick);
        secondTimerSeconds += tick;
        secondTimerValue.Value -= tick;
        secondTickingActive = true;
    }

    IEnumerator DisableSecondTimer()
    {
        yield return new WaitForSeconds(1f);
        secondTimerEndEvent.Invoke();
        secondTimerActive.Value = false;
    }

    IEnumerator HideSecondTimer()
    {
        yield return new WaitForSeconds(1f);
        secondTimerHideEvent.Invoke();
    }
}
