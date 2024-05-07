using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NavigationController : MonoBehaviour
{
    public InputField searchInputField;
    public GameObject arrowPrefab; // Prefab for the arrow model
    public Transform arCameraTransform; // Transform of the AR camera

    private string apiKey = "AIzaSyCVecLmTxv_KPDzueiLtW9Bz1PpIQR-tQs";

    public void SearchLocation()
    {
        string searchQuery = searchInputField.text;
        StartCoroutine(GetLocationCoordinates(searchQuery));
    }

    IEnumerator GetLocationCoordinates(string searchQuery)
    {
        string url = "https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input=" 
                     + UnityWebRequest.EscapeURL(searchQuery) 
                     + "&inputtype=textquery&fields=geometry&key=" + apiKey;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch location coordinates: " + www.error);
                yield break;
            }

            // Parse the JSON response to get the location coordinates
            LocationData locationData = JsonUtility.FromJson<LocationData>(www.downloadHandler.text);

            // Extract latitude and longitude from the response
            double latitude = locationData.candidates[0].geometry.location.lat;
            double longitude = locationData.candidates[0].geometry.location.lng;

            // Get directions to the location
            StartCoroutine(GetDirections(latitude, longitude));
        }
    }

    IEnumerator GetDirections(double destinationLatitude, double destinationLongitude)
    {
        // Get current location of the device
        Vector2 currentLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);

        // Construct URL for Directions API
        string url = "https://maps.googleapis.com/maps/api/directions/json?origin=" 
                     + currentLocation.x + "," + currentLocation.y 
                     + "&destination=" + destinationLatitude + "," + destinationLongitude 
                     + "&key=" + apiKey;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch directions: " + www.error);
                yield break;
            }

            // Parse the JSON response to get directions
            DirectionsData directionsData = JsonUtility.FromJson<DirectionsData>(www.downloadHandler.text);

            // Extract polyline data from the response and decode it
            string polyline = directionsData.routes[0].overview_polyline.points;
            Vector2[] routePoints = PolylineDecoder.Decode(polyline);

            // Draw the route in AR space using AR Foundation
            DrawRoute(routePoints);

            // Show arrow indicating direction
            ShowDirectionArrow(routePoints[1]);
        }
    }

    void DrawRoute(Vector2[] routePoints)
    {
        // Code to draw the route in AR space using AR Foundation
    }

    void ShowDirectionArrow(Vector2 directionPoint)
    {
        // Calculate direction vector from current camera position to the next point
        Vector3 direction = new Vector3(directionPoint.x, arCameraTransform.position.y, directionPoint.y) 
                            - arCameraTransform.position;

        // Instantiate arrow prefab and rotate it to face the direction
        GameObject arrow = Instantiate(arrowPrefab, arCameraTransform.position, Quaternion.LookRotation(direction));
    }
}

[System.Serializable]
public class LocationData
{
    public Candidate[] candidates;
}

[System.Serializable]
public class Candidate
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
    public double lat;
    public double lng;
}

[System.Serializable]
public class DirectionsData
{
    public Route[] routes;
}

[System.Serializable]
public class Route
{
    public OverviewPolyline overview_polyline;
}

[System.Serializable]
public class OverviewPolyline
{
    public string points;
}
