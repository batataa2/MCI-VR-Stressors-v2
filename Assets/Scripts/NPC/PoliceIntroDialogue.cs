using UnityEngine;
using TMPro;

public class PoliceIntroDialogue : MonoBehaviour
{
    [Header("Root Canvas")]
    public GameObject rootCanvas;
    [Header("UI")]
    public GameObject panelDialogue;
    public TextMeshProUGUI dialogueText;
    public GameObject buttonBack;
    public GameObject buttonContinue;
    public GameObject buttonStartSimulation;

    [Header("Other UI")]
    public GameObject buttonInteract;   // <-- NEU

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] dialogueClips;

    [Header("Dialogue Pages")]
    [TextArea(2,6)]
    public string[] dialoguePages;

    private int currentPage = 0;

    void Start()
    {
        panelDialogue.SetActive(false);
    }

    private bool hasStarted = false;

    public void Interact()
    {
        if (hasStarted) return; // verhindert doppelten Aufruf
        hasStarted = true;

        if (buttonInteract != null)
            buttonInteract.SetActive(false);

        currentPage = 0;

        panelDialogue.SetActive(true);
        ShowPage();
    }

    public void ShowPage()
    {
        dialogueText.text = dialoguePages[currentPage];

        // Audio abspielen
        if (dialogueClips[currentPage] != null)
        {
            audioSource.clip = dialogueClips[currentPage];
            audioSource.Play();
        }

        // Buttons steuern
        buttonBack.SetActive(currentPage > 0);
        buttonContinue.SetActive(currentPage < dialoguePages.Length - 1);
        buttonStartSimulation.SetActive(currentPage == dialoguePages.Length - 1);
    }

    public void NextPage()
    {
        if (currentPage < dialoguePages.Length - 1)
        {
            currentPage++;
            ShowPage();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage();
        }
    }

    public void StartSimulation()
{
    Debug.Log("[PoliceIntro] StartSimulation pressed");

    // Audio stoppen
    if (audioSource != null)
        audioSource.Stop();

    // Dialog-Panel aus
    panelDialogue.SetActive(false);

    // gesamtes Intro-Canvas aus
    if (rootCanvas != null)
        rootCanvas.SetActive(false);

    // Simulation starten (EINZIGER Startpunkt)
    var sim = FindAnyObjectByType<SimulationManager>();
    if (sim != null)
    {
        sim.BeginSimulation();
    }
    else
    {
        Debug.LogError("[PoliceIntro] SimulationManager not found!");
    }
}

}
