using UnityEngine;
using TMPro;

public class TriageCardUI : MonoBehaviour
{
    public TMP_Text injuryText;
    public TMP_Text vitalsText;
    public TMP_Text breathingText;
    public TMP_Text consciousnessText;

    private NPCCondition currentCondition;

    public void Display(NPCCondition c)
    {
        currentCondition = c;

        injuryText.text = c.injuryDescription;
        vitalsText.text = c.vitalSigns;
        breathingText.text = c.isBreathing ? "Breathing" : "Not Breathing";
        consciousnessText.text = c.isConscious ? "Conscious" : "Unconscious";
    }

    public void AssignTriage(string color)
    {
        ScoreManager.instance.CheckTriage(color, currentCondition);
        this.gameObject.SetActive(false);
    }
}
