namespace ItsJackAnton.Patterns
{
    using UnityEngine;
    [System.Serializable]
    public class AssetList<T> : ScriptableObject
    {
        [SerializeField] protected T[] content = new T[0];

        public T[] Content { get { return content; } }

        public T GetRandom()
        {
            if (content.Length == 0) return default;
            return content[Random.Range(0, content.Length)];
        }
    }
}