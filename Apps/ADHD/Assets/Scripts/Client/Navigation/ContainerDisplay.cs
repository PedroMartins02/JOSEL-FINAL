using UnityEngine;
using TMPro;

public class ContainerDisplay : MonoBehaviour
{
    // UI Elements
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI taskcompletionText;
    [SerializeField] private TextMeshProUGUI coinsText;

    // Assigned mission (ScriptableObject)
    private Container assignedMission;

    // Assign a mission to this container
    public void AssignMission(Container mission)
    {
        assignedMission = mission;
        UpdateUI();
    }

    // Update the UI to reflect the mission details
    private void UpdateUI()
    {
        if (assignedMission == null) return;

        descriptionText.text = assignedMission.description;
        taskcompletionText.text = assignedMission.task_completion;
        coinsText.text = assignedMission.coins.ToString();
    }
}
