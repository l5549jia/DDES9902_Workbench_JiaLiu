using System;
using UnityEngine;

public class TimeBalance : MonoBehaviour
{
    [Range(0f, 600f)] public float studyTime = 300f;
    [Range(0f, 600f)] public float restTime = 300f;

    public TreeFeedback[] treeFeedbacks;

    [Range(0f, 1f)] public float lowThreshold = 0.7f;  
    [Range(0f, 1f)] public float highThreshold = 0.9f; 
    public float stableSecondsTarget = 10f;         

    [Range(0f, 1f)] public float balanceIndex;          

    public EnvironmentFX envFX;

    float stableTimer = 0f;
    bool hasFlourished = false;

    public Action OnAnyUserInput;

    void Update()
    {
        float total = Mathf.Max(studyTime + restTime, 1f);
        balanceIndex = 1f - Mathf.Abs(studyTime - restTime) / total;
        balanceIndex = Mathf.Clamp01(balanceIndex);

        if (treeFeedbacks != null)
        {
            foreach (var tf in treeFeedbacks)
                if (tf) tf.UpdateGarden(balanceIndex);
        }

        if (balanceIndex > lowThreshold && balanceIndex < highThreshold)
        {
            stableTimer += Time.deltaTime;
            if (!hasFlourished && stableTimer >= stableSecondsTarget)
            {
                hasFlourished = true;
                if (treeFeedbacks != null)
                {
                    foreach (var tf in treeFeedbacks)
                        if (tf) tf.FlowerBloom();
                }
            }
        }
        else
        {
            stableTimer = 0f;
        }

        if (envFX) envFX.UpdateEnvironment(balanceIndex);
    }

    public void AddStudy(float delta)
    {
        studyTime = Mathf.Clamp(studyTime + delta, 0f, 600f);
        NotifyUserInput();
    }

    public void AddRest(float delta)
    {
        restTime = Mathf.Clamp(restTime + delta, 0f, 600f);
        NotifyUserInput();
    }

    public void SetBalanced()
    {
        studyTime = 300f;
        restTime = 300f;
        NotifyUserInput();
    }

    public void NotifyUserInput()
    {
        OnAnyUserInput?.Invoke();
    }
}
