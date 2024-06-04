using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Collison Detected");
        if(this.gameObject.name.Equals("MoveTowards"))
        {
            if(other.CompareTag("Player"))
            {
               // SceneManager.LoadScene("ClassroomScene Test 1");
                SceneManager.LoadSceneAsync("ClassroomScene Test 1");
            }
        }
    }
}
