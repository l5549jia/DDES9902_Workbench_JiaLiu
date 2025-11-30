using UnityEngine;

public class TutorialFlow : MonoBehaviour
{
    public UIFader fader;
    public CanvasGroup tutorialGroup;
    public CanvasGroup consoleHintGroup;
    public float delayBeforeHint = 0.3f;

    public void OnUnderstandClicked()
    {
        // fade out tutorial
        fader.FadeOut(tutorialGroup);

        // delay a bit, then fade in console hint
        Invoke(nameof(ShowConsoleHint), delayBeforeHint);
    }

    void ShowConsoleHint()
    {
        fader.FadeIn(consoleHintGroup);
    }
}
