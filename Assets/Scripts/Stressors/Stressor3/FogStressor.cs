using UnityEngine;

public class FogStressor : MonoBehaviour
{
    public Transform playerHead;      // VR-Kamera / Main Camera
    public float maxDensity = 0.25f;  // maximale Nebeldichte
    public float increaseSpeed = 0.05f;
    public float decreaseSpeed = 0.05f;

    private float currentDensity = 0f;
    private bool fogIncreasing = false;
    private bool fogDecreasing = false;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.gray;
        RenderSettings.fogDensity = 0f;
    }

    void Update()
    {
        // Fog dynamisch anpassen
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
        }

        RenderSettings.fogDensity = currentDensity;

        // Fog folgt dem Spieler (Option)
        if (playerHead != null)
            transform.position = playerHead.position;
    }

    // WIRD vom SimulationManager aufgerufen
    public void StartFogIncrease()
    {
        fogIncreasing = true;
        fogDecreasing = false;
    }

    // WIRD NACH Patient 3 aufgerufen
    public void StopFog()
    {
        fogIncreasing = false;
        fogDecreasing = true;
    }
}
