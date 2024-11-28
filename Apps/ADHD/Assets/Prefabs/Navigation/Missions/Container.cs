using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Container", menuName = "Container")]
public class Container : ScriptableObject
{
    public string description;
    public int coins;

    // Progress-related fields
    public int progress; // Current progress
    public int scale;    // Scale to complete the task
}
