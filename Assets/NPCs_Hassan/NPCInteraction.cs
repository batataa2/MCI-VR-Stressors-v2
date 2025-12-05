using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public NPCCondition condition;        // Reference to NPCCondition script
    public GameObject triageCardUI;       // The UI canvas/panel
    public AudioSource audioSource;       // For patient voice lines

    private bool uiOpen = false;

    void OnMouseDown()
    {


        // Toggle UI on/off when clicked
        uiOpen = !uiOpen;

        if (uiOpen)
        {
            // Show triage UI
            triageCardUI.SetActive(true);

            // Update the UI with this NPC's condition
            var ui = triageCardUI.GetComponent<TriageCardUI>();
            ui.Display(condition);

            // Play speech if available
            if (condition.speechClip != null && audioSource != null)
            {
                audioSource.clip = condition.speechClip;
                audioSource.Play();
            }
        }
        else
        {
            // Hide UI
            triageCardUI.SetActive(false);

            // Stop any ongoing speech
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    void Update()
    {
        // Allow closing the UI with Escape
        if (uiOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            uiOpen = false;
            triageCardUI.SetActive(false);

            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}
