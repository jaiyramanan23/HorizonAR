using UnityEngine;
using System.Collections;

public class GameObjectController : MonoBehaviour
{
   public GameObject panel1;
    public GameObject panel2;

    void Start()
    {
        // Start the coroutine to handle the panel switching
        StartCoroutine(PanelSwitching());
    }

    IEnumerator PanelSwitching()
    {
        // Wait for 5 seconds before showing the first panel
        yield return new WaitForSeconds(5f);

        // Show the first panel
        panel1.SetActive(true);

        // Wait for 5 seconds before hiding the first panel
        yield return new WaitForSeconds(5f);

        // Hide the first panel
        panel1.SetActive(false);

        // Wait for 1 minute before showing the second panel
        yield return new WaitForSeconds(55f);

        // Show the second panel
        panel2.SetActive(true);

        // Wait for 5 seconds before hiding the second panel
        yield return new WaitForSeconds(5f);

        // Hide the second panel

        // Restart the coroutine to repeat the process
        StartCoroutine(PanelSwitching());
    }
}