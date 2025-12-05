using UnityEngine;

public class NPCCondition : MonoBehaviour
{
    public string patientName;
    public string injuryDescription;
    public string vitalSigns;
    public bool isBreathing;
    public bool isConscious;

    public string recommendedTriage; // "red", "yellow", "green", "black"
    public AudioClip speechClip;
}
