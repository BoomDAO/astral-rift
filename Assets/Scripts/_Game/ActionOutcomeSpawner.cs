 using Boom.Patterns;
using ItsJackAnton.Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WorldActionTrigger))]  
public class ActionOutcomeSpawner : MonoBehaviour
{
    [SerializeField] EntityPrefabList entitiesAssets;
    [SerializeField] Transform optinalSpawnpoint;
    [SerializeField] Vector3 spawnOffset = Vector3.zero;

    WorldActionTrigger source;

    public Vector3 SpawnPoint { get {
        return optinalSpawnpoint? optinalSpawnpoint.position : transform.position;
        } 
    }

    private void Start()
    {
        source = GetComponent<WorldActionTrigger>();

        source.OnSuccess.AddListener(OnSuccessHandler);
        source.OnFailure.AddListener(OnFailureHandler);
    }

    private void OnDestroy()
    {
        source.OnSuccess.RemoveListener(OnSuccessHandler);
        source.OnFailure.RemoveListener(OnFailureHandler);
    }

    private void OnSuccessHandler(ProcessedActionResponse arg)
    {
        if (entitiesAssets)
        {
            foreach (var entity in arg.receivedEntities)
            {
                var prefab = entitiesAssets.GetPrefabByKey(entity.GetKey());

                var go = Instantiate(prefab);
                go.transform.position = SpawnPoint + spawnOffset;
            }
        }
    }
    private void OnFailureHandler(string errMsg)
    {
        Debug.Log($"OnFailure: {errMsg}");
    }
}
