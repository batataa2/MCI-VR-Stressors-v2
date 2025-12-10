using UnityEngine;
using System;

public class NPCInteraction : MonoBehaviour
{
    [Header("References")]
    public NPCCondition condition;
    public AudioSource audioSource;   // AUDIO für Dialog

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
    private bool isFinished = false;
    private bool dialogPlayedOnce = false;  // NEU: Nur beim ersten Interact autoplay

    public static event Action<NPCInteraction> OnTriageCompleted;

    private void Start()
    {
        if (panelInteract != null) panelInteract.SetActive(true);
        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelVitalInfo != null) panelVitalInfo.SetActive(false);
        if (panelTriage != null) panelTriage.SetActive(false);
        if (panelConfirmFinish != null) panelConfirmFinish.SetActive(false);
    }

    // ----------------------------
    //  Interact Logic
    // ----------------------------
    public void Interact()
    {
        if (isFinished)
        {
            Debug.Log("NPCInteraction: Interact called, but NPC is finished.");
            return;
        }

        Debug.Log("NPCInteraction: Interact called - opening main menu.");

        if (panelInteract != null)
            panelInteract.SetActive(false);

        ShowMainMenu();

        // NEU — Dialog nur beim ersten Interact automatisch abspielen
        if (!dialogPlayedOnce)
        {
            PlayNPCDialog();
            dialogPlayedOnce = true;
        }
    }

    // ----------------------------
    // DIALOG AB SPIELEN
    // ----------------------------
    public void PlayNPCDialog()
    {
        Debug.Log("NPCInteraction: PlayNPCDialog called");

        if (condition.speechClip == null)
        {
            Debug.LogWarning("Dieser NPC hat keinen Dialogclip.");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource fehlt im NPCInteraction!");
            return;
        }

        audioSource.Stop();
        audioSource.clip = condition.speechClip;
        audioSource.Play();
    }

    // ----------------------------
    // PANEL STEUERUNG
    // ----------------------------
    public void ShowMainMenu()
    {
        Debug.Log("NPCInteraction: ShowMainMenu");

        panelMainMenu.SetActive(true);
        panelVitalInfo.SetActive(false);
        panelTriage.SetActive(false);
        panelConfirmFinish.SetActive(false);
    }

    public void ShowVitalInfo()
    {
        Debug.Log("NPCInteraction: ShowVitalInfo");

        BP.text          = $"Blood Pressure: {condition.bloodPressure} mmHg";
        Pulse.text       = $"Pulse: {condition.pulse} bpm";
        Tmp.text         = $"Temperature: {condition.temperature} °C";
        Spo2.text        = $"SpO₂: {condition.oxygenSaturation} %";
        Hurt_Status.text = $"Status: {condition.injuryDescription}";
        Breathing.text   = $"Breathing: {(condition.isBreathing ? "Yes" : "No")}";

        panelMainMenu.SetActive(false);
        panelVitalInfo.SetActive(true);
    }

    public void ShowTriage()
    {
        Debug.Log("NPCInteraction: ShowTriage");

        panelMainMenu.SetActive(false);
        panelTriage.SetActive(true);
    }

    public void ShowFinishConfirm()
    {
        Debug.Log("NPCInteraction: ShowFinishConfirm");

        panelMainMenu.SetActive(false);
        panelConfirmFinish.SetActive(true);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    // ----------------------------
    // TRIAGE AUSWAHL
    // ----------------------------
    public void SelectTriage(string color)
    {
        pendingTriageColor = color;
        Debug.Log($"NPCInteraction: Selected Triage Color: {color}");
    }

    // ----------------------------
    // TRIAGE ABSCHLUSS
    // ----------------------------
    public void ConfirmFinish_Yes()
    {
        Debug.Log("NPCInteraction: ConfirmFinish_Yes");

        if (pendingTriageColor != null)
        {
            condition.SetTriageResult(pendingTriageColor);
        }
        else
        {
            Debug.LogWarning("NPCInteraction: Finished without selecting a triage color.");
        }

        isFinished = true;
        HideAllPanels();
        OnTriageCompleted?.Invoke(this);
    }

    public void ConfirmFinish_No()
    {
        Debug.Log("NPCInteraction: ConfirmFinish_No");
        ShowMainMenu();
    }

    private void HideAllPanels()
    {
        panelInteract.SetActive(false);
        panelMainMenu.SetActive(false);
        panelVitalInfo.SetActive(false);
        panelTriage.SetActive(false);
        panelConfirmFinish.SetActive(false);
    }
}
