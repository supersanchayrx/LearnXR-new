using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiAnimations : MonoBehaviour
{
    public Animator CenterCanvas;
    public Animator LeftCanvas;
    public Animator RightCanvas;
    /*public Animator passInput;
    public Animator signinButton;*/
    public playFabAuth auth;
    bool localAuthBool;

    private void Start()
    {
        localAuthBool = true;
    }


    public void moveRight()
    {
        RightCanvas.Play("right");
        //passInput.Play("right");
    }

    public void moveLeft()
    {
        LeftCanvas.Play("left");
        //passText.Play("left");
    }

    public void moveUp()
    {
        CenterCanvas.Play("up");
    }


    private void Update()
    {
        if(auth.authSuccessful && localAuthBool)
        {
            localAuthBool = false;
            moveLeft();
            moveRight();
            moveUp();
        }
    }
}
