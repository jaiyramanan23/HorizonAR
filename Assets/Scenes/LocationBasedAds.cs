using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LocationBasedAds : MonoBehaviour
{
    private const string GoogleMapsAPIKey = "AIzaSyCr8-4oik-ZISKbcjPK4plEczTwic7xf8w";
    private const string GoogleMapsAPIURL = "https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}";

    public string targetLocation; // Name of the city or area
    public float range = 1000f; // Default range in meters

    private IEnumerator Start()
    {
        string url = string.Format(GoogleMapsAPIURL, targetLocation, GoogleMapsAPIKey);

        // Request to Google Maps API
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                // Parse JSON response
                string jsonResponse = www.downloadHandler.text;
                GoogleMapsResponse response = JsonUtility.FromJson<GoogleMapsResponse>(jsonResponse);

                if (response.status == "OK")
                {
                    float latitude = response.results[0].geometry.location.lat;
                    float longitude = response.results[0].geometry.location.lng;
                    Debug.Log("Location Coordinates: " + latitude + ", " + longitude);

                    // Show ads within the specified range
                    ShowAdsInRange(latitude, longitude);
                }
                else
                {
                    Debug.LogError("Google Maps API Error: " + response.status);
                }
            }
        }
    }

    private void ShowAdsInRange(float latitude, float longitude)
    {
        // Implement your ad logic here based on the latitude, longitude, and range
        Debug.Log("Showing ads within range for location: " + targetLocation);
    }
}

[System.Serializable]
public class GoogleMapsResponse
{
    public string status;
    public Result[] results;
}

[System.Serializable]
public class Result
{
    public Geometry geometry;
}

[System.Serializable]
public class Geometry
{
    public Location location;
}

[System.Serializable]
public class Location
{
    public float lat;
    public float lng;
}
