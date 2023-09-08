using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform optinalSpawnpoint;
    [SerializeField] Vector3 spawnOffset = Vector3.zero;
    public Vector3 SpawnPoint
    {
        get
        {
            return optinalSpawnpoint ? optinalSpawnpoint.position : transform.position;
        }
    }


    public void Spawn()
    {
        var go = Instantiate(prefab);
        go.transform.position = SpawnPoint + spawnOffset;
    }
}
