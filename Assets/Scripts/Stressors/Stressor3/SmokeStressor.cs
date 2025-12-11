using UnityEngine;
using System.Collections;

public class SmokeStressor : MonoBehaviour
{
    public float fadeInDuration = 2f;
    public float fadeOutDuration = 2f;

    private ParticleSystem[] particles;
    private AudioSource[] audios;

    private bool isActive = false;

    void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>(true);
        audios = GetComponentsInChildren<AudioSource>(true);

        // Beim Start alle Effekte deaktivieren
        foreach (var p in particles)
        {
            p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        foreach (var a in audios)
        {
            a.volume = 0f;
            a.Stop();
        }
    }

    // Vom SimulationManager aufgerufen
    public void StartSmoke()
    {
        if (isActive) return;

        isActive = true;
        gameObject.SetActive(true);

        StartCoroutine(FadeIn());
    }

    public void StopSmoke()
    {
        if (!isActive) return;

        isActive = false;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;

        // Partikel starten
        foreach (var p in particles)
            p.Play();

        // Audio starten
        foreach (var a in audios)
        {
            a.volume = 0f;
            a.Play();
        }

        // Lautstärke langsam hochfahren
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float v = t / fadeInDuration;

            foreach (var a in audios)
                a.volume = Mathf.Lerp(0f, 1f, v);

            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;

        float[] startVolumes = new float[audios.Length];
        for (int i = 0; i < audios.Length; i++)
            startVolumes[i] = audios[i].volume;

        // Volume runterfahren
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float v = 1f - (t / fadeOutDuration);

            for (int i = 0; i < audios.Length; i++)
                audios[i].volume = startVolumes[i] * v;

            yield return null;
        }

        // Alles stoppen
        foreach (var p in particles)
        {
            p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        foreach (var a in audios)
        {
            a.Stop();
        }

        // Optional: komplettes Objekt deaktivieren
        // gameObject.SetActive(false);
    }
}
