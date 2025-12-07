using System.Collections;
using UnityEngine;

public class FireEvolutionController : MonoBehaviour
{
    // -----------------------------
    // Particles
    // -----------------------------
    public ParticleSystem smoke;
    public ParticleSystem sparks;
    public ParticleSystem fire1;
    public ParticleSystem fire2;
    public ParticleSystem fire3;

    // -----------------------------
    // Audio
    // -----------------------------
    public AudioSource smallFireAudio;
    public AudioSource mediumFireAudio;
    public AudioSource largeFireAudio;

    // -----------------------------
    // UI Button + Player reference
    // -----------------------------
    public GameObject fireCanvas;      // Canvas
    public Transform playerHead;       // VR camera (MainCamera)
    public float buttonShowDistance = 3f;

    // -----------------------------
    // Timing
    // -----------------------------
    public float smokePhase = 20f;
    public float sparksDelay = 10f;
    public float fire3Delay = 10f;
    public float fire2Delay = 10f;
    public float fire1Delay = 10f;

    public float fullFireDuration = 20f;
    public float fadeOutDuration = 40f;

    private bool extinguishTriggered = false;
    private bool fire1IsActive = false;

    void Start()
    {
        // UI ausblenden
        if (fireCanvas != null)
            fireCanvas.SetActive(false);

        // Audio deaktivieren
        smallFireAudio.enabled = false;
        mediumFireAudio.enabled = false;
        largeFireAudio.enabled = false;

        smallFireAudio.volume = 0;
        mediumFireAudio.volume = 0;
        largeFireAudio.volume = 0;

        // Partikel stoppen
        sparks.Stop();
        fire1.Stop();
        fire2.Stop();
        fire3.Stop();

        StartCoroutine(FireSequence());
    }

    void Update()
    {
        if (!fire1IsActive || extinguishTriggered) return;

        // Button schaut zum Spieler
        if (playerHead != null && fireCanvas != null)
        {
            fireCanvas.transform.LookAt(playerHead);
            fireCanvas.transform.Rotate(0, 180, 0);
        }

        // Distanz prüfen
        float dist = Vector3.Distance(playerHead.position, transform.position);

        if (dist < buttonShowDistance)
            fireCanvas.SetActive(true);
        else
            fireCanvas.SetActive(false);
    }

    // -----------------------------------------
    //  MAIN SEQUENCE
    // -----------------------------------------
    IEnumerator FireSequence()
    {
        smoke.Play();
        yield return new WaitForSeconds(smokePhase);

        if (extinguishTriggered) yield break;

        sparks.Play();
        StartCoroutine(FadeInAudio(smallFireAudio, 0.5f, 2f));
        yield return new WaitForSeconds(sparksDelay);

        if (extinguishTriggered) yield break;

        // -------------------------------
        // FIRST REAL FIRE STARTS HERE!
        // Button soll ab hier erscheinen
        // -------------------------------
        fire3.Play();
        fire1IsActive = true;   // <-- Button ab jetzt aktiv!

        StartCoroutine(FadeOutAudio(smallFireAudio, 0f, 2f));
        StartCoroutine(FadeInAudio(mediumFireAudio, 0.6f, 2f));
        yield return new WaitForSeconds(fire3Delay);

        if (extinguishTriggered) yield break;

        fire2.Play();
        StartCoroutine(FadeOutAudio(mediumFireAudio, 0f, 2f));
        StartCoroutine(FadeInAudio(largeFireAudio, 1f, 3f));
        yield return new WaitForSeconds(fire2Delay);

        if (extinguishTriggered) yield break;

        fire1.Play();
        yield return new WaitForSeconds(fire1Delay);

        if (extinguishTriggered) yield break;

        StartCoroutine(FadeInAudio(largeFireAudio, 1.3f, 2f));
        yield return new WaitForSeconds(fullFireDuration);

        fire1IsActive = false;
    }

    // =========================================
    // 🔥 EXTINGUISH FUNCTION
    // =========================================
    public void ExtinguishFire()
    {
        if (extinguishTriggered) return;

        extinguishTriggered = true;

        if (fireCanvas != null)
            fireCanvas.SetActive(false);

        fire1IsActive = false;

        StopAllCoroutines();

        float fastFade = 4f;

        StartCoroutine(FadeOutParticles(fire1, fastFade));
        StartCoroutine(FadeOutParticles(fire2, fastFade));
        StartCoroutine(FadeOutParticles(fire3, fastFade));
        StartCoroutine(FadeOutParticles(smoke, fastFade));

        StartCoroutine(FadeOutAudio(largeFireAudio, 0f, fastFade));
        StartCoroutine(FadeOutAudio(mediumFireAudio, 0f, fastFade));
        StartCoroutine(FadeOutAudio(smallFireAudio, 0f, fastFade));

        sparks.Stop();
    }

    // ---------------------------------------------------------
    IEnumerator FadeOutParticles(ParticleSystem ps, float duration)
    {
        var emission = ps.emission;

        float startRate = emission.rateOverTime.constant;
        float time = 0f;

        while (time < duration)
        {
            emission.rateOverTime = Mathf.Lerp(startRate, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        emission.rateOverTime = 0f;
        ps.Stop();
    }

    // ---------------------------------------------------------
    IEnumerator FadeInAudio(AudioSource audio, float targetVolume, float duration)
    {
        audio.enabled = true;
        float startVolume = audio.volume;
        audio.Play();

        float time = 0f;
        while (time < duration)
        {
            audio.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        audio.volume = targetVolume;
    }

    // ---------------------------------------------------------
    IEnumerator FadeOutAudio(AudioSource audio, float targetVolume, float duration)
    {
        float startVolume = audio.volume;
        float time = 0f;

        while (time < duration)
        {
            audio.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        audio.volume = targetVolume;

        if (targetVolume == 0f)
            audio.Stop();
    }
}
