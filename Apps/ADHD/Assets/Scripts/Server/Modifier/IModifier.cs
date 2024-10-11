using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModel
{
    public abstract class IModifier
    {
        public abstract void Apply();
        public abstract void Remove();
    }
}