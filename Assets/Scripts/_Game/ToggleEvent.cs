using UnityEngine;
using UnityEngine.Events;

public class ToggleEvent : MonoBehaviour
{
    [SerializeField] bool invokeOnStart;
    [SerializeField] bool startValue;
    [SerializeField, ShowOnly] bool value;
    [SerializeField] UnityEvent onTrue;
    [SerializeField] UnityEvent onFalse;

    private void Start()
    {
        value = startValue;

        if (!invokeOnStart) return;

        if (value) onTrue.Invoke();
        else onFalse.Invoke();
    }
    public void Toggle()
    {
        Toggle(!value);
    }
    public void Toggle(bool value)
    {
        if (this.value != value)
        {
            if (value) onTrue.Invoke();
            else onFalse.Invoke();

            this.value = value;
        }
    }
}
