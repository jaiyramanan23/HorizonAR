using UnityEngine;
using TMPro;
using System.Collections;
using System.Net;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text responseText;
    public GameObject loadingSpinner;
    private string baseURL = "http://horizon-420212.el.r.appspot.com/getuserprofile";

    public BusinessDetailsScript businessDetailsScript; // Reference to the BusinessDetailsScript

    public void OnLoginButtonClicked()
    {
        StartCoroutine(Login());
    }

    IEnumerator Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        loadingSpinner.SetActive(true);
        string url = $"{baseURL}?username={username}&password={password}";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string responseBody = reader.ReadToEnd();

                // Store response body in a file
                string filePath = Path.Combine(Application.persistentDataPath, "response.json");
                File.WriteAllText(filePath, responseBody);

                Debug.Log("Response Body: " + responseBody); // Debug logging

                // Parse the response body manually
                UserResponse userResponse = ParseResponseBody(responseBody);

                if (userResponse != null)
                {
                    Debug.Log("User ID: " + userResponse._id);
                    Debug.Log("Username: " + userResponse.userName);
                    Debug.Log("Role: " + userResponse.role);
                    // Add more properties as needed
                    Debug.Log("Title: " + userResponse.title);
                    Debug.Log("Full Name: " + userResponse.fullName);
                    Debug.Log("Contact Number: " + userResponse.contactNumber);
                    Debug.Log("Mail: " + userResponse.mail);
                    Debug.Log("Address: " + userResponse.address);

                    // Example of accessing connectedBusiness
                    foreach (ConnectedBusiness connectedBusiness in userResponse.connectedBusiness)
                    {
                        Debug.Log("Connected Business ID: " + connectedBusiness.businessId);
                        Debug.Log("Position: " + connectedBusiness.position);
                    }

                    // Proceed with your logic
                }
                else
                {
                    Debug.LogError("Failed to parse response."); // Debug logging
                    responseText.text = "Failed to parse response";
                }
            }
        }

        loadingSpinner.SetActive(false);
        yield return null;
    }

  private UserResponse ParseResponseBody(string responseBody)
{
    try
    {
        // Remove the parentheses from the response body
        responseBody = responseBody.Trim('(', ')');

        // Split the response body into key-value pairs
        string[] pairs = responseBody.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

        // Create a dictionary to store the key-value pairs
        Dictionary<string, object> dictionary = new Dictionary<string, object>();

        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim();
                string value = keyValue[1].Trim();

                // Handle nested objects like ConnectedBusiness
                if (value.StartsWith("["))
                {
                    // Parse the nested object manually
                    List<ConnectedBusiness> connectedBusinessList = ParseConnectedBusinessList(value);
                    dictionary[key] = connectedBusinessList;
                }
                else
                {
                    // Remove single quotes from the value
                    value = value.Trim('\'');
                    dictionary[key] = value;
                }
            }
        }

        // Create an instance of UserResponse and populate its properties
        UserResponse userResponse = new UserResponse();
        userResponse._id = (string)dictionary["_id"];
        userResponse.userName = (string)dictionary["userName"];
        userResponse.title = (string)dictionary["title"];
        userResponse.fullName = (string)dictionary["fullName"];
        userResponse.password = (string)dictionary["password"];
        userResponse.contactNumber = (string)dictionary["contactNumber"];
        userResponse.mail = (string)dictionary["mail"];
        userResponse.role = (string)dictionary["role"];
        userResponse.address = (string)dictionary["address"];
        userResponse.connectedBusiness = ((List<ConnectedBusiness>)dictionary["connectedBusiness"]).ToArray();

        // Handle emergencyId
        if (dictionary.ContainsKey("emergencyId"))
        {
            object emergencyIdValue = dictionary["emergencyId"];
            if (emergencyIdValue != null)
            {
                if (emergencyIdValue is string)
                {
                    userResponse.emergencyId = ((string)emergencyIdValue).Split(',');
                }
                else if (emergencyIdValue is List<string>)
                {
                    userResponse.emergencyId = ((List<string>)emergencyIdValue).ToArray();
                }
            }
        }

        userResponse.noOfFalseAttempts = int.Parse((string)dictionary["noOfFalseAttempts"]);

        return userResponse;
    }
    catch (Exception ex)
    {
        Debug.LogError("Error parsing response body: " + ex.Message);
        return null;
    }
}
    private List<ConnectedBusiness> ParseConnectedBusinessList(string value)
    {
        List<ConnectedBusiness> connectedBusinessList = new List<ConnectedBusiness>();

        // Remove the square brackets from the value
        value = value.Trim('[', ']');

        // Split the value into individual connected business entries
        string[] entries = value.Split("), ");

        foreach (string entry in entries)
        {
            string[] keyValues = entry.Trim('(', ')').Split(", ");
            ConnectedBusiness connectedBusiness = new ConnectedBusiness();

            foreach (string keyValue in keyValues)
            {
                string[] parts = keyValue.Split('=');
                string key = parts[0];
                string val = parts[1].Trim('\'');

                if (key == "businessId")
                {
                    connectedBusiness.businessId = val;
                }
                else if (key == "position")
                {
                    connectedBusiness.position = val;
                }
            }

            connectedBusinessList.Add(connectedBusiness);
        }

        return connectedBusinessList;
    }

    // Class to represent the deserialized response
    [System.Serializable]
    public class UserResponse
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
        // Add more properties as needed
        public ConnectedBusiness[] connectedBusiness;
        public string[] emergencyId;
        public int noOfFalseAttempts;
    }

    // Class to represent the connected business
    [System.Serializable]
    public class ConnectedBusiness
    {
        public string businessId;
        public string position;
    }
}