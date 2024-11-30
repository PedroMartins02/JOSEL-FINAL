using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Logic.Actions.UI
{
    public interface IUIAction
    {
        public IEnumerator Execute();
    }
}
