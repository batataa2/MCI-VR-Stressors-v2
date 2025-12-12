using UnityEngine;
using System;

public class NPCInteraction : MonoBehaviour
{
    [Header("Triage (pro Patient im Inspector setzen)")]
    [SerializeField] private string recommendedTriage = "UNKNOWN";

    [Header("References")]
    public NPCCondition condition;
    public AudioSource audioSource;

    [Header("UI Panels")]
    public GameObject panelInteract;
    public GameObject panelMainMenu;
    public GameObject panelVitalInfo;
    public GameObject panelTriage;
    public GameObject panelConfirmFinish;

    [Header("Vital Text Fields")]
    public TMPro.TextMeshProUGUI BP;
    public TMPro.TextMeshProUGUI Pulse;
    public TMPro.TextMeshProUGUI Tmp;
    public TMPro.TextMeshProUGUI Breathing;
    public TMPro.TextMeshProUGUI Spo2;
    public TMPro.TextMeshProUGUI Hurt_Status;

    private string pendingTriageColor = null;

    private bool dialogPlayedOnce = false;
    private bool finished = false;

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
        if (condition == null || condition.speechClip == null) return;
        if (audioSource == null) return;

        audioSource.Stop();
        audioSource.clip = condition.speechClip;
        audioSource.Play();
    }

    // ----------------------------
    // PANEL STEUERUNG
    // ----------------------------
    public void ShowMainMenu()
    {
        if (panelMainMenu) panelMainMenu.SetActive(true);
        if (panelVitalInfo) panelVitalInfo.SetActive(false);
        if (panelTriage) panelTriage.SetActive(false);
        if (panelConfirmFinish) panelConfirmFinish.SetActive(false);
    }

    public void ShowVitalInfo()
    {
        if (condition == null) return;

        if (BP) BP.text = $"Blood Pressure: {condition.bloodPressure} mmHg";
        if (Pulse) Pulse.text = $"Pulse: {condition.pulse} bpm";
        if (Tmp) Tmp.text = $"Temperature: {condition.temperature} °C";
        if (Spo2) Spo2.text = $"SpO2: {condition.oxygenSaturation} %"; // <- kein Sonderzeichen, sonst Font-Warn
        if (Hurt_Status) Hurt_Status.text = $"Status: {condition.injuryDescription}";
        if (Breathing) Breathing.text = $"Breathing: {(condition.isBreathing ? "Yes" : "No")}";

        if (panelMainMenu) panelMainMenu.SetActive(false);
        if (panelVitalInfo) panelVitalInfo.SetActive(true);
    }

    public void ShowTriage()
    {
        if (panelMainMenu) panelMainMenu.SetActive(false);
        if (panelTriage) panelTriage.SetActive(true);
    }

    public void ShowFinishConfirm()
    {
        if (panelMainMenu) panelMainMenu.SetActive(false);
        if (panelConfirmFinish) panelConfirmFinish.SetActive(true);
    }

    public void BackToMainMenu() => ShowMainMenu();

    // ----------------------------
    // TRIAGE
    // ----------------------------
    public void SelectTriage(string color)
    {
        pendingTriageColor = color;
        Debug.Log($"[NPCInteraction] {name} selected triage: {color}");
    }

    // ----------------------------
    // ABSCHLUSS: auch ohne Triage -> weiter
    // ----------------------------
    public void ConfirmFinish_Yes()
    {
        if (finished) return;

        bool triageSelected = !string.IsNullOrWhiteSpace(pendingTriageColor);
        string userChoice = triageSelected ? pendingTriageColor : "NONE";

        if (triageSelected && condition != null)
            condition.SetTriageResult(pendingTriageColor);

        float decisionTime = (firstInteractTime >= 0f) ? (Time.time - firstInteractTime) : 0f;

        // Evaluation (optional, darf NICHT crashen wenn Instance fehlt)
        if (SimulationEvaluationManager.Instance != null)
        {
            SimulationEvaluationManager.Instance.RegisterTriage(
            patientId: gameObject.name,
            recommended: recommendedTriage,
            userChoice: userChoice,
            decisionTime: decisionTime,
            triageCompleted: triageSelected
            );

        }
        else
        {
            Debug.LogWarning("[NPCInteraction] SimulationEvaluationManager.Instance is NULL -> triage not logged");
        }

        finished = true;
        HideAllPanels();

        Debug.Log($"[NPCInteraction] FINISHED: {name} | triageSelected={triageSelected} | userChoice={userChoice}");
        OnTriageCompleted?.Invoke(this);
    }

    public void ConfirmFinish_No()
    {
        ShowMainMenu();
    }

    private void HideAllPanels()
    {
        panelInteract?.SetActive(false);
        panelMainMenu?.SetActive(false);
        panelVitalInfo?.SetActive(false);
        panelTriage?.SetActive(false);
        panelConfirmFinish?.SetActive(false);
    }
}
