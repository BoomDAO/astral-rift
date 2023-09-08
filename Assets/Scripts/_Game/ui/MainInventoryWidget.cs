using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainInventoryWidget : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Button button;
    [SerializeField, ShowOnly] string entityId;
    Action<string> worldActionCallback;
    public void Setup(string entityId, string name, string url, Action<string> worldActionCallback)
    {
        image.DownloadImage(url);
        this.nameText.text = name;
        this.entityId = entityId;
        this.worldActionCallback = worldActionCallback;

        button.onClick.AddListener(OnClickHandler);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickHandler);
    }

    private void OnClickHandler()
    {
        worldActionCallback?.Invoke(entityId);
    }
}
