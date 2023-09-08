using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitUEvent : MonoBehaviour
{
    [SerializeField] bool initiateOnStart;
    [SerializeField] private float waitTime = 5;
    [SerializeField, ShowOnly] private float secondLeftForCheck;
    private float cachedTime;
    [SerializeField, ShowOnly] bool started;
    [SerializeField, ShowOnly] bool completed;

    [SerializeField] UnityEvent OnComplete;


    private void Start()
    {
        if (initiateOnStart) StartWaiting();
    }
    private void Update()
    {
        if (!started || completed) return;

        if (cachedTime < Time.time)
        {
            if (cachedTime != 0)
            {
                completed = true;
                OnComplete.Invoke();
            }
            else
            {
                cachedTime = Time.time + waitTime;
            }
        }
        else
        {
            secondLeftForCheck = cachedTime - Time.time;
        }
    }


    public void StartWaiting()
    {
        started = true;
    }
}
