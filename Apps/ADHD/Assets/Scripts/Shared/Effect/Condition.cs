using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool IsTrue();
    }
}