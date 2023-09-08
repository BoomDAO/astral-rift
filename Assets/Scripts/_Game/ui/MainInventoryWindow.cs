using Boom.Utility;
using ItsJackAnton.Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainInventoryWindow : MonoBehaviour
{
    //[SerializeField] EntityPrefabList entitiesAssets;
    [SerializeField] MainInventoryWidget inventoryWidgetPrefab;
    [SerializeField] Transform content;
    [SerializeField] GameObject infoPanel;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;


    private void Start()
    {
        UserUtil.RegisterToDataChange<DataTypes.Entity>(EntityDataChangeHandler, true);
    }
    private void OnDisable()
    {
        infoPanel.SetActive(false);
    }

    private void EntityDataChangeHandler(DataState<Data<DataTypes.Entity>> state)
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }


        if (state.IsReady())
        {
            state.data.elements.Iterate(e =>
            {
                string entityName = "";
                string entityImageUrl = "";
                if(e.Value.TryGetConfig(out var config))
                {
                    entityName = config.name;
                    entityImageUrl = config.imageUrl;
                }

                if(!string.IsNullOrEmpty(entityName) && e.Value.quantity > 0)
                {
                    MainInventoryWidget widget = Instantiate(inventoryWidgetPrefab, content);

                    widget.Setup(e.Key, $"{entityName}: {e.Value.quantity}", entityImageUrl, OnSelect);
                }
            });
        }
    }

    private void OnSelect(string key)
    {
        infoPanel.SetActive(true);

        string entityName = "";
        string entityDescription = "";
        if (EntityUtil.TryGetConfig(key, out var config))
        {
            entityName = config.name;
            entityDescription = config.description;
        }

        itemNameText.text = entityName;
        itemDescriptionText.text = entityDescription;
        Debug.Log("Entity Selected: "+ key);
    }

    private void OnDestroy()
    {
        UserUtil.UnregisterToDataChange<DataTypes.Entity>(EntityDataChangeHandler);
    }
}
