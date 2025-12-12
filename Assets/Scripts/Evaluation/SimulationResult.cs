using System;
using System.Collections.Generic;

[Serializable]
public class SimulationResult
{
    public float simulationStartTime;
    public float simulationEndTime;
    public float totalDuration;

    public List<StressorRecord> stressors = new List<StressorRecord>();
    public List<TriageRecord> triages = new List<TriageRecord>();
}
