using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            // Update progress bar or loading screen
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            yield return null;
        }
    }
}
