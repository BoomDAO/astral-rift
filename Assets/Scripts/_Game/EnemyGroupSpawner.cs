using Boom.Utility;
using Gamekit3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyGroupSpawner : MonoBehaviour
{
    public enum BasicEnemyType
    {
        Chomper,
        Spitter,
        None
    }
    public struct InstanceInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public GameObject instance;
        public BasicEnemyType basicEnemyType;

        public InstanceInfo(GameObject instance) : this()
        {
            if (!instance)
            {
                Debug.LogError("Instance cannot be null");
                return;
            }
            this.instance = instance;
            position = instance.transform.position;
            rotation = instance.transform.rotation;

            if(instance.GetComponent<ChomperBehavior>() != null)
            {
                basicEnemyType = BasicEnemyType.Chomper;
            }
            else
            {
                if (instance.GetComponent<SpitterBehaviour>() != null)
                {
                    basicEnemyType = BasicEnemyType.Spitter;
                }
                else
                {
                    basicEnemyType = BasicEnemyType.None;
                }
            }
        }

        public void DisableInstance()
        {
            if(instance) instance.SetActive(false);
        }
        public void EnableInstance(EnemyGroupSpawner enemyGroupSpawner)
        {
            if (instance) instance.SetActive(true);
            else
            {
                if(basicEnemyType  == BasicEnemyType.None)
                {
                    Debug.LogError("Instance cannot be of BasicEnemyType None");

                    return;
                }

                
                if(basicEnemyType == BasicEnemyType.Chomper)
                {
                    var newInstance = Instantiate(enemyGroupSpawner.ChomperPrefab, position, rotation);
                    instance = newInstance.gameObject;
                }
                else
                {
                    var newInstance = Instantiate(enemyGroupSpawner.SpitterPrefab, position, rotation);
                    instance = newInstance.gameObject;
                }
            }
        }

        public bool IsDisabled()
        {
            return instance == null || !instance.activeSelf;
        }
    }
    [field: SerializeField] public ChomperBehavior ChomperPrefab { get; private set; }
    [field: SerializeField] public SpitterBehaviour SpitterPrefab { get; private set; }

    [SerializeField] private string attackActionId;
    [SerializeField] private float frequencyCheck = 30;
    [SerializeField, ShowOnly] private float secondLeftForCheck;
    private float cachedTime;

    [SerializeField, ShowOnly] bool hasTryConstrains; 
    [SerializeField, ShowOnly] ulong triesLeft; 
    [SerializeField] List<GameObject> enemies;
    LinkedList<InstanceInfo> enemiesInfo = new();


    private void Awake()
    {
        enemies.Iterate(e =>
        {
            enemiesInfo.AddLast(new InstanceInfo(e));
        });
    }

    private void Start()
    {
        UserUtil.RegisterToDataChange<DataTypes.Action>(OnActionStateChange, true);

        TrySetupEnemies();
    }

    private void OnDestroy()
    {
        UserUtil.UnregisterToDataChange<DataTypes.Action>(OnActionStateChange);
    }

    private void OnActionStateChange(DataState<Data<DataTypes.Action>> state)
    {
        hasTryConstrains = ActionUtil.TryGetTriesLeft(attackActionId, out triesLeft);
    }

    private void TrySetupEnemies()
    {
        int enemiesToDisableCount = enemies.Count - (int)triesLeft;

        if (enemiesToDisableCount > 0)
        {
            var arr = enemiesInfo.ToArray().Randomize();

            for (int i = 0; i < enemiesToDisableCount; i++)
            {
                arr[i].DisableInstance();
            }
        }
    }

    private void Update()
    {
        if(cachedTime < Time.time)
        {
            if(cachedTime != 0)
            {
                CheckToEnableEnemies();
            }
            cachedTime = Time.time + frequencyCheck;
        }
        else
        {
            secondLeftForCheck = cachedTime - Time.time;
        }
    }

    private void CheckToEnableEnemies()
    {
        ulong activedCount = 0;
        enemiesInfo.Iterate(e =>
        {
            if (!e.IsDisabled()) ++activedCount;
        });

        if(triesLeft > activedCount)
        {
            enemiesInfo.Iterate(e => e.EnableInstance(this));

            TrySetupEnemies();
        }
    }
}
