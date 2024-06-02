using UnityEngine;
using TMPro;
using System.Collections;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text responseText;
    public GameObject loadingSpinner;
    private string baseURL = "https://horizon-420212.el.r.appspot.com/getuserprofile"; // Updated to HTTPS

    public void OnLoginButtonClicked()
    {
        StartCoroutine(Login());
    }

    IEnumerator Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        loadingSpinner.SetActive(true);

        // Construct the URL with username and password
        string url = $"{baseURL}?username={username}&password={password}";

        // Use UnityWebRequest for better compatibility with Unity
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
                responseText.text = "Failed to connect to server.";
                loadingSpinner.SetActive(false);
                yield break;
            }

            string responseBody = webRequest.downloadHandler.text;

            // Handle JSON parsing and error handling
            UserResponse userResponse = JsonUtility.FromJson<UserResponse>(responseBody);
            if (userResponse != null && userResponse.user != null) // Check if userResponse and user are not null
            {
                Debug.Log("User ID: " + userResponse.user._id);
                Debug.Log("Username: " + userResponse.user.userName);
                Debug.Log("Role: " + userResponse.user.role);
                // Add more properties as needed

                // Handle connectedBusiness
                foreach (ConnectedBusiness connectedBusiness in userResponse.user.connectedBusiness)
                {
                    Debug.Log("Connected Business ID: " + connectedBusiness.businessId);
                    Debug.Log("Position: " + connectedBusiness.position);
                }

                // Proceed to the desired scene
                SceneManager.LoadScene("Mapbox Route With Search");
            }
            else
            {
                Debug.LogError("Failed to parse response.");
                responseText.text = "Failed to parse response";
            }
        }

        loadingSpinner.SetActive(false);
    }

    // Class to represent the deserialized response
    [Serializable]
    public class UserResponse
    {
        public UserProfile user;
        public string errorMessage;
    }

    [Serializable]
    public class UserProfile
    {
        public string _id;
        public string userName;
        public string title;
        public string fullName;
        public string password;
        public string contactNumber;
        public string mail;
        public string role;
        public string address;
        public ConnectedBusiness[] connectedBusiness;
        public string[] emergencyId;
        public int noOfFalseAttempts;
    }

    // Class to represent the connected business
    [Serializable]
    public class ConnectedBusiness
    {
        public string businessId;
        public string position;
    }
}
