using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SimulationEvaluationManager : MonoBehaviour
{
    public static SimulationEvaluationManager Instance;

    public float TotalDuration { get; private set; }

    private SimulationResult result;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        result = new SimulationResult();
    }

    // -------------------------------------------------
    // SIMULATION
    // -------------------------------------------------

    // Wird beim Start der Simulation aufgerufen (optional, für Logging)
    public void StartSimulation()
    {
        result.simulationStartTime = Time.time;
        Debug.Log($"[EVAL] Simulation started at {result.simulationStartTime}");
    }

    // 🔑 WICHTIG: Dauer wird übergeben (vom TimerStressor!)
    public void EndSimulation(float totalDuration)
    {
        TotalDuration = totalDuration;
        result.totalDuration = totalDuration;

        result.simulationEndTime = Time.time;
        Debug.Log($"[EVAL] Simulation ended, duration={TotalDuration}");

        SaveToJson();
    }

    // Getter für Scoreboard / UI
    public float GetTotalDuration()
    {
        return TotalDuration;
    }

    public SimulationResult GetResult()
    {
        return result;
    }

    // -------------------------------------------------
    // STRESSORS
    // -------------------------------------------------
    public void StressorStarted(string name)
    {
        result.stressors.Add(new StressorRecord
        {
            stressorName = name,
            startTime = Time.time,
            endTime = -1f
        });
    }

    public void StressorEnded(string name)
    {
        var s = result.stressors.FindLast(x => x.stressorName == name && x.endTime < 0f);
        if (s != null)
            s.endTime = Time.time;
    }

    // -------------------------------------------------
    // TRIAGE
    // -------------------------------------------------
    public void RegisterTriage(
        string patientId,
        string recommended,
        string userChoice,
        float decisionTime,
        bool triageCompleted
    )
    {
        result.triages.Add(new TriageRecord
        {
            patientId = patientId,
            recommendedTriage = recommended,
            userTriage = userChoice,
            decisionTime = decisionTime,
            triageCompleted = triageCompleted
        });
    }

    // -------------------------------------------------
    // JSON
    // -------------------------------------------------
    private void SaveToJson()
    {
        string json = JsonUtility.ToJson(result, true);

        string path = Path.Combine(
            Application.persistentDataPath,
            $"simulation_result_{System.DateTime.Now:yyyyMMdd_HHmmss}.json"
        );

        File.WriteAllText(path, json);
        Debug.Log($"<color=green>Simulation saved:</color>\n{path}");
    }
}
