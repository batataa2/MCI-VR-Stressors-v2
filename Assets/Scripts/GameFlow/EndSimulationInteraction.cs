using UnityEngine;

public class EndSimulationInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelEndSimulation;

    private void Start()
    {
        if (panelEndSimulation != null)
            panelEndSimulation.SetActive(false);
    }

    // Wird vom Interact-Button / Interact-System aufgerufen
    public void Interact()
    {
        if (panelEndSimulation != null)
            panelEndSimulation.SetActive(true);
    }

    // NO-Button
    public void BackToSimulation()
    {
        if (panelEndSimulation != null)
            panelEndSimulation.SetActive(false);
    }

    // YES-Button
    public void EndSimulation()
    {
        Debug.Log("<color=red>SIMULATION ENDED BY USER</color>");

        // Evaluation finalisieren
        SimulationEvaluationManager.Instance.EndSimulation();

        // Simulation einfrieren
        var sim = FindAnyObjectByType<SimulationManager>();
        if (sim != null)
            sim.enabled = false;

        // UI schließen
        if (panelEndSimulation != null)
            panelEndSimulation.SetActive(false);

        // Optional später:
        // SceneManager.LoadScene("MainMenu");
    }
}
