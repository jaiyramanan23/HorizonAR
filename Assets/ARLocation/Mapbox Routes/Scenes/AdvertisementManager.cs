using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;

public class AdvertisementManager : MonoBehaviour
{
    public Canvas adCanvas;
    public TextMeshProUGUI adContentText;
    public TextMeshProUGUI businessNameText;
    private bool isInRange = false;
    private Vector2 userLocation;
    private Coroutine fetchAdsCoroutine;
    private const string adsApiUrl = "http://horizon-420212.el.r.appspot.com/getadsforuser?";
    private const float checkInterval = 5f; // Check for user location every 10 seconds

    void Start()
    {
        StartCoroutine(UpdateUserLocation());
    }

    IEnumerator UpdateUserLocation()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(checkInterval);
            // Get user's current location
            yield return StartCoroutine(GetUserLocation());
            // Check if user is in range of any advertisement
            CheckInRange(); // Moved outside of GetUserLocation coroutine
            if (isInRange)
            {
                if (fetchAdsCoroutine == null)
                {
                    fetchAdsCoroutine = StartCoroutine(FetchAds());
                }
            }
            else
            {
                // User is out of range, deactivate ad canvas
                adCanvas.gameObject.SetActive(false);
                if (fetchAdsCoroutine != null)
                {
                    StopCoroutine(fetchAdsCoroutine);
                    fetchAdsCoroutine = null;
                }
            }
        }
    }

    IEnumerator GetUserLocation()
    {
        // Use Google Maps API to get user's current location
        // Replace with your own method to fetch user location
        UnityWebRequest www = UnityWebRequest.Get("https://maps.googleapis.com/maps/api/geocode/json?latlng=" + Input.location.lastData.latitude + "," + Input.location.lastData.longitude + "&key=YOUR_API_KEY");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Failed to get user location: " + www.error);
            yield break;
        }

        string jsonResult = www.downloadHandler.text;
        // Parse JSON to get user location
        // Example: userLocation = ParseUserLocation(jsonResult);
        // For simplicity, assuming user location is received successfully
        userLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
    }

    void CheckInRange()
    {
        // Check if user is within range of any advertisement location
        // For simplicity, assuming user is always in range
        isInRange = true;
    }

    IEnumerator FetchAds()
    {
        // Construct the API request URL
        string adsApiRequest = adsApiUrl + "latitude=" + userLocation.x + "&longitude=" + userLocation.y;

        // Debug log the API request URL
        Debug.Log("API Request: " + adsApiRequest);

        // Create a UnityWebRequest to fetch advertisements from the backend
        UnityWebRequest adsRequest = UnityWebRequest.Get(adsApiRequest);
        yield return adsRequest.SendWebRequest();

        // Check if there's an error with the request
        if (adsRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch ads: " + adsRequest.error);
            yield break;
        }

        // Parse the JSON response to get advertisement details
        string jsonResponse = adsRequest.downloadHandler.text;
        Debug.Log("Backend Response: " + jsonResponse);

        // Check if the JSON response is empty
        if (string.IsNullOrEmpty(jsonResponse))
        {
            Debug.LogWarning("Empty JSON response.");
            adCanvas.gameObject.SetActive(false); // Deactivate the canvas if no ads are found
            yield break;
        }

        // Deserialize JSON array properly
        Advertisement[] ads = JsonHelper.FromJsonArray<Advertisement>(jsonResponse);

        // Check if the deserialization was successful
        if (ads == null || ads.Length == 0)
        {
            Debug.LogWarning("No advertisements found.");
            adCanvas.gameObject.SetActive(false); // Deactivate the canvas if no ads are found
            yield break;
        }

        // Display the first advertisement
        Advertisement ad = ads[0];
        Debug.Log("Ad Content: " + ad.content);
        Debug.Log("Business Name: " + ad.businessName);
        adCanvas.gameObject.SetActive(true);
        adContentText.text = ad.content;
        businessNameText.text = ad.businessName;
    }

    // Data structure to represent advertisement
    [System.Serializable]
    public class Advertisement
    {
        public string _id;
        public string businessId;
        public int billBoardId;
        public float latitude;
        public float longitude;
        public float range;
        public float minLatitude;
        public float maxLatitude;
        public float minLongitude;
        public float maxLongitude;
        public string businessName;
        public string startingDate;
        public string closingDate;
        public string status;
        public string contentType;
        public string content;
    }

    // Helper class to deserialize JSON
    public static class JsonHelper
    {
        // Deserialize JSON array properly
        public static T[] FromJsonArray<T>(string json)
        {
            string newJson = "{\"Items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.Items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
