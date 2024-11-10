using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI objectiveText;

    public void UpdateObjectiveText(string text)
    {
        objectiveText.text = text;
    }
}
