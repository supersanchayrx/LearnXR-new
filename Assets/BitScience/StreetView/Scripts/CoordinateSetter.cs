using UnityEngine;
using BitScience;

public class CoordinateSetter : MonoBehaviour
{
    [SerializeField] private ExampleStreetView exampleStreetView;

    private void Start()
    {
        if (exampleStreetView == null)
        {
            exampleStreetView = FindObjectOfType<ExampleStreetView>();
            if (exampleStreetView == null)
            {
                Debug.LogError("ExampleStreetView reference is missing.");
            }
        }
    }

    public void SetCoordinates(float latitude, float longitude, string locationName)
    {
        Debug.Log($"Setting coordinates: {latitude}, {longitude}, {locationName}");
        exampleStreetView.SetCoordinates(latitude, longitude, locationName);
    }
}
