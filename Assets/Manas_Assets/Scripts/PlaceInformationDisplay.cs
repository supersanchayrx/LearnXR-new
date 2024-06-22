using UnityEngine;
using UnityEngine.UI;

public class PlaceInformationDisplay : MonoBehaviour
{
    public Text placeInformationText;

    public void DisplayPlaceInformation(string information)
    {
        Debug.Log($"Displaying Place Information: {information}");
        placeInformationText.text = information;
    }
}
