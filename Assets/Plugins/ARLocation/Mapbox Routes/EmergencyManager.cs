using UnityEngine;

public class EmergencyManager : MonoBehaviour
{
    public GameObject emergencyMarkerPrefab; // Prefab for the emergency marker

    public void ReportEmergency()
    {
        // Retrieve the user's location
        Vector2 userLocation = GetUserLocation();

        // Mark the user's location on the map
        MarkEmergencyLocation(userLocation);

        // Send the user's location to the API
        SendLocationToAPI(userLocation);
    }

    Vector2 GetUserLocation()
    {
        Vector2 userLocation = Vector2.zero;

        // Check if location services are enabled
        if (Input.location.isEnabledByUser)
        {
            // Start the location service updates
            Input.location.Start();

            // Wait until the location service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                // Wait for 1 second for the location service to initialize
                System.Threading.Thread.Sleep(1000);
                maxWait--;
            }

            // If initialization has failed or the max wait time is reached
            if (maxWait < 1 || Input.location.status == LocationServiceStatus.Failed)
            {
                Debug.LogError("Unable to determine device location.");
            }
            else
            {
                // Location service has initialized, get the user's location
                userLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            }

            // Stop the location service updates
            Input.location.Stop();
        }
        else
        {
            Debug.LogError("Location services are not enabled on this device.");
        }

        return userLocation;
    }

    void MarkEmergencyLocation(Vector2 location)
    {
        // Instantiate an emergency marker at the user's location
        GameObject emergencyMarker = Instantiate(emergencyMarkerPrefab, new Vector3(location.x, 0, location.y), Quaternion.identity);
    }

    void SendLocationToAPI(Vector2 location)
    {
        // Add your code here to send the user's location to an API
        // This could involve using the Mapbox API or any other method of communication
        // For demonstration purposes, let's just log the user's location
        Debug.Log("Emergency reported at location: " + location);
    }
}
