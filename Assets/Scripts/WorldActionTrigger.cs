using Boom.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldActionTrigger : MonoBehaviour
{
    [SerializeField] string actionId;

    [field: SerializeField] public UnityEvent<ProcessedActionResponse> OnSuccess { get; private set; }
    [field: SerializeField] public UnityEvent<string> OnFailure { get; private set; }

    public string ActionId { get { return actionId; } }

    public async void ExecuteAction()
    {
        var result = await ActionUtil.Action.Default(ActionId);

        if (result.IsOk) OnSuccess.Invoke(result.AsOk());
        else
        {
            OnFailure.Invoke(result.AsErr().content);
            Debug.LogError(result.AsErr().content);
        }
    }
}
