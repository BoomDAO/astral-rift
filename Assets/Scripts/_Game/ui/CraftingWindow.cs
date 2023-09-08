using Boom.Patterns.Broadcasts;
using Boom.Utility;
using Gamekit3D;
using ItsJackAnton.Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static ActionErrType;

public class CraftingWindow : Singleton<CraftingWindow>
{
    [SerializeField] EntityPrefabList entitiesAssets;
    [SerializeField] CraftablesWidget craftablesWidget;
    [SerializeField] GameObject craftingWindowCrosshair;
    [SerializeField] GameObject craftingWindowHolder;
    [SerializeField] Transform content;

    [SerializeField, ShowOnly] string selectedActionId;
    [SerializeField] RawImage selectionImage;
    [SerializeField] TextMeshProUGUI selectionName;
    [SerializeField] TextMeshProUGUI selectionDescription;
    [SerializeField] Button craftButton;
    [SerializeField] TextMeshProUGUI warningRequirementText;

    [field: SerializeField] public UnityEvent<ProcessedActionResponse> OnSuccess { get; private set; }
    [field: SerializeField] public UnityEvent<string> OnFailure { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        craftButton.onClick.AddListener(ExecuteWorldAction);
        ToggleCraftingWindowCrosshair(false);
        ToggleCraftingWindowHolder(false);

        UserUtil.RegisterToDataChange<DataTypes.Entity>(OnEntityDataChangeHandler);
    }

    private void OnDestroy()
    {
        craftButton.onClick.RemoveListener(ExecuteWorldAction);

        UserUtil.UnregisterToDataChange<DataTypes.Entity>(OnEntityDataChangeHandler);
    }
    private void OnEntityDataChangeHandler(DataState<Data<DataTypes.Entity>> state)
    {
        UpdateWindow();
        OnCraftableSelected(selectedActionId);
    }
    private void UpdateWindow()
    {
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }


        var actionConfigResult = UserUtil.GetDataOfType<DataTypes.ActionConfig>();

        if (actionConfigResult.IsErr)
        {
            Debug.LogError("Actions Configs could not be found");
            return;
        }

        var actionConfigAsOk = actionConfigResult.AsOk();

        var craftableActions = actionConfigAsOk.data.elements.Values.Filter(e => e.tag == "craftable");

        bool firstWasSetup = false;
        if (craftableActions != null)
        {
            craftableActions.Iterate(e =>
            {
                if (!firstWasSetup)
                {
                    firstWasSetup = true;
                    OnCraftableSelected(e.aid);
                }

                CraftablesWidget widget = Instantiate(craftablesWidget, content);

                widget.Setup(e.imageUrl, e.name, e.aid, OnCraftableSelected);
            });
        }
    }

    private void OnCraftableSelected(string actionId)
    {
        selectedActionId = actionId;
        var asOk = UserUtil.GetElementOfType<DataTypes.ActionConfig>(actionId).AsOk();

        var requirements = asOk.entityConstraints.Map(e =>
        {
            var requiredEntityId = e.GetKey();
            var entityConfig = UserUtil.GetElementOfType<DataTypes.EntityConfig>(requiredEntityId);

            if (entityConfig.IsErr)
            {
                Debug.LogError("ERROR " + e.Eid);
                return "";
            }

            var entityConfigAsOk = entityConfig.AsOk();

            if (e.GreaterThanOrEqualQuantity.HasValue)
            {
                return $"{entityConfigAsOk.name}: {EntityUtil.GetCurrentQuantity(e.GetKey())}/{e.GreaterThanOrEqualQuantity.ValueOrDefault}";
            }
            return "";
        });
        selectionImage.DownloadImage(asOk.imageUrl);
        selectionName.text = asOk.name;
        selectionDescription.text = $"{asOk.description}\n\n Requirements:\n{requirements.Reduce(e => e, "\n")}";

        var canBeCraftableResult = ActionUtil.ValidateActionConfig(actionId);

        craftButton.interactable = canBeCraftableResult.IsOk;

        warningRequirementText.gameObject.SetActive(!canBeCraftableResult.IsOk);
    }

    private async void ExecuteWorldAction()
    {

        var asOk = UserUtil.GetElementOfType<DataTypes.ActionConfig>(selectedActionId).AsOk();

        var requirements = asOk.entityConstraints.Filter(e => e.GreaterThanOrEqualQuantity.HasValue).Map(e =>
        {
            return new DataTypes.Entity(e.Wid.ValueOrDefault, e.Gid, e.Eid, e.GreaterThanOrEqualQuantity.ValueOrDefault, null, null);
        });

        var validateActionState = ActionUtil.ValidateActionConfig(selectedActionId);

        craftButton.interactable = false;
        var result = await ActionUtil.Action.Default(selectedActionId);


        if (result.IsOk)
        {
            result.AsOk().spentEntities.Iterate(e => EntityUtil.DecrementCurrentQuantity(e));
            result.AsOk().receivedEntities.Iterate(e => EntityUtil.IncrementCurrentQuantity(e));

            OnSuccess.Invoke(result.AsOk());
        }
        else
        {
            Debug.LogWarning(result.AsErr().content);
            OnFailure.Invoke(result.AsErr().content);
        }
    }

    public void CloseWindow()
    {
        Broadcast.Invoke<CloseCraftingWindow>();
    }


    public void ToggleCraftingWindowHolder(bool val)
    {
        if(val)
        {
            UpdateWindow();
            OnCraftableSelected(selectedActionId);
        }
        craftingWindowHolder.SetActive(val);
        Time.timeScale = val? 0 : 1;
    }
    public void ToggleCraftingWindowCrosshair(bool val)
    {
        craftingWindowCrosshair.SetActive(val);
    }
}
