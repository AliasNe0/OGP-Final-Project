using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections;

public class PlayerAttributes : NetworkBehaviour
{
    public NetworkVariable<float> playerID = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone);
    public NetworkVariable<float> playerScore = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone);
    private TMP_Text playerScoreText;

    private void Start()
    {
        playerScoreText = GameObject.Find($"Canvas/PlayerUI/PlayerScore").GetComponent<TMP_Text>();
    }

    public void UpdateScoreText()
    {
        StartCoroutine(PlayerScoreDelayTimer());
    }

    IEnumerator PlayerScoreDelayTimer()
    {
        yield return new WaitForSeconds(0.5f);
        if (IsOwner)
            playerScoreText.text = $"Score: {playerScore.Value}";
    }
}
