namespace Boom.Sensors
{
    using Boom.Utility;
    using UnityEngine;
    using UnityEngine.Events;

    public class OnContact : MonoBehaviour
    {
        [SerializeField] LayerMask targetLayers;
        [SerializeField] string[] tags;
        [field: SerializeField] public UnityEvent<Transform> OnContactIn { private set; get; }
        [field: SerializeField] public UnityEvent<Transform> OnContactOut { private set; get; }
        [field: SerializeField, ShowOnly] public int DetectionCount { private set; get; }


        private void OnCollisionEnter(Collision collision)
        {
            OnEnter_(collision.gameObject);
        }
        private void OnTriggerEnter(Collider other)
        {
            OnEnter_(other.gameObject);
        }
        private void OnCollisionExit(Collision collision)
        {
            OnExit_(collision.gameObject);
        }
        private void OnTriggerExit(Collider other)
        {
            OnExit_(other.gameObject);
        }
        //
        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnEnter_(collision.gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnEnter_(collision.gameObject);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            OnExit_(collision.gameObject);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            OnExit_(collision.gameObject);
        }

        private void OnEnter_(GameObject val)
        {
            if (Conditions_(val, true))
            {
                ++DetectionCount;
                OnContactIn.Invoke(val.transform);
                InvokeOnEnter_(val);
            }
        }
        private void OnExit_(GameObject val)
        {
            if (Conditions_(val, false))
            {
                var detectionCount = DetectionCount - 1;
                if (detectionCount < 0) return;

                DetectionCount = detectionCount;
                OnContactOut.Invoke(val.transform);
                InvokeOnExit_(val);
            }
        }

        protected virtual void InvokeOnEnter_(GameObject val)
        {
        }
        protected virtual void InvokeOnExit_(GameObject val)
        {
        }



        protected virtual bool Conditions_(GameObject val, bool onEnter)
        {
            bool hasLayer = val.HasLayerOf(targetLayers);

            bool hasTag = tags.Length == 0;
            foreach (var item in tags)
            {
                if (val.CompareTag(item))
                {
                    hasTag = true;
                    break;
                }
            }

            return hasLayer && hasTag;
        }
    }
}