using UnityEngine;

public class EnvironmentFX : MonoBehaviour
{
    public Light sun;

    public Gradient ambientColor = new Gradient
    {
        colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(new Color(0.25f, 0.25f, 0.28f), 0f), 
            new GradientColorKey(new Color(0.95f, 0.98f, 1f), 1f)    
        }
    };

    public AnimationCurve sunIntensity = AnimationCurve.Linear(0f, 0.6f, 1f, 1.4f);

    public AudioSource ambience;

    public void UpdateEnvironment(float balance)
    {
        balance = Mathf.Clamp01(balance);

        RenderSettings.ambientLight = ambientColor.Evaluate(balance);

        if (sun) sun.intensity = sunIntensity.Evaluate(balance);

        if (ambience) ambience.volume = Mathf.Lerp(0.3f, 1f, balance);
    }
}
