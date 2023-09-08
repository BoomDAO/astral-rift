using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private string actionId = "set_level##_checkpoint##";
        [field: SerializeField] public string CheckpointId { get; private set; } = "level##_checkpoint";
        public static Dictionary<string, Checkpoint> Checkpoints { get; private set; } = new();

        private void Awake()
        {
            //we make sure the checkpoint is part of the Checkpoint layer, which is set to interact ONLY with the player layer.
            gameObject.layer = LayerMask.NameToLayer("Checkpoint");

            if(!Checkpoints.TryAdd(CheckpointId, this))
            {
                Debug.LogError("Issue registering checkpoint of id: "+ CheckpointId + ".  Current Checkpoint count: "+ Checkpoints.Count);
            }
        }

        private void OnDestroy()
        {
            Checkpoints.Remove(CheckpointId);
        }

        private void OnTriggerEnter(Collider other)
        {
            ExecuteAction(other);
        }

        private async void ExecuteAction(Collider other)
        {
            PlayerController controller = other.GetComponent<PlayerController>();

            if (controller == null)
                return;

            if (controller.GetCheckpoint() == this) return;

            if(EntityUtil.GetCurrentQuantity($"{Env.CanisterIds.WORLD}{"checkpoint"}{CheckpointId}") > 0)
            {
                Debug.Log($"SET CHECKPOINT OF ID: {CheckpointId} AS CURRENT CHECKPOINT. A");

                controller.SetCheckpoint(this);
            }
            else
            {
                Debug.Log($"SET CHECKPOINT OF ID: {CheckpointId} AS CURRENT CHECKPOINT. B");

                var result = await ActionUtil.Action.Default(actionId);

                if (result.IsErr)
                {
                    Debug.LogError(result.AsErr().content);

                    return;
                }

                Debug.Log($"SET CHECKPOINT OF ID: {CheckpointId} AS CURRENT CHECKPOINT. C");

                controller.SetCheckpoint(this);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue * 0.75f;
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawRay(transform.position, transform.forward * 2);
        }
    }
}
