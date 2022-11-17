using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;

public class PackSpawner : NetworkBehaviour
{
    public GameObject packPrefab;
    public int elements;
    public float spawnTimer = 0f;
    // Start is called before the first frame update
    void OnStart()
    {
        for(int i = 0; i < elements; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f));
                Quaternion spawnRotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
                GameObject healthPack = GameObject.Instantiate(packPrefab, spawnPosition, spawnRotation);
                healthPack.GetComponent<NetworkObject>().Spawn();
            }
    }

    // Update is called once per frame
    
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(IsServer && spawnTimer <= 0)
        {
            spawnTimer = 30f;
            for(int i = 0; i < elements; i++)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f));
                Quaternion spawnRotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
                GameObject healthPack = GameObject.Instantiate(packPrefab, spawnPosition, spawnRotation);
                healthPack.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
    
}
