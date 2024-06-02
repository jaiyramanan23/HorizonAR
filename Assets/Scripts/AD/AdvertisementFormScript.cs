using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class AdvertisementFormScript : MonoBehaviour
{
    // Public fields to hold input values
    public TMP_InputField businessNameInput;
    public TMP_InputField contentInput;
    public TMP_InputField targetLocationInput;
    public Slider rangeSlider;
    public Button submitButton; // New submit button

    private string baseURL = "http://horizon-420212.el.r.appspot.com/createadvertisement";
    private string advertisementId; // Variable to store Advertisement ID

    private float targetLatitude;
    private float targetLongitude;
    private float maxLatitude;
    private float maxLongitude;
    private float minLatitude;
    private float minLongitude;

    private void Start()
    {
        // Add listener to the submit button
        submitButton.onClick.AddListener(SubmitAdvertisement);
    }

    private void SubmitAdvertisement()
    {
        // Get the target location input from the user
        string targetLocation = targetLocationInput.text;

        // Call Google Maps API to get the target latitude and longitude
        StartCoroutine(GetLocationFromGoogleMaps(targetLocation));
    }

    IEnumerator GetLocationFromGoogleMaps(string targetLocation)
    {
        // Construct the URL for the Google Maps Geocoding API request
        string googleMapsURL = "https://maps.googleapis.com/maps/api/geocode/json?address=" + WWW.EscapeURL(targetLocation);

        // You need to replace "yourGoogleMapsAPIKey" with your actual Google Maps API key
        googleMapsURL += "&key=";

        // Create a web request
        HttpWebRequest googleMapsRequest = (HttpWebRequest)WebRequest.Create(googleMapsURL);
        googleMapsRequest.Method = "GET";

        // Send the request and wait for the response
        using (HttpWebResponse response = (HttpWebResponse)googleMapsRequest.GetResponse())
        {
            // Read the response
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string jsonResponse = reader.ReadToEnd();

                // Parse the JSON response
                LocationData locationData = JsonUtility.FromJson<LocationData>(jsonResponse);

                // Check if the location was found
                if (locationData != null && locationData.results.Length > 0)
                {
                    targetLatitude = locationData.results[0].geometry.location.lat;
                    targetLongitude = locationData.results[0].geometry.location.lng;

                    // Calculate maximum and minimum latitude and longitude
                    CalculateMinMaxLatLngFromTarget();
                }
                else
                {
                    // Log an error or display a message indicating that the location was not found
                    Debug.LogError("Location not found: " + targetLocation);
                }
            }
        }

        yield return null;
    }

    private void CalculateMinMaxLatLngFromTarget()
    {
        // Get the range value from the slider
        float range = rangeSlider.value;

        // Convert range from meters to degrees (approximation)
        float degreesPerMeter = 1 / 111000f; // Approximation: 1 degree = 111 km
        float rangeInDegrees = range * degreesPerMeter;

        // Calculate maximum and minimum latitude and longitude
        maxLatitude = targetLatitude + Mathf.Abs(rangeInDegrees);
        minLatitude = targetLatitude - Mathf.Abs(rangeInDegrees);

        // Longitude calculation needs adjustment based on latitude
        float maxLonMultiplier = Mathf.Cos(Mathf.Deg2Rad * targetLatitude);
        float minLonMultiplier = Mathf.Cos(Mathf.Deg2Rad * targetLatitude);
        maxLongitude = targetLongitude + Mathf.Abs(rangeInDegrees) / maxLonMultiplier;
        minLongitude = targetLongitude - Mathf.Abs(rangeInDegrees) / minLonMultiplier;

        // Create advertisement using calculated values
        CreateAdvertisement();
    }

    private void CreateAdvertisement()
    {
        // Gather input values
        string businessName = businessNameInput.text;
        string content = contentInput.text;
        int range = (int)rangeSlider.value;

        // Create advertisement object
        AdvertisementForm advertisement = new AdvertisementForm()
        {
            businessName = businessName,
            content = content,
            latitude = targetLatitude,
            longitude = targetLongitude,
            maxLatitude = maxLatitude,
            maxLongitude = maxLongitude,
            minLatitude = minLatitude,
            minLongitude = minLongitude,
            range = range,
            contentType = "text",
            status = "running"
            // You can set other properties as needed
        };

        // Send the POST request
        StartCoroutine(SendAdvertisement(advertisement));
    }

    IEnumerator SendAdvertisement(AdvertisementForm advertisement)
    {
        // Convert advertisement object to JSON string
        string jsonAdvertisement = JsonUtility.ToJson(advertisement);

        // Create a web request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseURL);
        request.Method = "POST";
        request.ContentType = "application/json";

        // Convert the JSON string to bytes
        byte[] byteData = System.Text.Encoding.ASCII.GetBytes(jsonAdvertisement);
        request.ContentLength = byteData.Length;

        // Write data to the request stream
        using (Stream requestStream = request.GetRequestStream())
        {
            requestStream.Write(byteData, 0, byteData.Length);
        }

        // Send the request and get the response
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            // Log the response code
            Debug.Log("Response code: " + response.StatusCode);

            // You can handle the response as needed
            // Retrieve the Advertisement ID from the response
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream);
                string jsonResponse = reader.ReadToEnd();
                
                AdvertisementResponse responseObj = JsonUtility.FromJson<AdvertisementResponse>(jsonResponse);
                advertisementId = responseObj.advertisementId;
            }
        }

        // Now you have the Advertisement ID and can use it in further API requests

        yield return null;
    }

    [System.Serializable]
    public class AdvertisementForm
    {
        public string _id;
        public int billBoardId;
        public string businessName;
        public string closingDate;
        public string content;
        public string contentType;
        public float latitude;
        public float longitude;
        public float maxLatitude;
        public float maxLongitude;
        public float minLatitude;
        public float minLongitude;
        public int range;
        public string startingDate;
        public string status;
    }

    [System.Serializable]
    public class LocationData
    {
        public Result[] results;

        [System.Serializable]
        public class Result
        {
            public Geometry geometry;

            [System.Serializable]
            public class Geometry
            {
                public Location location;

                [System.Serializable]
                public class Location
                {
                    public float lat;
                    public float lng;
                }
            }
        }
    }

    [System.Serializable]
    public class AdvertisementResponse
    {
        public string advertisementId;
    }
}
