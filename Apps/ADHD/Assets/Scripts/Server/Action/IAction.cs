using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModel
{
    public abstract class IAction
    {
        public abstract bool IsLegal();
        public abstract Task Execute();
    }
}