using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiAnimations : MonoBehaviour
{
    public Animator emailText;
    public Animator passText;
    public Animator emailInput;
    public Animator passInput;
    public Animator signinButton;
    public playFabAuth auth;


    public void moveRight()
    {
        emailInput.Play("right");
        passInput.Play("right");
    }

    public void moveLeft()
    {
        emailText.Play("left");
        passText.Play("left");
    }

    public void moveUp()
    {
        signinButton.Play("up");
    }


    private void Update()
    {
        if(auth.authSuccessful)
        {
            auth.authSuccessful = false;
            moveLeft();
            moveRight();
            moveUp();
        }
    }
}
