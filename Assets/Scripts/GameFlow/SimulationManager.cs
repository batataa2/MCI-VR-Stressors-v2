using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [Header("Patienten (Triagierbar, Reihenfolge!)")]
    public GameObject[] patientNPCs;

    [Header("Audio")]
    public AudioSource ambientNoise;

    [Header("Stressor 1 Globaler Timer")]
    public TimerStressor mainTimer;

    [Header("Stressor 2 Crying")]
    public CryingStressor[] cryingStressors;

    [Header("Stressor 3 Smoke + Fog")]
    public SmokeStressor smokeStressor;
    public FogStressor fogStressor;

    [Header("Stressor 4 Fire")]
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
        if (patientNPCs != null)
            foreach (var npc in patientNPCs)
                if (npc != null) npc.SetActive(false);
    }

    public void BeginSimulation()
    {
        if (simulationRunning) return;
        simulationRunning = true;

        Debug.Log("<color=green>SIMULATION STARTED</color>");

        mainTimer?.StartTimer();

        if (ambientNoise != null)
            ambientNoise.Play(); // <- sonst bekommst du die “ambientNoise not assigned” Meldung

        if (cryingStressors != null)
            foreach (var c in cryingStressors)
                if (c != null) c.StartStressor();

        ActivatePatient(0); // Patient 1
    }

    private void HandleTriageCompleted(NPCInteraction npc)
    {
        if (!simulationRunning) return;
        if (npc == null) return;

        // Best effort: finde Index des NPC, aber blockiere NICHT wenn’s komisch ist
        int completedIndex = IndexOfPatient(npc.gameObject);
        Debug.Log($"[SIM] Triage completed by: {npc.name} | idx={completedIndex} | currentIdx={currentPatientIndex}");

        // Falls wir ihn finden, syncen wir currentPatientIndex darauf (damit die Trigger stimmen)
        if (completedIndex >= 0)
            currentPatientIndex = completedIndex;

        // -----------------------------
        // Trigger nach Patient 2 (Index 1)
        // -----------------------------
        if (currentPatientIndex == 1)
        {
            if (cryingStressors != null)
                foreach (var c in cryingStressors)
                    if (c != null) c.FadeOut();

            smokeStressor?.StartSmoke();
            fogStressor?.StartFogIncrease();

            HUDMessageController.Instance?.ShowPersistentMessage(
                "RAUCH BREITET SICH AUS – SICHT EINGESCHRÄNKT!"
            );
        }

        // -----------------------------
        // Trigger nach Patient 4 (Index 3)
        // -----------------------------
        if (currentPatientIndex == 3)
        {
            if (fireStressors != null)
                foreach (var fire in fireStressors)
                    if (fire != null) fire.BeginFire();

            HUDMessageController.Instance?.ShowPersistentMessage(
                "FEUER BRICHT AUS! TRIAGIERE SCHNELL!"
            );
        }

        // -----------------------------
        // Trigger nach Patient 7 (Index 6)
        // -----------------------------
        if (currentPatientIndex == 6)
        {
            if (fireStressors != null)
                foreach (var fire in fireStressors)
                    if (fire != null) fire.ExtinguishFire();

            fogStressor?.StopFog();
            smokeStressor?.StopSmoke();

            HUDMessageController.Instance?.ShowPersistentMessage(
                "FEUERWEHR VOR ORT – LAGE UNTER KONTROLLE"
            );

            Invoke(nameof(ClearHUDMessage), 4f);

            Debug.Log("<color=yellow>Simulation beendet</color>");
            return;
        }

        // Nächster Patient
        ActivatePatient(currentPatientIndex + 1);
    }

    private void ActivatePatient(int index)
    {
        if (patientNPCs == null || patientNPCs.Length == 0) return;

        if (index < 0 || index >= patientNPCs.Length)
        {
            Debug.LogWarning($"[SIM] ActivatePatient out of range: {index}");
            return;
        }

        currentPatientIndex = index;

        var go = patientNPCs[index];
        if (go == null)
        {
            Debug.LogError($"[SIM] patientNPCs[{index}] is NULL!");
            return;
        }

        // WICHTIG: Wenn ein Parent-Objekt deaktiviert ist, bringt SetActive(true) am Kind nix (activeInHierarchy bleibt false)
        if (go.transform.parent != null && !go.transform.parent.gameObject.activeInHierarchy)
            Debug.LogWarning($"[SIM] Parent of {go.name} is inactive -> NPC won’t be visible until parent is active.");

        go.SetActive(true);

        Debug.Log($"[SIM] Activated patient {index + 1}: {go.name} | activeSelf={go.activeSelf} | activeInHierarchy={go.activeInHierarchy}");
    }

    private int IndexOfPatient(GameObject go)
    {
        if (go == null || patientNPCs == null) return -1;

        // root ist sicherer, falls NPCInteraction auf Child hängt
        var root = go.transform.root.gameObject;

        for (int i = 0; i < patientNPCs.Length; i++)
        {
            if (patientNPCs[i] == null) continue;

            if (patientNPCs[i] == go) return i;
            if (patientNPCs[i] == root) return i;

            if (go.transform.IsChildOf(patientNPCs[i].transform)) return i;
            if (patientNPCs[i].transform.IsChildOf(go.transform)) return i;
        }

        return -1;
    }

    private void ClearHUDMessage()
    {
        HUDMessageController.Instance?.ClearMessage();
    }
}
