using UnityEngine;
using Newtonsoft.Json;

public class ExampleUsage : MonoBehaviour
{
    private MicrophoneCapture micCapture;

    void Start()
    {
        micCapture = GetComponent<MicrophoneCapture>();
        if (micCapture == null)
        {
            Debug.LogError("MicrophoneCapture component not found!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            micCapture.StartRecording();
            Debug.Log("Recording started");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            micCapture.StopRecording();
            Debug.Log("Recording stopped");
           //  StartCoroutine(micCapture.ConvertAudioToText());
        }
    }
}
