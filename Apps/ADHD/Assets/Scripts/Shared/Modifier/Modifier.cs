using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public abstract class Modifier : ScriptableObject
    {
        public abstract void Apply(IModifiable[] targets);
        public abstract void Remove(IModifiable[] targets);
    }
}