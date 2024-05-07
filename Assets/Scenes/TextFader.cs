using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextFader : MonoBehaviour
{
    public Text titleText;
    public Text descriptionText;
    public float fadeDuration = 1f;
    public float displayDuration = 1f;
    public string[] titles;
    public string[] descriptions;

    private int currentIndex = 0;

    void Start()
    {
        StartCoroutine(DisplayText());
    }

    IEnumerator DisplayText()
    {
        while (true)
        {
            // Fade out
            yield return FadeOut(titleText);
            yield return new WaitForSeconds(0.2f); // Delay between title and description
            yield return FadeOut(descriptionText);

            // Change text
            titleText.text = titles[currentIndex];
            descriptionText.text = descriptions[currentIndex];
            currentIndex = (currentIndex + 1) % titles.Length;

            // Fade in
            yield return FadeIn(titleText);
            yield return new WaitForSeconds(0.2f); // Delay between title and description
            yield return FadeIn(descriptionText);

            // Display duration
            yield return new WaitForSeconds(displayDuration);
        }
    }

    IEnumerator FadeOut(Text text)
    {
        float elapsedTime = 0;
        Color startColor = text.color;
        Color transparentColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(startColor, transparentColor, elapsedTime / fadeDuration);
            yield return null;
        }

        text.color = transparentColor;
    }

    IEnumerator FadeIn(Text text)
    {
        float elapsedTime = 0;
        Color startColor = text.color;
        Color opaqueColor = new Color(startColor.r, startColor.g, startColor.b, 1);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            text.color = Color.Lerp(startColor, opaqueColor, elapsedTime / fadeDuration);
            yield return null;
        }

        text.color = opaqueColor;
    }
}
