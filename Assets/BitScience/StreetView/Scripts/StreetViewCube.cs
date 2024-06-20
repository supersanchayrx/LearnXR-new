using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System;

namespace BitScience
{
    public class StreetViewCube : MonoBehaviour
    {
        #region Consts
        // 110,574.61 Meters / 1 Degree Latitude
        private const float LAT_DEGREE_METERS = 110574.61f;
        // 111,302.62 Meters / 1 Degree Longitude
        private const float LONG_DEGREE_METERS = 111302.62f;
        #endregion

        #region Enums
        // Which side of the cubemap
        private enum Face
        {
            FRONT,
            BACK,
            LEFT,
            RIGHT,
            UP,
            DOWN
        }
        #endregion

        #region Inspector Fields
        [SerializeField]
        [Tooltip("You vave to register for a Google Street View API")]
        private string _key = "";

        [SerializeField]
        [Tooltip("Cubemap to show the streetview on")]
        private Cubemap _cubeMap;

        [SerializeField]
        [Tooltip("Cubemap to show the streetview on")]
        private Material _material;

        [SerializeField]
        [Tooltip("Current Max Supported Size is 512")]
        private int _size = 512;

        [SerializeField]
        [Tooltip("Latitude to retrieve the Street View From")]
        private float _lat = -23.5834057f;

        [SerializeField]
        [Tooltip("Longitude to retrieve the Street View From")]
        private float _long = -46.681568f;

        [SerializeField]
        [Tooltip("Minimum time between requests")]
        private float _updateDelay = 2.0f;

        [SerializeField]
        [Tooltip("Enter Min distance in Meters (We'll convert to Lat/Long)")]
        private float _updateDist = 1f;

        [SerializeField]
        [Tooltip("Which way is 'North' in your project")]
        private int _heading = 0;

        [SerializeField]
        [Tooltip("Show Debug Logs")]
        private bool _debugOutput = false;
        #endregion

        #region Public Properties
        public int Size
        {
            get { return _size; }
        }

        public int FOV
        {
            get { return _fov; }
        }

        public int Heading
        {
            get { return _heading; }
        }

        public string APIKey
        {
            get { return _key; }
        }

        public float Latitude
        {
            get { return _lat; }
            set { _lat = value; }
        }

        public float Longitude
        {
            get { return _long; }
            set { _long = value; }
        }
        #endregion

        #region Private Fields
        private string _apiRoot = "https://maps.googleapis.com/maps/api/streetview?";
        private int _fov = 90;
        private Dictionary<Face, Texture2D> _faces;
        private float _prevLat;
        private float _prevLong;
        private Cubemap _tempCubeMap;
        #endregion

        #region Behavriour Overrides
        void Start()
        {
            // set the cubemap size to the correct size
            ResizeTextureSize(Size);
            
            // Start the update coroutine
            StartCoroutine(UpdateStreetView());

        }
        #endregion

        #region Property Assign Functions
        //Really expensive because we need to release all the cached images.
        public void ResizeTextureSize(int newSize)
        {
            _size = newSize;
            _tempCubeMap = new Cubemap(_size, TextureFormat.RGB24, false);

            if (_faces != null && _faces.Count > 0)
            {
                foreach (KeyValuePair<Face, Texture2D> face in _faces)
                {
                    GameObject.DestroyImmediate(face.Value);
                }

                System.GC.Collect();
            }

            // allocate a Dictionary (To clear out any old sized Textures)
            _faces = new Dictionary<Face, Texture2D>();
        }

        #endregion

        #region Pulling Data
        // Coroutine for puloling the streetview data
        private IEnumerator UpdateStreetView()
        {
            float timeTillUpdate = 0.0f;
            while (true)
            {
                timeTillUpdate -= Time.deltaTime;
                if (timeTillUpdate <= 0.0f)
                {
                    // 110,574.61 Meters / 1 Degree Latitude
                    // 111,302.62 Meters / 1 Degree Longitude
                    if (true || Mathf.Abs(_prevLat - _lat) > _updateDist / LAT_DEGREE_METERS || Mathf.Abs(_prevLong - _long) > _updateDist / LONG_DEGREE_METERS)
                    {
                        //Pull the latest Images.
                        yield return StartCoroutine(DownloadStreetView());
                        timeTillUpdate = _updateDelay;
                        _prevLat = _lat;
                        _prevLong = _long;
                    }
                    else
                    {
                        if (_debugOutput)
                            Debug.Log("Not Far Enough Away.   Skipping Update");
                    }
                }
                else
                {
                    if (_debugOutput)
                        Debug.Log("Waiting Till next Update.");
                }
                yield return null;
            }
        }

        //Download all of the Street View Images
        private IEnumerator DownloadStreetView()
        {
            if (_debugOutput)
                Debug.Log("Starting Update.");

            if (string.IsNullOrEmpty(_key))
            {
                Debug.LogError("Please enter an API key for Google Street View Access.  ");
                yield break;
            }

            //string ExampleUrl = "https://maps.googleapis.com/maps/api/streetview?size=512x512&location=-23.5834057,-46.681568&fov=180&heading=270&pitch=0&key=[GOOGLE_API_KEY_HERE]";
            string frontRequest = _apiRoot + "size=" + _size + "x" + _size + "&location=" + _lat + "," + _long + "&fov=" + _fov + "&heading=" + (_heading + 0) + "&key=" + _key;
            string rightRequest = _apiRoot + "size=" + _size + "x" + _size + "&location=" + _lat + "," + _long + "&fov=" + _fov + "&heading=" + (_heading + 90) + "&key=" + _key;
            string backRequest = _apiRoot + "size=" + _size + "x" + _size + "&location=" + _lat + "," + _long + "&fov=" + _fov + "&heading=" + (_heading + 180) + "&key=" + _key;
            string leftRequest = _apiRoot + "size=" + _size + "x" + _size + "&location=" + _lat + "," + _long + "&fov=" + _fov + "&heading=" + (_heading + 270) + "&key=" + _key;
            string upRequest = _apiRoot + "size=" + _size + "x" + _size + "&location=" + _lat + "," + _long + "&fov=" + _fov + "&heading=" + (_heading + 0) + "&pitch=90" + "&key=" + _key;
            string downRequest = _apiRoot + "size=" + _size + "x" + _size + "&location=" + _lat + "," + _long + "&fov=" + _fov + "&heading=" + (_heading + 0) + "&pitch=-90" + "&key=" + _key;

            // Sending requests, pausing to make sure a frame passes before the next request is made to eliminate visual glitches
            yield return StartCoroutine(handleresponse(frontRequest, Face.FRONT));
            yield return null;
            yield return StartCoroutine(handleresponse(rightRequest, Face.RIGHT));
            yield return null;
            yield return StartCoroutine(handleresponse(backRequest, Face.BACK));
            yield return null;
            yield return StartCoroutine(handleresponse(leftRequest, Face.LEFT));
            yield return null;
            yield return StartCoroutine(handleresponse(upRequest, Face.UP));
            yield return null;
            yield return StartCoroutine(handleresponse(downRequest, Face.DOWN));
            yield return null;

            CombineSides();

            _material.SetTexture("_Cube", _tempCubeMap);
        }

        //Handle Response from the Street View API
        private IEnumerator handleresponse(string request, Face face)
        {
            WWW response = new WWW(request);

            while (!response.isDone)
            {
                yield return null;
            }

            if (response.error == null)
            {
                if (_debugOutput)
                    Debug.Log("Assign Texture From Response for: " + face.ToString());

                if (_faces.ContainsKey(face))
                {
                    CopyTexture(response.texture, _faces[face]);
                }
                else
                {
                    _faces.Add(face, response.texture);
                }
            }
            else
            {
                Debug.LogError("BitScience - StreetView Error: " + response.error);
            }
        }
        #endregion

        #region Texture Management
        //Combine all of the sides of the cubemap
        private void CombineSides()
        {
            foreach (KeyValuePair<Face, Texture2D> face in _faces)
            {
                switch (face.Key)
                {
                    case Face.FRONT: AssignCubemapFace(face.Value, CubemapFace.PositiveZ); break;
                    case Face.RIGHT: AssignCubemapFace(face.Value, CubemapFace.PositiveX); break;
                    case Face.BACK: AssignCubemapFace(face.Value, CubemapFace.NegativeZ); break;
                    case Face.LEFT: AssignCubemapFace(face.Value, CubemapFace.NegativeX); break;
                    case Face.UP: AssignCubemapFace(face.Value, CubemapFace.NegativeY); break;
                    case Face.DOWN: AssignCubemapFace(face.Value, CubemapFace.PositiveY); break;
                }
            }
        }

        //Copy pixels from one texture to another
        private void CopyTexture(Texture2D origin, Texture2D dest)
        {
            for (int i = 0; i < origin.width; i++)
            {
                for (int j = 0; j < origin.height; j++)
                {
                    Color pixel = origin.GetPixel(i, j);
                    dest.SetPixel(i, j, pixel);
                }
            }

            dest.Apply();
        }

        //Reads the pixels in from the side, and assigns it to the proper face of the cubemap
        private void AssignCubemapFace(Texture2D side, CubemapFace cubemapFace)
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    Color pixel = side.GetPixel(i, j);
                    _tempCubeMap.SetPixel(cubemapFace, i, j, pixel);
                }
            }

            _tempCubeMap.Apply();
        }
        #endregion
    }
}