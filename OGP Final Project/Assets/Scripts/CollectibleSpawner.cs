using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class CollectibleSpawner : NetworkBehaviour
{
    [SerializeField] public GameObject collectiblePrefab;
    [SerializeField] private float collectibleLimit = 10f;
    public NetworkVariable<float> collectibleCount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone);
    private GameObject[] spawnPointsArray;
    public List<GameObject> spawnPointList = new();

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
        NetworkManager.Singleton.OnServerStarted += OnServerStart;
    }

    private void OnServerStart()
    {
        spawnPointsArray = GameObject.FindGameObjectsWithTag("CollectibleSpawnPoint");
        foreach (GameObject spawnPoint in spawnPointsArray)
        {
            spawnPointList.Add(spawnPoint);
        }
    }

    private void FixedUpdate()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            if (spawnPointList.Count > 0 && collectibleCount.Value < collectibleLimit)
            {
                NetworkObject no = NetworkObjectPool.Singleton.GetNetworkObject(collectiblePrefab);
                Transform spawnPointTransform = GetRandomSpawnPointPosition();
                no.transform.position = spawnPointTransform.position;
                no.transform.rotation = spawnPointTransform.localRotation;
                no.Spawn();
                //no.GetComponent<NetworkTransform>().Teleport(spawnPointTransform.position, spawnPointTransform.localRotation, spawnPointTransform.localScale);
                collectibleCount.Value++;
            }
        }
    }

    private Transform GetRandomSpawnPointPosition()
    {
        int index = Random.Range(0, spawnPointList.Count);
        GameObject spawnPoint = spawnPointList[index];
        spawnPointList.RemoveAt(index);
        return spawnPoint.transform;
    }
}
