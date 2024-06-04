using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MoveToPortal : MonoBehaviour
{
    public bool move;
    public Transform moveTowards;
    [SerializeField] float time;
    //public playFabAuth auth;

    private void Start()
    {
        move = false;
    }
    private void Update()
    {
        //move = auth.authSuccessful;
        if(move)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTowards.position, time * Time.deltaTime);
            time = Mathf.MoveTowards(time, time + 25, Time.deltaTime * 20f);
        }
    }
}
