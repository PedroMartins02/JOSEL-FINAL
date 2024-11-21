using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    // Reference to the MissionPoolManager
    [SerializeField] private MissionPoolManager missionPoolManager;

    // List of UI containers
    [SerializeField] private List<ContainerDisplay> containerDisplays;

    void Start()
    {
        AssignDailyMissions();
    }

    private void AssignDailyMissions()
    {
        if (missionPoolManager == null)
        {
            Debug.LogError("MissionPoolManager is not assigned!");
            return;
        }

        // Get the mission pool
        List<Container> availableMissions = missionPoolManager.GetMissionPool();

        // Ensure there are enough missions in the pool
        if (availableMissions.Count < containerDisplays.Count)
        {
            Debug.LogWarning("Not enough missions in the pool to assign unique missions to all containers!");
            return;
        }

        // Randomly assign unique missions to each container
        foreach (var containerDisplay in containerDisplays)
        {
            int randomIndex = Random.Range(0, availableMissions.Count);
            Container randomMission = availableMissions[randomIndex];

            // Assign the mission to the container
            containerDisplay.AssignMission(randomMission);

            // Remove the assigned mission from the pool to avoid repetition
            availableMissions.RemoveAt(randomIndex);
        }
    }
}
