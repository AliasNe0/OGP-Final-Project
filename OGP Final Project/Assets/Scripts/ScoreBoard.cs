using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ScoreBoard : MonoBehaviour
{
    private List<GameObject> playerObjects = new();

    static ScoreBoard _instance;
    public static ScoreBoard Singleton { get { return _instance; } }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public KeyValuePair<float, float> FindWinner()
    {
        CalculateScores();
        KeyValuePair<float, float> winner = new KeyValuePair<float, float>(0f, 0f);
        foreach (GameObject go in playerObjects)
        {
            float playerID = go.GetComponent<PlayerAttributes>().playerID.Value;
            float playerScore = go.GetComponent<PlayerAttributes>().playerScore.Value;
            if (playerScore > winner.Value)
                winner = new KeyValuePair<float, float> (playerID, playerScore);
        }
        return winner;
    }

    private void CalculateScores()
    {
        GameObject[] playerObjectsArray = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in playerObjectsArray)
        {
            playerObjects.Add(playerObject);
        }
    }
}
