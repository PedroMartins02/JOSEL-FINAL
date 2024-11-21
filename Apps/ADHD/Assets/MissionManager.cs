using System.Collections.Generic;
using UnityEngine;

public class MissionPoolManager : MonoBehaviour
{
    // List of all available missions
    [SerializeField] private List<Container> allMissions;

    // Public method to get a copy of the mission pool
    public List<Container> GetMissionPool()
    {
        return new List<Container>(allMissions); // Return a copy to avoid modifying the original list
    }
}
