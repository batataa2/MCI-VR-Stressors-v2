using UnityEngine;
using System.Collections;

public class CryingStressor : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeOutDuration = 2f;

    public void StartStressor()
    {
        if (audioSource != null)
            audioSource.Play();
    }

    public void FadeOut()
    {
        if (audioSource != null)
            StartCoroutine(FadeOutCoroutine());
    }

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
    }
}
