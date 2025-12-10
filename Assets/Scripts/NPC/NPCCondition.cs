using UnityEngine;

public class NPCCondition : MonoBehaviour
{
    [Header("Patient Basic Info")]
    public string patientName;
    public string injuryDescription;

    [Header("Vital Signs")]
    public string bloodPressure;         // z.B. "120/80"
    public string pulse;                 // z.B. "85 bpm"
    public string temperature;           // z.B. "37.1°C"
    public string oxygenSaturation;      // z.B. "96%"
    public bool isBreathing;             // true/false
    public bool isConscious;             // true/false

    [Header("Recommended Triage")]
    public string recommendedTriage;     // expected triage result

    [Header("Player Result (Runtime)")]
    public string playerTriage = null;
    public bool triageCompleted = false;

    [Header("Audio")]
    public AudioClip speechClip;

    // Speichert das Ergebnis der Triage
    public void SetTriageResult(string color)
    {
        playerTriage = color;
        triageCompleted = true;
    }
}
