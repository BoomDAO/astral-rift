using Boom.Patterns.Broadcasts;
using Boom.Sensors;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CloseCraftingWindow : IBroadcast { }
public class CraftingTable : MonoBehaviour
{
    [SerializeField] OnContact sensor;
    [SerializeField, ShowOnly] bool canOpenCraftingWindow;
    [SerializeField, ShowOnly] bool windowIsOpened;

    private void Awake()
    {
        if (sensor)
        {
            sensor.OnContactIn.AddListener(OnInteraction);
            sensor.OnContactOut.AddListener(OnInteraction);
        }
        Broadcast.Register<CloseCraftingWindow>(CloseWindow);
    }

    private void OnDestroy()
    {
        if (sensor)
        {
            sensor.OnContactIn.RemoveListener(OnInteraction);
            sensor.OnContactOut.RemoveListener(OnInteraction);
        }
        Broadcast.Unregister<CloseCraftingWindow>(CloseWindow);
    }
    private void OnInteraction(Transform arg0)
    {
        canOpenCraftingWindow = sensor.DetectionCount > 0;

        if(canOpenCraftingWindow) CraftingWindow.Instance.ToggleCraftingWindowCrosshair(true);
        else CraftingWindow.Instance.ToggleCraftingWindowCrosshair(false);
    }

    private void Update()
    {
        if (canOpenCraftingWindow)
        {
            if (Input.GetKeyDown("e"))
            {
                if (!windowIsOpened)
                {
                    OpenWindow();
                }
                else
                {
                    CloseWindow();
                }
            }
        }
        else
        {
            CloseWindow();
        }
    }

    private void OpenWindow()
    {
        Debug.Log("Open crafting window");
        windowIsOpened = true;
        CraftingWindow.Instance.ToggleCraftingWindowHolder(true);
        CraftingWindow.Instance.ToggleCraftingWindowCrosshair(false);
        PlayerInput.Instance.ReleaseControl();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseWindow()
    {
        if (!windowIsOpened) return;
        windowIsOpened = false;
        Debug.Log("Close crafting window");
        CraftingWindow.Instance.ToggleCraftingWindowHolder(false);
        PlayerInput.Instance.GainControl();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void CloseWindow(CloseCraftingWindow window)
    {
        CloseWindow();
    }
}
