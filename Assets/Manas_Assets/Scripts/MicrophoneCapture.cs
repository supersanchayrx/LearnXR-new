using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class MicrophoneCapture : MonoBehaviour
{
    private AudioClip audioClip;
    private bool isRecording = false;
    private string fileName = "recordedAudio.wav"; 
    private string customDirectoryPath = "D:"; 
    private string whisperAPIEndpoint = "https://api.openai.com/v1/audio/transcriptions";
    private string apiKey = "sk-proj-XpJetuWlQU1R0rUmYMNGT3BlbkFJAhRl7ym2M6Ze4Nxkwiyv";

    public ChatAi chatAi; // Reference to ChatAi component
    public CoordinateSetter coordinateSetter; // Reference to CoordinateSetter component
    public PlaceInformationDisplay placeInformationDisplay; // Reference to PlaceInformationDisplay component
    public bool responseReceived;
    public string transcriptFileName = "transcription.txt";
    public string coordinatesFileName = "coordinates.txt";

    private void Start()
    {
        responseReceived = false;

        if (chatAi == null)
        {
            chatAi = FindObjectOfType<ChatAi>();
            if (chatAi == null)
            {
                Debug.LogError("ChatAi reference is missing.");
            }
        }

        if (coordinateSetter == null)
        {
            coordinateSetter = FindObjectOfType<CoordinateSetter>();
            if (coordinateSetter == null)
            {
                Debug.LogError("CoordinateSetter reference is missing.");
            }
        }

        if (placeInformationDisplay == null)
        {
            placeInformationDisplay = FindObjectOfType<PlaceInformationDisplay>();
            if (placeInformationDisplay == null)
            {
                Debug.LogError("PlaceInformationDisplay reference is missing.");
            }
        }
    }

    private void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        // Use keyboard inputs in the editor or standalone build
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRecording();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StopRecording();
        }
        #elif UNITY_ANDROID
        // Use Oculus controller buttons in the Android build (for Oculus Quest)
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            StartRecording();
        }

        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            StopRecording();
        }
        #endif
    }

    public void StartRecording()
    {
        if (isRecording) return;

        audioClip = Microphone.Start(null, false, 60, 44100);
        isRecording = true;
    }

    public void StopRecording()
    {
        if (!isRecording) return;

        Microphone.End(null);
        SaveWavFile(fileName, audioClip);
        isRecording = false;
        StartCoroutine(SendAudioToOpenAI());
    }

    private void SaveWavFile(string fileName, AudioClip audioClip)
    {
        if (audioClip == null) return;

        if (!Directory.Exists(customDirectoryPath))
        {
            Directory.CreateDirectory(customDirectoryPath);
        }

        string filePath = Path.Combine(customDirectoryPath, fileName);

        var samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        using (var writer = new BinaryWriter(fileStream))
        {
            int sampleRate = audioClip.frequency;
            int channels = audioClip.channels;
            int samplesLength = samples.Length;

            writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + samplesLength * 2);
            writer.Write(new char[4] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[4] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(sampleRate * channels * 2);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);
            writer.Write(new char[4] { 'd', 'a', 't', 'a' });
            writer.Write(samplesLength * 2);

            foreach (var sample in samples)
            {
                var intSample = (short)(sample * 32767);
                writer.Write(intSample);
            }
        }

        Debug.Log($"Saved WAV file to: {filePath}");
    }

    private IEnumerator SendAudioToOpenAI()
    {
        string filePath = Path.Combine(customDirectoryPath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Audio file not found at path: {filePath}");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("model", "whisper-1");
        byte[] audioData = File.ReadAllBytes(filePath);
        form.AddBinaryData("file", audioData, "recordedAudio.wav", "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post(whisperAPIEndpoint, form))
        {
            www.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                HandleApiResponse(jsonResponse);
            }
        }
    }

    private void HandleApiResponse(string jsonResponse)
    {
        var responseObj = JsonUtility.FromJson<WhisperResponse>(jsonResponse);
        string transcript = responseObj.text;

        Debug.Log($"Transcript: {transcript}");
        SaveTextToFile(transcript, transcriptFileName);

        IdentifyPlaceCoordinates(transcript);
    }

    private void SaveTextToFile(string text, string fileName)
    {
        string filePath = Path.Combine(customDirectoryPath, fileName);
        File.WriteAllText(filePath, text);
        Debug.Log($"Saved text to: {filePath}");
    }

    private async void IdentifyPlaceCoordinates(string transcription)
    {
        if (chatAi == null)
        {
            Debug.LogError("ChatAi reference is missing.");
            return;
        }

        string prompt = $"Identify the place name in the following text and provide its latitude and longitude coordinates: {transcription}";
        string response = await chatAi.IdentifyPlaceAndGetCoordinates(prompt);

        if (!string.IsNullOrEmpty(response))
        {
            Debug.Log($"Coordinates: {response}");
            SaveTextToFile(response, coordinatesFileName);
            ParseCoordinatesAndSetInCoordinateSetter(response);

            // Get place name from the response and request more information
            string placeName = ExtractPlaceNameFromResponse(response);
            if (!string.IsNullOrEmpty(placeName))
            {
                string placeInfo = await chatAi.GetPlaceInformation(placeName);
                if (!string.IsNullOrEmpty(placeInfo) && placeInformationDisplay != null)
                {
                    Debug.Log($"Place Information: {placeInfo}");
                    placeInformationDisplay.DisplayPlaceInformation(placeInfo);
                }
                else
                {
                    Debug.LogError("Failed to get place information or PlaceInformationDisplay is null.");
                }
            }
            else
            {
                Debug.LogError("Failed to extract place name.");
            }
        }
        else
        {
            Debug.LogError("Failed to get coordinates.");
        }
    }

    private string ExtractPlaceNameFromResponse(string response)
    {
        // Log the response to see what format it is in
        Debug.Log($"Extracting place name from response: {response}");

        // Assuming the place name is in the line starting with "Location:"
        string[] lines = response.Split('\n');
        foreach (var line in lines)
        {
            if (line.ToLower().Contains("location"))
            {
                return line.Split(':')[1].Trim();
            }
        }

        // If the above method fails, try other patterns
        foreach (var line in lines)
        {
            if (line.ToLower().Contains("place name"))
            {
                return line.Split(':')[1].Trim();
            }
        }

        // Log failure to extract place name for further debugging
        Debug.LogError("Failed to extract place name from the response.");
        return null;
    }

    private void ParseCoordinatesAndSetInCoordinateSetter(string coordinatesText)
    {
        Debug.Log($"Received Coordinates Text: {coordinatesText}");

        float latitude = 0f;
        float longitude = 0f;
        bool latParsed = false;
        bool lonParsed = false;
        string locationName = "Unknown Location";

        string[] lines = coordinatesText.Split('\n');

        foreach (var line in lines)
        {
            if (line.ToLower().Contains("latitude"))
            {
                string latStr = line.Split(':')[1].Trim().Replace("° N", "").Replace("° S", "");
                latParsed = float.TryParse(latStr, out latitude);
                if (line.Contains("S")) latitude = -latitude;
            }

            if (line.ToLower().Contains("longitude"))
            {
                string lonStr = line.Split(':')[1].Trim().Replace("° E", "").Replace("° W", "");
                lonParsed = float.TryParse(lonStr, out longitude);
                if (line.Contains("W")) longitude = -longitude;
            }

            if (line.ToLower().Contains("location"))
            {
                locationName = line.Split(':')[1].Trim();
            }

            if (!latParsed && !lonParsed)
            {
                string[] coords = line.Split(',');
                if (coords.Length == 2)
                {
                    latParsed = float.TryParse(coords[0].Trim(), out latitude);
                    lonParsed = float.TryParse(coords[1].Trim(), out longitude);
                }
            }
        }

        if (latParsed && lonParsed && coordinateSetter != null)
        {
            Debug.Log($"Parsed Coordinates - Latitude: {latitude}, Longitude: {longitude}, Location: {locationName}");
            coordinateSetter.SetCoordinates(latitude, longitude, locationName);
        }
        else
        {
            Debug.LogError("Failed to parse latitude and longitude.");
        }
    }
}

[System.Serializable]
public class WhisperResponse
{
    public string text;
}
