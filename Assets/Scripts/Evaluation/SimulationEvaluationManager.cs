using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SimulationEvaluationManager : MonoBehaviour
{
    public static SimulationEvaluationManager Instance;

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

    // -------------------------
    // SIMULATION
    // -------------------------
    public void StartSimulation()
    {
        result.simulationStartTime = Time.time;
    }

    public void EndSimulation()
    {
        result.simulationEndTime = Time.time;
        result.totalDuration = result.simulationEndTime - result.simulationStartTime;
        SaveToJson();
    }

    // -------------------------
    // STRESSORS
    // -------------------------
    public void StressorStarted(string name)
    {
        result.stressors.Add(new StressorRecord
        {
            stressorName = name,
            startTime = Time.time
        });
    }

    public void StressorEnded(string name)
    {
        var s = result.stressors.FindLast(x => x.stressorName == name && x.endTime < 0);
        if (s != null)
            s.endTime = Time.time;
    }

    // -------------------------
    // TRIAGE
    // -------------------------
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


    // -------------------------
    // JSON
    // -------------------------
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
