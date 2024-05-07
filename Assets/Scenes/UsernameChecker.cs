using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class UsernameChecker : MonoBehaviour
{
    public string baseUrl = "https://horizon-420212.el.r.appspot.com/checkusername?username=";
    public float checkInterval = 5f; // Check interval in seconds
    public TMP_InputField usernameInputField; // Reference to the TMP InputField for username
    public GameObject successObject; // Reference to the GameObject to activate on successful username
    public GameObject[] deactivateObjects; // Array of GameObjects to deactivate on successful username

    private bool checkingUsername = true; // Flag to control the loop

    private void Start()
    {
        // Start checking for the availability of the username when the script starts
        StartCoroutine(CheckUsernameAvailability());

        // Add event listener for the username input field
        usernameInputField.onSubmit.AddListener(OnSubmitUsername);
    }

    private void OnDisable()
    {
        // Stop checking for username availability when the script is disabled
        checkingUsername = false;
    }

    IEnumerator CheckUsernameAvailability()
    {
        // Continuously check the availability of the username
        while (checkingUsername)
        {
            // Get the username from the TMP InputField
            string username = usernameInputField.text;

            // Check if the username is empty, skip this iteration if it is
            if (string.IsNullOrEmpty(username))
            {
                yield return new WaitForSeconds(checkInterval);
                continue;
            }
            
            // Construct the URL for the API request
            string url = baseUrl + username;

            // Send a GET request to the API endpoint
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Allow insecure connections
                request.url = url;
                request.certificateHandler = new BypassCertificate();

                yield return request.SendWebRequest();

                // Check if the request was successful
                if (request.result == UnityWebRequest.Result.Success)
                {
                    // Get the response from the server
                    string response = request.downloadHandler.text;
                    Debug.Log(response); // Log the response received from the server

                    // Handle the response to determine if the username is available
                    if (response.Contains("available"))
                    {
                        Debug.Log("Username is available");
                        // Activate the successObject
                        successObject.SetActive(true);
                        // Deactivate the objects in deactivateObjects array
                        foreach (GameObject obj in deactivateObjects)
                        {
                            obj.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.Log("Username is not available");
                        // Display a message indicating that the username is not available
                    }
                }
                else
                {
                    // Log an error if the request fails
                    Debug.LogError("Error checking username: " + request.error);
                    // Display an error message
                }
            }

            // Wait for the specified interval before checking again
            yield return new WaitForSeconds(checkInterval);
        }
    }

    // Method to handle the submission of the username
    private void OnSubmitUsername(string username)
    {
        // Check the availability of the username when submitted
        StartCoroutine(CheckUsernameAvailability());
    }
}

// Certificate handler to bypass certificate validation
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Allow all certificates
        return true;
    }
}
