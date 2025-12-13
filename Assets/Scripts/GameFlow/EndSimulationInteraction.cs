using UnityEngine;

public class EndSimulationInteraction : MonoBehaviour
{
    public GameObject panelInteract;
    public GameObject panelEndSimulation;

    public ScoreboardController scoreboardController;
    public TimerStressor mainTimer;

    private bool simulationEnded = false;

    private void Start()
    {
        panelEndSimulation?.SetActive(false);

        if (scoreboardController != null)
            scoreboardController.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (simulationEnded) return;

        panelInteract?.SetActive(false);
        panelEndSimulation?.SetActive(true);
    }

    public void BackToSimulation()
    {
        panelEndSimulation?.SetActive(false);
        panelInteract?.SetActive(true);
    }

    public void EndSimulation()
    {
        if (simulationEnded) return;
        simulationEnded = true;

        Debug.Log("<color=red>SIMULATION ENDED BY USER</color>");

        // -----------------------------
        // TIMER STOPPEN & ZEIT HOLEN
        // -----------------------------
        if (mainTimer == null)
        {
            Debug.LogError("[EndSimulation] mainTimer ist NICHT zugewiesen!");
            return;
        }

        mainTimer.StopTimer();
        float duration = mainTimer.GetElapsedTime();

        // -----------------------------
        // EVALUATION FINALISIEREN
        // -----------------------------
        if (SimulationEvaluationManager.Instance == null)
        {
            Debug.LogError("[EndSimulation] SimulationEvaluationManager fehlt!");
            return;
        }

        SimulationEvaluationManager.Instance.EndSimulation(duration);

        // -----------------------------
        // SIMULATION DEAKTIVIEREN
        // -----------------------------
        var sim = FindAnyObjectByType<SimulationManager>();
        if (sim != null)
        {
            sim.LockAllNPCInteractions();
            sim.enabled = false;
        }

        // -----------------------------
        // UI WECHSEL
        // -----------------------------
        panelEndSimulation?.SetActive(false);
        panelInteract?.SetActive(false);

        scoreboardController.ShowScoreboard();
    }
}
