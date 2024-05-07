using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class FormController : MonoBehaviour
{
    public TMP_InputField idField;
    public TMP_InputField addressField;
    public TMP_InputField contactNumberField;
    public TMP_InputField fullNameField;
    public TMP_InputField mailField;
    public TMP_InputField passwordField;
    public TMP_InputField roleField;
    public TMP_InputField titleField;
    public TMP_InputField userNameField;

    private string apiUrl = "http://horizon-420212.el.r.appspot.com/create";

    public void SubmitForm()
    {
        FormObject formObject = new FormObject(
            GetValueOrDefault(idField),
            GetValueOrDefault(addressField),
            GetValueOrDefault(contactNumberField),
            GetValueOrDefault(fullNameField),
            GetValueOrDefault(mailField),
            GetValueOrDefault(passwordField),
            GetValueOrDefault(roleField),
            GetValueOrDefault(titleField),
            GetValueOrDefault(userNameField)
        );

        StartCoroutine(SendFormData(formObject));
    }

private string GetValueOrDefault(TMP_InputField inputField)
{
    return inputField != null && !string.IsNullOrEmpty(inputField.text) ? inputField.text : null;
}


    IEnumerator SendFormData(FormObject formObject)
    {
        string jsonData = JsonUtility.ToJson(formObject);

        Debug.Log("Sending form data: " + jsonData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Form data sent successfully!");
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error sending form data: " + request.error);
                Debug.LogError("Response code: " + request.responseCode);
            }
        }
    }

    [System.Serializable]
    public class FormObject
    {
        public string _id;
        public string address;
        public ConnectedBusiness[] connectedBusiness;
        public string contactNumber;
        public string[] emergencyId;
        public string fullName;
        public string mail;
        public int noOfFalseAttempts;
        public string password;
        public string role;
        public string title;
        public string userName;

        public FormObject(string _id, string address, string contactNumber, string fullName, string mail, string password, string role, string title, string userName)
        {
            this._id = _id;
            this.address = address;
            this.connectedBusiness = new ConnectedBusiness[0];
            this.contactNumber = contactNumber;
            this.emergencyId = new string[0];
            this.fullName = fullName;
            this.mail = mail;
            this.noOfFalseAttempts = 0;
            this.password = password;
            this.role = role;
            this.title = title;
            this.userName = userName;
        }
    }

    [System.Serializable]
    public class ConnectedBusiness
    {
        public string businessId;
        public string position;
    }
}
