using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class EmergencyHandler : MonoBehaviour
{
    public Button emergencyButton;
    public TMP_InputField reasonInput;
    public TMP_InputField statusInput;
    public GameObject formPanel;

    private void Start()
    {
        emergencyButton.onClick.AddListener(OnEmergencyButtonClick);
        formPanel.SetActive(false);
    }

    private void OnEmergencyButtonClick()
    {
        formPanel.SetActive(true);
    }

    public void OnSubmitButtonClick()
    {
        StartCoroutine(SendEmergencyData());
    }

    private IEnumerator SendEmergencyData()
    {
        // Start service before querying location
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services are not enabled by the user.");
            yield break;
        }

        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;
            float altitude = Input.location.lastData.altitude;

            // Use the userId and emergencyId from the UserSession
            string userId = UserSession.UserId;
            string emergencyId = UserSession.EmergencyIds.Length > 0 ? UserSession.EmergencyIds[0] : ""; // Use the first emergency ID for simplicity.

            // Create the emergency data
            EmergencyData data = new EmergencyData
            {
                _id = emergencyId,
                emergencyLocation = new EmergencyLocation
                {
                    height = altitude,
                    lat = latitude,
                    lon = longitude
                },
                reason = reasonInput.text,
                status = statusInput.text,
                truthfulness = true,
                userId = userId
            };

            string jsonData = JsonUtility.ToJson(data);

            UnityWebRequest request = new UnityWebRequest("http://horizon-420212.el.r.appspot.com/emergency/create", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Emergency data sent successfully!");
            }
            else
            {
                Debug.Log("Error sending emergency data: " + request.error);
            }
        }

        // Stop location service if there is no need to query location updates continuously
        Input.location.Stop();
    }
}

[System.Serializable]
public class EmergencyData
{
    public string _id;
    public EmergencyLocation emergencyLocation;
    public string reason;
    public string status;
    public bool truthfulness;
    public string userId;
}

[System.Serializable]
public class EmergencyLocation
{
    public float height;
    public float lat;
    public float lon;
}
