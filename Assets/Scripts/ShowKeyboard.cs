using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEngine.UIElements;

public class ShowKeyboard : MonoBehaviour
{
    public TMP_InputField inputField1;
    [SerializeField] float distance;
    [SerializeField] float verticalOffset;
    public Transform positionSource;

    //public TMP_InputField passwordInput;

    void Start()
    {
        inputField1.onSelect.AddListener(x => openKeyBoard());
        //passwordInput.onSelect.AddListener(x => openKeyBoard(passwordInput));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void openKeyBoard()
    {
        NonNativeKeyboard.Instance.InputField = inputField1;
        NonNativeKeyboard.Instance.PresentKeyboard(inputField1.text);

        Vector3 direction = positionSource.forward;
        direction.y = 0;
        direction.Normalize();

        Vector3 targerPos = positionSource.position + direction * distance + Vector3.up * verticalOffset;

        NonNativeKeyboard.Instance.RepositionKeyboard(targerPos);
    }
}
