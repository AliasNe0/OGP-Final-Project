using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CollectibleSpawner : NetworkBehaviour
{
    [SerializeField] public GameObject collectiblePrefab;
    [SerializeField] private float collectibleLimit = 10f;
    public NetworkVariable<float> collectibleCount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone);
    private GameObject[] spawnPointsArray;
    public List<GameObject> spawnPointList = new();
    private bool spawning = false;

    static CollectibleSpawner _instance;
    public static CollectibleSpawner Singleton { get { return _instance; } }

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

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        spawnPointsArray = GameObject.FindGameObjectsWithTag("CollectibleSpawnPoint");
        foreach (GameObject spawnPoint in spawnPointsArray)
        {
            spawnPointList.Add(spawnPoint);
        }
        if (IsHost && spawnPointList.Count > 0 && collectibleCount.Value < collectibleLimit)
        {
            for (int i = 0; i < collectibleLimit; i++)
            {
                NetworkObject no = NetworkObjectPool.Singleton.GetNetworkObject(collectiblePrefab);
                Transform spawnPointTransform = GetRandomSpawnPointPosition();
                no.transform.position = spawnPointTransform.position;
                no.transform.rotation = spawnPointTransform.localRotation;
                no.Spawn();
                no.transform.parent = spawnPointTransform;
                collectibleCount.Value++;
            }
        }
    }

    private void FixedUpdate()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            if (!spawning && spawnPointList.Count > 0 && collectibleCount.Value < collectibleLimit)
            {
                spawning = true;
                StartCoroutine(CollectibleSpawnTimer());
            }
        }
    }

    IEnumerator CollectibleSpawnTimer()
    {
        yield return new WaitForSeconds(3f);
        NetworkObject no = NetworkObjectPool.Singleton.GetNetworkObject(collectiblePrefab);
        Transform spawnPointTransform = GetRandomSpawnPointPosition();
        no.transform.position = spawnPointTransform.position;
        no.transform.rotation = spawnPointTransform.localRotation;
        no.Spawn();
        no.transform.parent = spawnPointTransform;
        //no.GetComponent<NetworkTransform>().Teleport(spawnPointTransform.position, spawnPointTransform.localRotation, spawnPointTransform.localScale);
        collectibleCount.Value++;
        spawning = false;
    }

    private Transform GetRandomSpawnPointPosition()
    {
        int index = Random.Range(0, spawnPointList.Count);
        GameObject spawnPoint = spawnPointList[index];
        spawnPointList.Remove(spawnPoint);
        return spawnPoint.transform;
    }
}
