using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Header("NPCs (werden nacheinander aktiviert)")]
    public GameObject[] patientNPCs;

    [Header("Audio")]
    public AudioSource ambientNoise;

    [Header("Stressor 1 – Timer")]
    public TimerStressor timer;

    [Header("Stressor 2 – Crying NPCs")]
    public CryingStressor[] cryingStressors;   // mehrere weinende NPCs möglich

    private bool simulationRunning = false;
    private int currentPatientIndex = 0;

    private void OnEnable()
    {
        NPCInteraction.OnTriageCompleted += HandleTriageCompleted;
    }

    private void OnDisable()
    {
        NPCInteraction.OnTriageCompleted -= HandleTriageCompleted;
    }

    void Start()
    {
        // Alle Patienten ausschalten
        if (patientNPCs != null)
        {
            foreach (var npc in patientNPCs)
                npc.SetActive(false);
        }
    }

    // Wird vom PoliceIntroDialogue gestartet
    public void BeginSimulation()
    {
        Debug.Log("DEBUG: BeginSimulation wurde aufgerufen");

        if (simulationRunning)
            return;

        simulationRunning = true;

        Debug.Log("<color=green>SIMULATION STARTED</color>");

        // TIMER starten
        if (timer != null)
        {
            Debug.Log("DEBUG: StartTimer() wird aufgerufen");
            timer.StartTimer();
        }
        else
        {
            Debug.LogError("DEBUG: Timer ist NICHT verknüpft!");
        }

        // Ambient Noise starten
        if (ambientNoise != null)
            ambientNoise.Play();

        // ERSTEN Patienten sichtbar machen
        if (patientNPCs != null && patientNPCs.Length > 0)
        {
            currentPatientIndex = 0;
            patientNPCs[0].SetActive(true);
            Debug.Log("DEBUG: Patient 1 wurde aktiviert");
        }
        else
        {
            Debug.LogWarning("SimulationManager: Keine Patienten zugewiesen!");
        }

        // Weinende NPCs starten
        foreach (var stressor in cryingStressors)
        {
            if (stressor != null)
                stressor.StartStressor();
        }
    }

    // Wird aufgerufen wenn ein NPC triagiert wurde
    private void HandleTriageCompleted(NPCInteraction npc)
    {
        Debug.Log("DEBUG: Triage abgeschlossen von: " + npc.name);

        // WEINEN langsam ausfaden
        foreach (var stressor in cryingStressors)
        {
            if (stressor != null)
                stressor.FadeOut();
        }

        // NÄCHSTEN Patienten aktivieren
        ActivateNextPatient();
    }

    private void ActivateNextPatient()
    {
        currentPatientIndex++;

        if (currentPatientIndex >= patientNPCs.Length)
        {
            Debug.Log("<color=yellow>Alle Patienten abgearbeitet!</color>");
            return;
        }

        Debug.Log($"DEBUG: Patient {currentPatientIndex + 1} wird aktiviert");
        patientNPCs[currentPatientIndex].SetActive(true);
    }
}
