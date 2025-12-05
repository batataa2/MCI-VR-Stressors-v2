using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public GameObject Canvas_Dialogue;      // Dialogue box
    public GameObject Canvas_Quest;         // ❗ First interaction indicator
    public GameObject Canvas_Quest_After;   // ❓ After speaking indicator

    public AudioSource voiceSource;         // The NPC's AudioSource
    public AudioClip dialogueClip;          // The recorded voice line

    private bool hasTalked = false;         // Track if player has spoken before
    private bool dialogueOpen = false;      // Track if dialogue is currently open
    

    private void OnMouseDown()
    {
        Interact();
    }

    public void Interact()
    {
        // Toggle dialogue visibility
        dialogueOpen = !dialogueOpen;

        if (Canvas_Dialogue != null)
            Canvas_Dialogue.SetActive(dialogueOpen);

        // Handle ❗ icon (hide permanently after first talk)
        if (!hasTalked)
        {
            hasTalked = true;

            if (Canvas_Quest != null)
                Canvas_Quest.SetActive(false);
        }

        // Handle ❓ icon (show only when dialogue is closed)
        if (Canvas_Quest_After != null)
        {
            Canvas_Quest_After.SetActive(!dialogueOpen && hasTalked);
        }

        // Handle audio playback
        if (dialogueOpen)
        {
            // Dialogue OPEN → play voice
            if (voiceSource != null && dialogueClip != null)
            {
                voiceSource.clip = dialogueClip;
                voiceSource.Play();
            }
        }
        else
        {
            // Dialogue CLOSED → stop voice
            if (voiceSource != null)
                voiceSource.Stop();
        }
    }
}
