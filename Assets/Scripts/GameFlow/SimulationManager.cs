using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Header("Patienten (Reihenfolge!)")]
    public GameObject[] patientNPCs;

    [Header("End Simulation NPC")]
    public GameObject endSimulationNPC;

    [Header("Audio")]
    public AudioSource ambientNoise;

    [Header("Stressor 1 – Timer")]
    public TimerStressor mainTimer;

    [Header("Stressor 2 – Crying")]
    public CryingStressor[] cryingStressors;

    [Header("Stressor 3 – Smoke + Fog")]
    public SmokeStressor smokeStressor;
    public FogStressor fogStressor;

    [Header("Stressor 4 – Fire")]
    public FireEvolutionController[] fireStressors;

    private int currentPatientIndex = -1;
    private bool simulationRunning = false;

    private void OnEnable()
    {
        NPCInteraction.OnTriageCompleted += HandleTriageCompleted;
    }

    private void OnDisable()
    {
        NPCInteraction.OnTriageCompleted -= HandleTriageCompleted;
    }

    private void Start()
    {
        foreach (var npc in patientNPCs)
            if (npc != null) npc.SetActive(false);

        if (endSimulationNPC != null)
            endSimulationNPC.SetActive(false);
    }

    public void BeginSimulation()
    {
        if (simulationRunning) return;
        simulationRunning = true;

        SimulationEvaluationManager.Instance.StartSimulation();

        mainTimer?.StartTimer();
        ambientNoise?.Play();

        foreach (var c in cryingStressors)
            c?.StartStressor();

        ActivatePatient(0);

        if (endSimulationNPC != null)
            endSimulationNPC.SetActive(true);
    }

    private void HandleTriageCompleted(NPCInteraction npc)
    {
        if (!simulationRunning || npc == null) return;

        int idx = IndexOfPatient(npc.gameObject);
        if (idx >= 0)
            currentPatientIndex = idx;

        // -------------------------
        // NACH PATIENT 2 → RAUCH
        // -------------------------
        if (currentPatientIndex == 1)
        {
            foreach (var c in cryingStressors)
                c?.FadeOut();

            smokeStressor?.StartSmoke();
            fogStressor?.StartFogIncrease();

            HUDMessageController.Instance?.ShowPersistentMessage(
                "⚠️ RAUCH BREITET SICH AUS – SICHT EINGESCHRÄNKT!"
            );
        }

        // -------------------------
        // NACH PATIENT 4 → FEUER
        // -------------------------
        if (currentPatientIndex == 3)
        {
            foreach (var f in fireStressors)
                f?.BeginFire();

            HUDMessageController.Instance?.ShowPersistentMessage(
                "🔥 FEUER BRICHT AUS! TRIAGIERE SCHNELL!"
            );
        }

        // -------------------------
        // NACH PATIENT 7 → ENDE
        // -------------------------
        if (currentPatientIndex == 6)
        {
            foreach (var f in fireStressors)
                f?.ExtinguishFire();

            smokeStressor?.StopSmoke();
            fogStressor?.StopFog();

            HUDMessageController.Instance?.ShowPersistentMessage(
                "🚒 FEUERWEHR VOR ORT – LAGE UNTER KONTROLLE"
            );

            Invoke(nameof(ClearHUD), 4f);
            return;
        }

        ActivatePatient(currentPatientIndex + 1);
    }

    private void ActivatePatient(int index)
    {
        if (index < 0 || index >= patientNPCs.Length) return;

        currentPatientIndex = index;
        patientNPCs[index].SetActive(true);
    }

    public void LockAllNPCInteractions()
    {
        foreach (var npc in FindObjectsOfType<NPCInteraction>(true))
            npc.enabled = false;
    }

    private int IndexOfPatient(GameObject go)
    {
        var root = go.transform.root.gameObject;

        for (int i = 0; i < patientNPCs.Length; i++)
        {
            if (patientNPCs[i] == root)
                return i;
        }
        return -1;
    }

    private void ClearHUD()
    {
        HUDMessageController.Instance?.ClearMessage();
    }
}
