using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftablesWidget : MonoBehaviour
{
    [SerializeField] GameObject canCraftFeedback;
    [SerializeField] RawImage image;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button button;
    [SerializeField, ShowOnly] string actionId;
    Action<string> worldActionCallback;
    public void Setup(string url, string text, string actionId, Action<string> worldActionCallback)
    {
        image.DownloadImage(url);
        this.text.text = text;
        this.actionId = actionId;
        this.worldActionCallback = worldActionCallback;

        button.onClick.AddListener(OnClickHandler);

        canCraftFeedback.SetActive(ActionUtil.ValidateActionConfig(actionId).IsOk);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickHandler);
    }

    private void OnClickHandler()
    {
        worldActionCallback?.Invoke(actionId);
    }
}
