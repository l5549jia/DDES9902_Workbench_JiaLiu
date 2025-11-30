using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIFader : MonoBehaviour
{
    public float fadeDuration = 1.5f;

    public void FadeOut(CanvasGroup group)
    {
        StartCoroutine(Fade(group, 1f, 0f));
    }

    public void FadeIn(CanvasGroup group)
    {
        StartCoroutine(Fade(group, 0f, 1f));
    }

    private IEnumerator Fade(CanvasGroup group, float from, float to)
    {
        float t = 0;
        group.alpha = from;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        group.alpha = to;

        if (to == 0f)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }
        else
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }
    }
}
