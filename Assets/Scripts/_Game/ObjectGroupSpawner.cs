using Boom.Utility;
using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectGroupSpawner : MonoBehaviour
{
    public struct InstanceInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public GameObject instance;

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
        }

        public void DisableInstance()
        {
            if (instance) instance.SetActive(false);
        }
        public void EnableInstance(GameObject prefab)
        {
            if (instance) instance.SetActive(true);
            else
            {
                var newInstance = Instantiate(prefab, position, rotation);
                instance = newInstance.gameObject;
            }
        }

        public bool IsDisabled()
        {
            return instance == null || !instance.activeSelf;
        }
    }
    [field: SerializeField] public GameObject Prefab { get; private set; }

    [SerializeField] private string actionId;
    [SerializeField] private float frequencyCheck = 30;
    [SerializeField, ShowOnly] private float secondLeftForCheck;
    private float cachedTime;

    [SerializeField, ShowOnly] bool hasTryConstrains;
    [SerializeField, ShowOnly] ulong triesLeft;
    [SerializeField] List<GameObject> instances;
    LinkedList<InstanceInfo> instancesInfo = new();


    private void Awake()
    {
        instances.Iterate(e =>
        {
            instancesInfo.AddLast(new InstanceInfo(e));
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
        hasTryConstrains = ActionUtil.TryGetTriesLeft(actionId, out triesLeft);
    }

    private void TrySetupEnemies()
    {
        int enemiesToDisableCount = instances.Count - (int)triesLeft;

        if (enemiesToDisableCount > 0)
        {
            var arr = instancesInfo.ToArray().Randomize();

            for (int i = 0; i < enemiesToDisableCount; i++)
            {
                arr[i].DisableInstance();
            }
        }
    }

    private void Update()
    {
        if (cachedTime < Time.time)
        {
            if (cachedTime != 0)
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
        instancesInfo.Iterate(e =>
        {
            if (!e.IsDisabled()) ++activedCount;
        });

        if (triesLeft > activedCount)
        {
            instancesInfo.Iterate(e => e.EnableInstance(Prefab));

            TrySetupEnemies();
        }
    }
}
