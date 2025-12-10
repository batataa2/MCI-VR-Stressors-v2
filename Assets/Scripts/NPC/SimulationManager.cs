using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Header("NPCs (werden nacheinander aktiviert)")]
    public GameObject[] patientNPCs;

    [Header("Audio")]
    public AudioSource ambientNoise;

    [Header("Stressor 1 – Timer")]
    public TimerStressor timer;     // <- WICHTIG!

    [Header("Stressor-System (optional, später)")]
    public GameObject[] stressors;   // Feuer, Rauch, Schreie – aktuell noch inaktiv

    private bool simulationRunning = false;

    void Start()
    {
        // Patienten deaktivieren
        if (patientNPCs != null)
        {
            foreach (var npc in patientNPCs)
                npc.SetActive(false);
        }

        // Stressoren deaktivieren
        if (stressors != null)
        {
            foreach (var s in stressors)
                s.SetActive(false);
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

        // TIMER STARTEN
        if (timer != null)
        {
            Debug.Log("DEBUG: StartTimer() wird aufgerufen");
            timer.StartTimer();
        }
        else
        {
            Debug.LogError("DEBUG: Timer ist NICHT verknüpft!");
        }

        // GRUNDGERÄUSCHE STARTEN
        if (ambientNoise != null)
            ambientNoise.Play();

        // ERSTEN PATIENTEN AKTIVIEREN
        if (patientNPCs != null && patientNPCs.Length > 0)
        {
            Debug.Log("DEBUG: Patient 1 wurde aktiviert");
            patientNPCs[0].SetActive(true);
        }
        else
        {
            Debug.LogWarning("SimulationManager: Keine Patienten zugewiesen!");
        }

        // Hier später: Stressoren sequenziell starten
        // StartCoroutine(StartStressorsDelayed());
    }
}
