using UnityEngine;
using System;

public class NPCInteraction : MonoBehaviour
{
    [Header("References")]
    public NPCCondition condition;
    public AudioSource audioSource;

    [Header("UI Panels")]
    public GameObject panelInteract;
    public GameObject panelMainMenu;
    public GameObject panelVitalInfo;
    public GameObject panelTriage;
    public GameObject panelConfirmFinish;

    [Header("Vital Texts")]
    public TMPro.TextMeshProUGUI BP;
    public TMPro.TextMeshProUGUI Pulse;
    public TMPro.TextMeshProUGUI Tmp;
    public TMPro.TextMeshProUGUI Breathing;
    public TMPro.TextMeshProUGUI Spo2;
    public TMPro.TextMeshProUGUI Hurt_Status;

    private string pendingTriageColor = null;
    private bool finished = false;
    private bool dialogPlayedOnce = false;
    private float firstInteractTime = -1f;

    public static event Action<NPCInteraction> OnTriageCompleted;

    private void Start()
    {
        panelInteract?.SetActive(true);
        panelMainMenu?.SetActive(false);
        panelVitalInfo?.SetActive(false);
        panelTriage?.SetActive(false);
        panelConfirmFinish?.SetActive(false);
    }

    public void Interact()
    {
        if (finished) return;

        if (firstInteractTime < 0f)
            firstInteractTime = Time.time;

        panelInteract?.SetActive(false);
        ShowMainMenu();

        if (!dialogPlayedOnce)
        {
            PlayNPCDialog();
            dialogPlayedOnce = true;
        }
    }

    public void PlayNPCDialog()
    {
        if (condition?.speechClip == null || audioSource == null) return;
        audioSource.Stop();
        audioSource.clip = condition.speechClip;
        audioSource.Play();
    }

    public void ShowMainMenu()
    {
        panelMainMenu?.SetActive(true);
        panelVitalInfo?.SetActive(false);
        panelTriage?.SetActive(false);
        panelConfirmFinish?.SetActive(false);
    }

    public void ShowVitalInfo()
    {
        BP.text = $"Blood Pressure: {condition.bloodPressure}";
        Pulse.text = $"Pulse: {condition.pulse}";
        Tmp.text = $"Temperature: {condition.temperature}";
        Spo2.text = $"SpO2: {condition.oxygenSaturation}";
        Hurt_Status.text = condition.injuryDescription;
        Breathing.text = condition.isBreathing ? "Breathing: Yes" : "Breathing: No";

        panelMainMenu?.SetActive(false);
        panelVitalInfo?.SetActive(true);
    }

    public void ShowTriage()
    {
        panelMainMenu?.SetActive(false);
        panelTriage?.SetActive(true);
    }

    public void ShowFinishConfirm()
    {
        panelMainMenu?.SetActive(false);
        panelConfirmFinish?.SetActive(true);
    }

    public void SelectTriage(string color)
    {
        pendingTriageColor = color;
    }

    public void ConfirmFinish_Yes()
    {
        if (finished) return;

        bool triageSelected = !string.IsNullOrEmpty(pendingTriageColor);
        string userChoice = triageSelected ? pendingTriageColor : "NONE";

        if (triageSelected)
            condition.SetTriageResult(pendingTriageColor);

        float decisionTime = Time.time - firstInteractTime;

        SimulationEvaluationManager.Instance.RegisterTriage(
            gameObject.name,
            condition.recommendedTriage,
            userChoice,
            decisionTime,
            triageSelected
        );

        finished = true;
        HideAllPanels();
        OnTriageCompleted?.Invoke(this);
    }

    public void ConfirmFinish_No() => ShowMainMenu();

    private void HideAllPanels()
    {
        panelInteract?.SetActive(false);
        panelMainMenu?.SetActive(false);
        panelVitalInfo?.SetActive(false);
        panelTriage?.SetActive(false);
        panelConfirmFinish?.SetActive(false);
    }
    public void BackToMainMenu()
{
    if (finished)
        return;

    // alle Panels sicher aus
    if (panelVitalInfo) panelVitalInfo.SetActive(false);
    if (panelTriage) panelTriage.SetActive(false);
    if (panelConfirmFinish) panelConfirmFinish.SetActive(false);

    // Main Menu an
    if (panelMainMenu)
        panelMainMenu.SetActive(true);

    Debug.Log($"[NPCInteraction] BackToMainMenu called on {name}");
}

}
