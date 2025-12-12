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

        Debug.Log($"[SmokeStressor] Found {particles.Length} ParticleSystems, {audios.Length} AudioSources");

        SafeStopAll();
    }

    // -------------------------------------------------
    public void StartSmoke()
    {
        if (isActive) return;
        isActive = true;

        gameObject.SetActive(true);

        Debug.Log("[SmokeStressor] StartSmoke()");

        SimulationEvaluationManager.Instance?.StressorStarted("Smoke");

        StartCoroutine(FadeIn());
    }

    // -------------------------------------------------
    public void StopSmoke()
    {
        if (!isActive) return;
        isActive = false;

        Debug.Log("[SmokeStressor] StopSmoke()");

        StartCoroutine(FadeOut());
    }

    // -------------------------------------------------
    private IEnumerator FadeIn()
    {
        float t = 0f;

        foreach (var p in particles)
        {
            if (p != null)
                p.Play();
        }

        foreach (var a in audios)
        {
            if (a != null)
            {
                a.volume = 0f;
                a.Play();
            }
        }

        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float v = t / fadeInDuration;

            foreach (var a in audios)
            {
                if (a != null)
                    a.volume = Mathf.Lerp(0f, 1f, v);
            }

            yield return null;
        }
    }

    // -------------------------------------------------
    private IEnumerator FadeOut()
    {
        float t = 0f;

        float[] startVolumes = new float[audios.Length];
        for (int i = 0; i < audios.Length; i++)
            startVolumes[i] = audios[i] != null ? audios[i].volume : 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float v = 1f - (t / fadeOutDuration);

            for (int i = 0; i < audios.Length; i++)
            {
                if (audios[i] != null)
                    audios[i].volume = startVolumes[i] * v;
            }

            yield return null;
        }

        SafeStopAll();

        SimulationEvaluationManager.Instance?.StressorEnded("Smoke");
    }

    // -------------------------------------------------
    private void SafeStopAll()
    {
        foreach (var p in particles)
        {
            if (p != null)
                p.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        foreach (var a in audios)
        {
            if (a != null)
                a.Stop();
        }
    }
}
