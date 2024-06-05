using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;

public class playFabAuth : MonoBehaviour
{

    //public MoveToPortal portalScript;

    [Header("Testing")]
    //public TMP_Text messageText;
    public TMP_InputField emailInput, passwordInput;
    //public Button learnXRButton;
    public bool authSuccessful;
    


    private void Start()
    {
        authSuccessful = false;
    }
    public void RegisterUser()
    {
        var request = new RegisterPlayFabUserRequest {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError );

    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        //messageText.text = "Registered";
        authSuccessful = true;
        Debug.Log("SuccessfulRegistration");
        //portalScript.move = true;
        //portalScript.move = true;

    }

    void OnError(PlayFabError error)
    {
       // messageText.text = "Failed! Error Code = "+error.GenerateErrorReport();
        Debug.Log("Failed! Error Code = " + error.GenerateErrorReport());
    }
}
