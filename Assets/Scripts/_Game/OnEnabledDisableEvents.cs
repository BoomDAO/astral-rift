using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnabledDisableEvents : MonoBehaviour
{
    [SerializeField] bool once = false;
    bool didOnEnabled, didOnDisabled;
    [SerializeField] UnityEvent OnEnabled, OnDisabled;

    private void OnEnable()
    {
        if (once && didOnEnabled) return;

        OnEnabled.Invoke();
        didOnEnabled = true;
    }

    private void OnDisable()
    {
        if (once && didOnDisabled) return;

        OnDisabled.Invoke();
        didOnDisabled = true;
    }
}
