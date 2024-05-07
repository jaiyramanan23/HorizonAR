using UnityEngine;

public class EmergencyCall : MonoBehaviour
{
    public string emergencyNumber = "911"; // Change this to the appropriate emergency number for your region

    public void CallEmergencyNumber()
    {
        // Construct the phone number URI
        string phoneNumberURI = "tel:" + emergencyNumber;

        // Open the phone's dialer with the emergency number pre-filled
        Application.OpenURL(phoneNumberURI);
    }
}
