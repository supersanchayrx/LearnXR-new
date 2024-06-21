using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BitScience
{
    public class ExampleStreetView : MonoBehaviour
    {
        [System.Serializable]
        public class ExampleStreetViewCoords
        {
            [SerializeField]
            public string location;
            [SerializeField]
            public float latitude;
            [SerializeField]
            public float longitude;
        }

        [SerializeField]
        private List<ExampleStreetViewCoords> _coordList;

        [SerializeField]
        private StreetViewCube _streetViewCube;

        [SerializeField]
        private Text _label;

        private int i = 0;

        void Start()
        {
            StartCoroutine(CycleLocations());
        }

        private IEnumerator CycleLocations()
        {
            while (true)
            {
                if (_coordList.Count > 0)
                {
                    i = (i + 1) % _coordList.Count;
                    _streetViewCube.Latitude = _coordList[i].latitude;
                    _streetViewCube.Longitude = _coordList[i].longitude;
                    _label.text = "Requesting: " + _coordList[i].location;

                    yield return new WaitForSeconds(10f);
                }
                else
                {
                    yield return null;
                }
            }
        }

        public void SetCoordinates(float latitude, float longitude, string locationName)
        {
            Debug.Log($"Updating coordinates to: {latitude}, {longitude} for {locationName}");
            StopCoroutine(CycleLocations());
            _streetViewCube.Latitude = latitude;
            _streetViewCube.Longitude = longitude;
            _label.text = "Requesting: " + locationName;
            StartCoroutine(UpdateStreetViewNow());
        }

        private IEnumerator UpdateStreetViewNow()
        {
            yield return _streetViewCube.StartCoroutine("UpdateStreetView");
        }
    }
}
