using System;

[System.Serializable]
public class TriageRecord
{
    public string patientId;
    public string recommendedTriage;
    public string userTriage;
    public float decisionTime;
    public bool triageCompleted;
}

