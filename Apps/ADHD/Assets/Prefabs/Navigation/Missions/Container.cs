using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Container", menuName = "Container")]
public class Container : ScriptableObject
{
   // public new string name;
    public string description;
    //public Sprite background;
    public string task_completion;
   // public int id;
    public int coins;
}
