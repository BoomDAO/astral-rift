using Boom.Patterns.Broadcasts;
using Boom.Values;
using Candid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public class WindowData
    {
    }

    public Button logInBtn;
    public TextMeshProUGUI logInText;

    [SerializeField] UnityEvent OnLoggedIn;

    readonly List<Type> typesToLoad = new();

    bool? initialized;


    private void Awake()
    {
        Broadcast.Register<UserLogout>(LogoutHandler);
        UserUtil.RegisterToLoginDataChange(UpdateWindow, true);
        UserUtil.RegisterToDataChange<DataTypes.Entity>(UpdateWindow);
        UserUtil.RegisterToDataChange<DataTypes.Action>(UpdateWindow);
        UserUtil.RegisterToDataChange<DataTypes.Stake>(UpdateWindow);
        UserUtil.RegisterToDataChange<DataTypes.NftCollection>(UpdateWindow);

        logInBtn.onClick.AddListener(LogIn);
        typesToLoad.Add(typeof(DataTypes.Entity));
        typesToLoad.Add(typeof(DataTypes.Action));
        typesToLoad.Add(typeof(DataTypes.Stake));
        typesToLoad.Add(typeof(DataTypes.NftCollection));
    }
    private void OnDestroy()
    {
        LoginManager.Instance.CancelLogin();

        logInBtn.onClick.RemoveListener(LogIn);

        Broadcast.Unregister<UserLogout>(LogoutHandler);
        UserUtil.UnregisterToLoginDataChange(UpdateWindow);
        UserUtil.UnregisterToDataChange<DataTypes.Entity>(UpdateWindow);
        UserUtil.UnregisterToDataChange<DataTypes.Action>(UpdateWindow);
        UserUtil.UnregisterToDataChange<DataTypes.Stake>(UpdateWindow);
        UserUtil.UnregisterToDataChange<DataTypes.NftCollection>(UpdateWindow);
    }

    private void LogoutHandler(UserLogout logout)
    {
        logInBtn.gameObject.SetActive(true);
        logInBtn.interactable = true;
        logInText.text = "LOGIN";
    }

    private void UpdateWindow(DataState<Data<DataTypes.Entity>> state)
    {
        if (state.IsReady() && typesToLoad.Count > 0)
        {
            if (typesToLoad.Contains(typeof(DataTypes.Entity)))
            {
                typesToLoad.Remove(typeof(DataTypes.Entity));
            }
        }

        var loginDataStateResult = UserUtil.GetLogInDataState();
        if (loginDataStateResult.IsOk) UpdateWindow(loginDataStateResult.AsOk());
    }
    private void UpdateWindow(DataState<Data<DataTypes.Action>> state)
    {
        if (state.IsReady() && typesToLoad.Count > 0)
        {
            if (typesToLoad.Contains(typeof(DataTypes.Action)))
            {
                typesToLoad.Remove(typeof(DataTypes.Action));
            }
        }

        var loginDataStateResult = UserUtil.GetLogInDataState();
        if (loginDataStateResult.IsOk) UpdateWindow(loginDataStateResult.AsOk());
    }
    private void UpdateWindow(DataState<Data<DataTypes.Stake>> state)
    {
        if (state.IsReady() && typesToLoad.Count > 0)
        {
            if (typesToLoad.Contains(typeof(DataTypes.Stake)))
            {
                typesToLoad.Remove(typeof(DataTypes.Stake));
            }
        }

        var loginDataStateResult = UserUtil.GetLogInDataState();
        if (loginDataStateResult.IsOk) UpdateWindow(loginDataStateResult.AsOk());
    }
    private void UpdateWindow(DataState<Data<DataTypes.NftCollection>> state)
    {
        if (UserUtil.IsDataValid<DataTypes.NftCollection>(Env.Nfts.BOOM_COLLECTION_CANISTER_ID) && typesToLoad.Count > 0)
        {
            if (typesToLoad.Contains(typeof(DataTypes.NftCollection)))
            {
                typesToLoad.Remove(typeof(DataTypes.NftCollection));
            }
        }

        var loginDataStateResult = UserUtil.GetLogInDataState();
        if (loginDataStateResult.IsOk) UpdateWindow(loginDataStateResult.AsOk());
    }

    private void UpdateWindow(DataState<LoginData> state)
    {
        bool isLoading = state.IsLoading();
        var getIsLoginResult = UserUtil.GetLogInType();

        //logInBtn.interactable = state.IsReady();
        //logOutBtn.interactable = state.IsReady();

        if (getIsLoginResult.Tag == UResultTag.Ok)
        {
            if (getIsLoginResult.AsOk() == UserUtil.LoginType.User)
            {
                var isUserDataLoaded =
                    UserUtil.IsDataValid<DataTypes.Entity>() &&
                    UserUtil.IsDataValid<DataTypes.Action>() &&
                    UserUtil.IsDataValid<DataTypes.Stake>() &&
                    UserUtil.IsDataValid<DataTypes.NftCollection>(Env.Nfts.BOOM_COLLECTION_CANISTER_ID);

                if (isUserDataLoaded)// || (initialized.HasValue && initialized.Value))
                {
                    //initialized = true;

                    logInBtn.interactable = false;
                    logInBtn.interactable = true;
                    logInText.text = "LOGIN";
                    OnLoggedIn.Invoke();
                }
            }
        }
    }

    //

    public void CancelWalletIntegration()
    {
    }

    //private void LogoutUser()
    //{
    //    PlayerPrefs.SetString("authTokenId", string.Empty);
    //    Broadcast.Invoke<UserLogout>();
    //}

    //Login
    public void LogIn()
    {
        if (BroadcastState.TryRead<DataState<LoginData>>(out var dataState))
        {
            if (dataState.IsLoading()) return;
        }

        logInBtn.interactable = false;
        logInText.text = "LOGGING IN...";

        PlayerPrefs.SetString("walletType", "II");
        UserUtil.StartLogin("Logging In...");
    }
}