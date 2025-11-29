using System;
using UnityEngine;
using TMPro;

public class TimeBalance : MonoBehaviour
{
    [Header("Time (internal units 0–600 → 0–24h)")]
    [Range(0f, 600f)] public float studyTime = 300f;
    [Range(0f, 600f)] public float restTime = 300f;

    [Header("Tree & Environment Feedback")]
    public TreeFeedback[] treeFeedbacks;
    [Range(0f, 1f)] public float lowThreshold = 0.7f;
    [Range(0f, 1f)] public float highThreshold = 0.9f;
    public float stableSecondsTarget = 10f;

    [Range(0f, 1f)] public float balanceIndex;
    public EnvironmentFX envFX;

    float stableTimer = 0f;
    bool hasFlourished = false;

    [Header("Main UI Display (in hours)")]
    public TextMeshProUGUI studyText;
    public TextMeshProUGUI restText;
    public TextMeshProUGUI totalText;

    [Header("Unit ↔ Hour Mapping")]
    [Tooltip("Maximum hours in a day (for UI display)")]
    public float maxHoursPerDay = 24f;

    [Tooltip("Maximum internal units (same as the slider range)")]
    public float maxUnits = 600f;

    public Action OnAnyUserInput;

    /// <summary>
    /// Number of internal units per hour (600/24 = 25)
    /// </summary>
    public float UnitsPerHour => maxUnits / Mathf.Max(maxHoursPerDay, 0.0001f);

    /// <summary>Convert hours → internal units</summary>
    public float HoursToUnits(float hours) =>
        hours * UnitsPerHour;

    /// <summary>Convert internal units → hours</summary>
    public float UnitsToHours(float units) =>
        units / UnitsPerHour;

    // ====================================================================
    // Yesterday Input (V3)
    // ====================================================================

    [Header("Yesterday Routine UI")]
    public TextMeshProUGUI yStudyText;
    public TextMeshProUGUI yRestText;
    public TextMeshProUGUI yesterdayFeedbackText;

    [HideInInspector] public float yStudyHours = 0f;
    [HideInInspector] public float yRestHours = 0f;

    [Tooltip("Step size when adjusting yesterday's hours (1 = 1 hour)")]
    public float yesterdayStepHours = 1f;

    // ====================================================================

    void Start()
    {
        UpdateUI();
        UpdateYesterdayTexts();
    }

    void Update()
    {
        // Calculate balance index
        float totalUnits = Mathf.Max(studyTime + restTime, 1f);
        balanceIndex = 1f - Mathf.Abs(studyTime - restTime) / totalUnits;
        balanceIndex = Mathf.Clamp01(balanceIndex);

        // Update trees
        if (treeFeedbacks != null)
        {
            foreach (var tf in treeFeedbacks)
                if (tf) tf.UpdateGarden(balanceIndex);
        }

        // Check for stable balanced state
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

        // Update environment
        if (envFX)
            envFX.UpdateEnvironment(balanceIndex);
    }

    // ====================================================================
    // Current Study/Rest Time Controls (V2)
    // ====================================================================

    public void AddStudy(float delta)
    {
        studyTime = Mathf.Clamp(studyTime + delta, 0f, maxUnits);
        NotifyUserInput();
        UpdateUI();
    }

    public void AddRest(float delta)
    {
        restTime = Mathf.Clamp(restTime + delta, 0f, maxUnits);
        NotifyUserInput();
        UpdateUI();
    }

    /// <summary>Set both study and rest to balanced midpoint (12 hours each)</summary>
    public void SetBalanced()
    {
        float half = maxUnits * 0.5f; // 300 units = 12h
        studyTime = half;
        restTime = half;

        NotifyUserInput();
        UpdateUI();
    }

    public void NotifyUserInput()
    {
        OnAnyUserInput?.Invoke();
    }

    /// <summary>Refresh Study/Rest/Total UI display</summary>
    public void UpdateUI()
    {
        float studyHours = UnitsToHours(studyTime);
        float restHours = UnitsToHours(restTime);
        float totalHours = studyHours + restHours;

        if (studyText)
            studyText.text = $"Study: {studyHours:0} h";

        if (restText)
            restText.text = $"Rest: {restHours:0} h";

        if (totalText)
            totalText.text = $"Total: {totalHours:0} h";
    }

    // ====================================================================
    // Yesterday Routine Controls (V3)
    // ====================================================================

    public void YStudyPlus()
    {
        yStudyHours = Mathf.Clamp(yStudyHours + yesterdayStepHours, 0f, maxHoursPerDay);
        UpdateYesterdayTexts();
    }

    public void YStudyMinus()
    {
        yStudyHours = Mathf.Clamp(yStudyHours - yesterdayStepHours, 0f, maxHoursPerDay);
        UpdateYesterdayTexts();
    }

    public void YRestPlus()
    {
        yRestHours = Mathf.Clamp(yRestHours + yesterdayStepHours, 0f, maxHoursPerDay);
        UpdateYesterdayTexts();
    }

    public void YRestMinus()
    {
        yRestHours = Mathf.Clamp(yRestHours - yesterdayStepHours, 0f, maxHoursPerDay);
        UpdateYesterdayTexts();
    }

    void UpdateYesterdayTexts()
    {
        if (yStudyText)
            yStudyText.text = $"Yesterday Study: {yStudyHours:0} h";

        if (yRestText)
            yRestText.text = $"Yesterday Rest: {yRestHours:0} h";
    }

    /// <summary>
    /// Apply yesterday's routine:
    /// - Scale if exceeding 24h
    /// - Convert hours → internal units
    /// - Update garden & UI
    /// </summary>
    public void ApplyYesterdayRoutine()
    {
        float total = yStudyHours + yRestHours;

        // Ensure yesterday's total time does not exceed 24 hours
        if (total > maxHoursPerDay)
        {
            float scale = maxHoursPerDay / Mathf.Max(total, 0.0001f);
            yStudyHours *= scale;
            yRestHours *= scale;
            UpdateYesterdayTexts();
        }

        // Convert hours to internal units
        studyTime = Mathf.Clamp(HoursToUnits(yStudyHours), 0f, maxUnits);
        restTime = Mathf.Clamp(HoursToUnits(yRestHours), 0f, maxUnits);

        NotifyUserInput();
        UpdateUI();

        if (yesterdayFeedbackText)
        {
            yesterdayFeedbackText.text =
                "The garden now reflects yesterday's study–rest balance.";

            CancelInvoke(nameof(ClearYesterdayFeedback));
            Invoke(nameof(ClearYesterdayFeedback), 3f);
        }
    }

    void ClearYesterdayFeedback()
    {
        if (yesterdayFeedbackText)
            yesterdayFeedbackText.text = "";
    }
}
