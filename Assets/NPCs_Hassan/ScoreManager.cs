using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int correct = 0;
    public int incorrect = 0;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void CheckTriage(string chosenColor, NPCCondition condition)
    {
        if (chosenColor.ToLower() == condition.recommendedTriage.ToLower())
        {
            correct++;
            Debug.Log("Correct triage! Total correct: " + correct);
        }
        else
        {
            incorrect++;
            Debug.Log("Incorrect triage. Total incorrect: " + incorrect);
        }
    }
}
