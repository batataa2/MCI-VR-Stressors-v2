using UnityEngine;
using TMPro;

public class SimulationScoreboardUI : MonoBehaviour
{
    public TextMeshProUGUI totalTimeText;

    private void OnEnable()
    {
        var result = SimulationEvaluationManager.Instance.GetResult();

        totalTimeText.text =
            $"Total Time: {result.totalDuration:F1} seconds";
    }
}
