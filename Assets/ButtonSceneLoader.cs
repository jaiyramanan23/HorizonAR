using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneLoader : MonoBehaviour
{
    // Public variable to specify the scene to load
    public string sceneName;

    // Function to be called when the button is clicked
    public void LoadSceneOnClick()
    {
        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }
}
