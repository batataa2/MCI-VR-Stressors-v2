using System.Collections;
using UnityEngine;

public class FireEvolutionController : MonoBehaviour
{
    [Header("Particles")]
    public ParticleSystem smoke;
    public ParticleSystem sparks;
    public ParticleSystem fire1;
    public ParticleSystem fire2;
    public ParticleSystem fire3;

    [Header("Audio")]
    public AudioSource smallFireAudio;
    public AudioSource mediumFireAudio;
    public AudioSource largeFireAudio;

    [Header("Timing")]
    public float smokePhase = 20f;
    public float sparksDelay = 10f;
    public float fire3Delay = 10f;
    public float fire2Delay = 10f;
    public float fire1Delay = 10f;
    public float fullFireDuration = 20f;

    private Coroutine fireRoutine;
    private bool fireRunning = false;

    // -------------------------------------------------
    // Initial Setup – NICHT starten
    // -------------------------------------------------
    void Awake()
    {
        StopAllCoroutines();
        StopAllParticles();
        StopAllAudio();

        fireRunning = false;
        gameObject.SetActive(false);
    }

    // -------------------------------------------------
    // Wird vom SimulationManager aufgerufen
    // -------------------------------------------------
    public void BeginFire()
    {
        if (fireRunning) return;

        fireRunning = true;
        gameObject.SetActive(true);

        //Evaluation START
        SimulationEvaluationManager.Instance.StressorStarted("Fire");

        fireRoutine = StartCoroutine(FireSequence());
    }

    // Alias (Sicherheit)
    public void StopFire()
    {
        ExtinguishFire();
    }

    // -------------------------------------------------
    IEnumerator FireSequence()
    {
        if (smoke != null)
            smoke.Play();

        yield return new WaitForSeconds(smokePhase);
        if (!fireRunning) yield break;

        if (sparks != null)
            sparks.Play();

        FadeInAudio(smallFireAudio, 0.5f, 2f);
        yield return new WaitForSeconds(sparksDelay);
        if (!fireRunning) yield break;

        if (fire3 != null)
            fire3.Play();

        FadeOutAudio(smallFireAudio, 0f, 2f);
        FadeInAudio(mediumFireAudio, 0.6f, 2f);
        yield return new WaitForSeconds(fire3Delay);
        if (!fireRunning) yield break;

        if (fire2 != null)
            fire2.Play();

        FadeOutAudio(mediumFireAudio, 0f, 2f);
        FadeInAudio(largeFireAudio, 1f, 3f);
        yield return new WaitForSeconds(fire2Delay);
        if (!fireRunning) yield break;

        if (fire1 != null)
            fire1.Play();

        yield return new WaitForSeconds(fire1Delay + fullFireDuration);
    }

    // -------------------------------------------------
    // Wird vom SimulationManager aufgerufen (Feuerwehr)
    // -------------------------------------------------
    public void ExtinguishFire()
    {
        if (!fireRunning) return;

        fireRunning = false;

        if (fireRoutine != null)
            StopCoroutine(fireRoutine);

        StartCoroutine(FadeOutAll());
    }

    // -------------------------------------------------
    IEnumerator FadeOutAll()
    {
        float fadeTime = 4f;

        FadeOutAudio(largeFireAudio, 0f, fadeTime);
        FadeOutAudio(mediumFireAudio, 0f, fadeTime);
        FadeOutAudio(smallFireAudio, 0f, fadeTime);

        yield return new WaitForSeconds(fadeTime);

        StopAllParticles();
        StopAllAudio();

        //Evaluation ENDE
        SimulationEvaluationManager.Instance.StressorEnded("Fire");

        gameObject.SetActive(false);
    }

    // -------------------------------------------------
    void StopAllParticles()
    {
        if (smoke) smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (sparks) sparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (fire1) fire1.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (fire2) fire2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (fire3) fire3.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void StopAllAudio()
    {
        if (smallFireAudio) smallFireAudio.Stop();
        if (mediumFireAudio) mediumFireAudio.Stop();
        if (largeFireAudio) largeFireAudio.Stop();
    }

    void FadeInAudio(AudioSource audio, float targetVolume, float duration)
    {
        if (audio == null) return;
        StartCoroutine(FadeAudio(audio, targetVolume, duration));
    }

    void FadeOutAudio(AudioSource audio, float targetVolume, float duration)
    {
        if (audio == null) return;
        StartCoroutine(FadeAudio(audio, targetVolume, duration));
    }

    IEnumerator FadeAudio(AudioSource audio, float targetVolume, float duration)
    {
        if (audio == null) yield break;

        audio.enabled = true;
        if (!audio.isPlaying)
            audio.Play();

        float startVolume = audio.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, targetVolume, t / duration);
            yield return null;
        }

        audio.volume = targetVolume;

        if (Mathf.Approximately(targetVolume, 0f))
            audio.Stop();
    }
}
