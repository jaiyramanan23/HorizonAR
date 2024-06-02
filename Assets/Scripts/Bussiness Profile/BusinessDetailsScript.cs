using UnityEngine;
using TMPro;
using System.Collections;
using System.Net;
using System.IO;

public class BusinessDetailsScript : MonoBehaviour
{
    public TMP_Text businessNameText;
    public TMP_Text addressText;
    public TMP_Text contactNumberText;
    public TMP_Text mailText;

    private string baseURL = "http://horizon-420212.el.r.appspot.com/business";

    public void GetBusinessDetails(string businessId)
    {
        StartCoroutine(GetBusinessDetailsCoroutine(businessId));
    }

    IEnumerator GetBusinessDetailsCoroutine(string businessId)
    {
        // Construct the URL with business ID
        string url = $"{baseURL}?businessId={businessId}";
        Debug.Log("Request URL: " + url); // Add this line to log the URL

        // Create a web request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        // Send the request
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            // Read the response
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream);
                string responseBody = reader.ReadToEnd();
                Debug.Log("Response Body: " + responseBody); // Add this line to log the response

                // Parse the JSON response
                BusinessDetails businessDetails = JsonUtility.FromJson<BusinessDetails>(responseBody);

                // Display the business details
                businessNameText.text = "Business Name: " + businessDetails.businessName;
                addressText.text = "Address: " + businessDetails.address;
                contactNumberText.text = "Contact Number: " + businessDetails.contactNumber;
                mailText.text = "Mail: " + businessDetails.mail;
            }
        }

        yield return null;
    }


    [System.Serializable]
    public class BusinessDetails
    {
        public string _id;
        public string businessName;
        public string address;
        public string contactNumber;
        public string mail;
        // Add other fields as needed
    }
}
