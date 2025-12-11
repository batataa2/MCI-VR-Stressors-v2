using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Header("NPCs die während Stressoren sichtbar werden sollen")]
    public GameObject[] stressorNPCs;   // z.B. weinende Menschen

    [Header("NPCs (werden nacheinander aktiviert)")]
    public GameObject[] patientNPCs;    // Patienten, mit denen Triage durchgeführt wird

    [Header("Audio")]
    public AudioSource ambientNoise;

    [Header("Stressor 1 – Timer")]
    public TimerStressor timer;

    [Header("Stressor 2 – Crying NPCs")]
    public CryingStressor[] cryingStressors;

    [Header("Stressor 2 – Smoke")]
    public SmokeStressor smokeStressor;
    public FogStressor fogStressor;

    private bool simulationRunning = false;
    private int currentPatientIndex = 0;

    // Events abonnieren
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
        // Alle Patienten unsichtbar machen
        if (patientNPCs != null)
        {
            foreach (var npc in patientNPCs)
                npc.SetActive(false);
        }

        // NPCs für Stressoren ebenfalls unsichtbar machen
        if (stressorNPCs != null)
        {
            foreach (var npc in stressorNPCs)
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

        // ERSTEN Patienten aktivieren
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

    // Wird aufgerufen, wenn ein NPC triagiert wurde
    private void HandleTriageCompleted(NPCInteraction npc)
    {
        Debug.Log("DEBUG: Triage abgeschlossen von: " + npc.name);

        // Nächsten Patienten aktivieren
        ActivateNextPatient();

        // ------------------------------------------------------
        // WICHTIG: Rauch und Fog erst nach Patient 2 starten
        // ------------------------------------------------------
        if (currentPatientIndex == 2) // Patient 1=Index0, Patient 2=Index1 → nächster ist 2
        {
            Debug.Log("<color=orange>Stressor 2 wird aktiviert (Rauch + Fog)</color>");

            // RAUCH starten
            if (smokeStressor != null)
                smokeStressor.StartSmoke();

            // FOG starten
            if (fogStressor != null)
                fogStressor.StartFogIncrease();
        }

        // ------------------------------------------------------
        // Weinende NPCs nur nach Patient 1 ausfaden
        // ------------------------------------------------------
        if (currentPatientIndex == 1)
        {
            Debug.Log("<color=yellow>Crying Stressor wird ausgeblendet</color>");

            foreach (var stressor in cryingStressors)
            {
                if (stressor != null)
                    stressor.FadeOut();
            }
        }
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
