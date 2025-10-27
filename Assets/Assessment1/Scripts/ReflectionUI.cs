using UnityEngine;
using TMPro;

public class ReflectionUI : MonoBehaviour
{
    public TimeBalance timeBalance;       
    public GameObject panel;              
    public TextMeshProUGUI text;          
    public float step = 30f;

    public float idlePopupSeconds = 15f;

    float idleTime = 0f;

    void Start()
    {
        if (panel) panel.SetActive(false);
        if (timeBalance) timeBalance.OnAnyUserInput += ResetIdle;
        ResetIdle();
    }

    void Update()
    {
        idleTime += Time.deltaTime;
        if (idleTime > idlePopupSeconds) ShowReflection();
    }

    public void StudyPlus() { if (timeBalance) { timeBalance.AddStudy(+step); ResetIdle(); } }
    public void StudyMinus() { if (timeBalance) { timeBalance.AddStudy(-step); ResetIdle(); } }
    public void RestPlus() { if (timeBalance) { timeBalance.AddRest(+step); ResetIdle(); } }
    public void RestMinus() { if (timeBalance) { timeBalance.AddRest(-step); ResetIdle(); } }
    public void BalancedPreset()
    {
        if (timeBalance) { timeBalance.SetBalanced(); ResetIdle(); }
    }

    public void ShowReflection()
    {
        if (!panel || !timeBalance || !text) return;

        panel.SetActive(true);
        float b = timeBalance.balanceIndex;

        if (b < 0.4f)
            text.text = "Your garden is overworked ！ add rest blocks.";
        else if (b < 0.7f)
            text.text = "Closer to balance ！ try short recovery breaks.";
        else if (b < 0.9f)
            text.text = "Looking healthy ！ keep your rhythm.";
        else
            text.text = "Beautiful harmony ！ maintain what works today.";

        ResetIdle();
    }

    public void Hide()
    {
        if (panel) panel.SetActive(false);
        ResetIdle();
    }

    void ResetIdle() => idleTime = 0f;
}
