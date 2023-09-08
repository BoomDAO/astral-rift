namespace ItsJackAnton.Patterns
{
    using System;
    using UnityEngine;

    [Serializable]
    public class EntityPrefab
    {
        [SerializeField] string entityId;
        [SerializeField] string groupId;
        [SerializeField] string worldId;
        [SerializeField] GameObject prefab;
        //[SerializeField] Sprite icone;

        public GameObject Prefab { get { return prefab; } }
        //public Sprite Icone { get { return icone; } }

        public string GetKey()
        {
            var _worldId = worldId;
            if (string.IsNullOrEmpty(_worldId)) _worldId = Env.CanisterIds.WORLD;

            return $"{_worldId}{groupId}{entityId}";
        }
    }
    [CreateAssetMenu(fileName = "EntityPrefabList", menuName = "Scriptable Objects/Patterns/Asset List/EntityPrefabList")]
    public class EntityPrefabList : AssetList<EntityPrefab>
    {
        public GameObject GetPrefabByKey(string key)
        {
            foreach (var item in content) { 
                if(item.GetKey() == key)
                {
                    return item.Prefab;
                }
            }

            Debug.LogError($"Prefab for KEY: {key} was not found");
            return null;
        }

        //public Sprite GetIconByKey(string key)
        //{
        //    foreach (var item in content)
        //    {
        //        if (item.GetKey() == key)
        //        {
        //            return item.Icone;
        //        }
        //    }

        //    return null;
        //}
    }
}