using UnityEngine;

public class FogStressor : MonoBehaviour
{
    public Transform playerHead;
    public float maxDensity = 0.25f;
    public float increaseSpeed = 0.05f;
    public float decreaseSpeed = 0.05f;

    private float currentDensity = 0f;
    private bool fogIncreasing = false;
    private bool fogDecreasing = false;
    private bool isActive = false;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogDensity = 0f;
    }

    void Update()
    {
        if (fogIncreasing)
        {
            currentDensity = Mathf.MoveTowards(
                currentDensity,
                maxDensity,
                increaseSpeed * Time.deltaTime
            );
        }
        else if (fogDecreasing)
        {
            currentDensity = Mathf.MoveTowards(
                currentDensity,
                0f,
                decreaseSpeed * Time.deltaTime
            );

            // Wenn vollständig weg → Stressor beenden
            if (Mathf.Approximately(currentDensity, 0f))
            {
                fogDecreasing = false;
                EndStressor();
            }
        }

        RenderSettings.fogDensity = currentDensity;

        // Optional: Fog folgt Spieler (eigentlich unnötig bei RenderSettings)
        if (playerHead != null)
            transform.position = playerHead.position;
    }

    // -----------------------------------------
    // Wird vom SimulationManager aufgerufen
    // -----------------------------------------
    public void StartFogIncrease()
    {
        if (isActive) return;
        isActive = true;

        fogIncreasing = true;
        fogDecreasing = false;

        // Evaluation START
        SimulationEvaluationManager.Instance.StressorStarted("Fog");
    }

    // -----------------------------------------
    // Wird vom SimulationManager aufgerufen
    // -----------------------------------------
    public void StopFog()
    {
        if (!isActive) return;

        fogIncreasing = false;
        fogDecreasing = true;
    }

    // -----------------------------------------
    // Evaluation ENDE
    // -----------------------------------------
    private void EndStressor()
    {
        if (!isActive) return;
        isActive = false;

        SimulationEvaluationManager.Instance.StressorEnded("Fog");
    }
}
