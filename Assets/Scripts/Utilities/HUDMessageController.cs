using UnityEngine;
using TMPro;
using System.Collections;

public class HUDMessageController : MonoBehaviour
{
    public static HUDMessageController Instance { get; private set; }

    [Header("UI")]
    public TMP_Text messageText;

    [Header("Animation")]
    public float pulseSpeed = 2f;
    public Color colorA = Color.red;
    public Color colorB = Color.white;

    private Coroutine pulseRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (messageText != null)
            messageText.text = "";
    }

    // -----------------------------------
    // PERSISTENTE MELDUNG
    // -----------------------------------
    public void ShowPersistentMessage(string message)
    {
        if (messageText == null) return;

        messageText.text = message;

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseText());
    }

    // -----------------------------------
    // MELDUNG LÖSCHEN
    // -----------------------------------
    public void ClearMessage()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        if (messageText != null)
            messageText.text = "";
    }

    // -----------------------------------
    // TEXT PULSIERT + FARBE WECHSELT
    // -----------------------------------
    private IEnumerator PulseText()
    {
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * pulseSpeed;

            if (messageText != null)
            {
                messageText.color = Color.Lerp(colorA, colorB, Mathf.PingPong(t, 1));
                float scale = 1f + Mathf.Sin(t) * 0.05f;
                messageText.transform.localScale = Vector3.one * scale;
            }

            yield return null;
        }
    }
}
