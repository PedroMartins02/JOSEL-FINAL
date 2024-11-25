using System.Collections;
using System.Threading.Tasks;

namespace Game.Logic.Actions
{
    public interface IAction
    {
        public bool IsLegal();
        public IEnumerator Execute();
    }
}