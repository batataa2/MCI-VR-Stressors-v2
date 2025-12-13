using UnityEngine;
using TMPro;

public class ScoreboardController : MonoBehaviour
{
    [Header("Root (wird ein/ausgeschaltet)")]
    public GameObject scoreboardRoot;

    [Header("Texts")]
    public TMP_Text totalTimeText;
    public TMP_Text[] patientTexts;
    public TMP_Text[] recommendedTexts;
    public TMP_Text[] userTexts;

    [Header("Patients (in Reihenfolge!)")]
    public NPCCondition[] patientsInOrder;

    private void Awake()
    {
        // GARANTIERT unsichtbar beim Start
        scoreboardRoot.SetActive(false);
    }

    public void ShowScoreboard()
    {
        FillScoreboard();
        scoreboardRoot.SetActive(true);
    }

    private void FillScoreboard()
    {
        var eval = SimulationEvaluationManager.Instance;
        if (eval == null)
        {
            Debug.LogError("[Scoreboard] SimulationEvaluationManager fehlt!");
            return;
        }

        float t = eval.GetResult().totalDuration;
        int min = Mathf.FloorToInt(t / 60f);
        int sec = Mathf.FloorToInt(t % 60f);

        totalTimeText.text = $"Total Time: {min:00}:{sec:00}";

        int rows = Mathf.Min(
            patientsInOrder.Length,
            patientTexts.Length,
            recommendedTexts.Length,
            userTexts.Length
        );

        for (int i = 0; i < rows; i++)
        {
            var p = patientsInOrder[i];
            if (p == null) continue;

            patientTexts[i].text = p.patientName;
            recommendedTexts[i].text = p.recommendedTriage;

            userTexts[i].text =
                p.triageCompleted && !string.IsNullOrEmpty(p.playerTriage)
                ? p.playerTriage
                : "no choice";
        }
    }
}
