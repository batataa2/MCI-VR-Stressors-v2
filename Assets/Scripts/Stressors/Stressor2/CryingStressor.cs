using UnityEngine;
using System.Collections;

public class CryingStressor : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeOutDuration = 2f;

    private bool isActive = false;

    // -------------------------------------------------
    // Stressor START
    // -------------------------------------------------
    public void StartStressor()
    {
        if (isActive) return;
        isActive = true;

        if (audioSource != null)
            audioSource.Play();

        // Evaluation: Start loggen
        SimulationEvaluationManager.Instance.StressorStarted("Crying");
    }

    // -------------------------------------------------
    // Stressor ENDE (ausfaden)
    // -------------------------------------------------
    public void FadeOut()
    {
        if (!isActive) return;
        isActive = false;

        if (audioSource != null)
            StartCoroutine(FadeOutCoroutine());
        else
            EndStressor(); // Sicherheit
    }

    // -------------------------------------------------
    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;

        EndStressor();
    }

    // -------------------------------------------------
    // Evaluation: Ende vom Weinen loggen
    // -------------------------------------------------
    private void EndStressor()
    {
        SimulationEvaluationManager.Instance.StressorEnded("Crying");
    }
}
