using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityAds : MonoBehaviour
{
    public float proximityThreshold = 50f; // Threshold distance for showing ads (in meters)
    public List<AdLocation> adLocations = new List<AdLocation>(); // List of ad locations with latitude, longitude, and ad content

    // Called when the script instance is being loaded
    private void Start()
    {
        StartCoroutine(UpdateProximityAds());
    }

    // Coroutine to periodically check proximity to ad locations
    private IEnumerator UpdateProximityAds()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Check proximity every 5 seconds

            // Get the actual location of the person
            LocationInfo location = Input.location.lastData;
            if (location.horizontalAccuracy < 100)
            {
                foreach (var adLocation in adLocations)
                {
                    float distance = CalculateDistance(location.latitude, location.longitude, adLocation.latitude, adLocation.longitude);

                    if (distance < proximityThreshold)
                    {
                        ShowAd(adLocation.adContent);
                    }
                }
            }
        }
    }

    // Calculate distance between two locations using haversine formula
    private float CalculateDistance(float lat1, float lon1, float lat2, float lon2)
    {
        float earthRadius = 6371; // Earth's radius in kilometers

        float lat1Radians = DegreesToRadians(lat1);
        float lon1Radians = DegreesToRadians(lon1);
        float lat2Radians = DegreesToRadians(lat2);
        float lon2Radians = DegreesToRadians(lon2);

        float latDifference = lat2Radians - lat1Radians;
        float lonDifference = lon2Radians - lon1Radians;

        float a = Mathf.Pow(Mathf.Sin(latDifference / 2), 2) +
                  Mathf.Cos(lat1Radians) * Mathf.Cos(lat2Radians) *
                  Mathf.Pow(Mathf.Sin(lonDifference / 2), 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        float distance = earthRadius * c * 1000; // Convert to meters

        return distance;
    }

    // Convert degrees to radians
    private float DegreesToRadians(float degrees)
    {
        return degrees * Mathf.PI / 180f;
    }

    // Method to show ad (in this example, just print the ad content)
    private void ShowAd(string adContent)
    {
        Debug.Log("Show Ad: " + adContent);
        // Here you would display the ad content on the screen
    }
}

// Class to represent an ad location with latitude, longitude, and ad content
[Serializable]
public class AdLocation
{
    public float latitude;
    public float longitude;
    public string adContent;
}
