using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameModel
{
    public static class ActionQueueManager
    {
        private static Queue<IAction> actionQueue = new Queue<IAction>();
        private static bool isProcessing = false;

        public static void AddAction(IAction action)
        {
            actionQueue.Enqueue(action);
        }

        public static void AddPriorityAction(IAction action)
        {
            var newQueue = new Queue<IAction>();
            newQueue.Enqueue(action);

            while (actionQueue.Count > 0)
            {
                newQueue.Enqueue(actionQueue.Dequeue());
            }

            actionQueue = newQueue;
        }

        private async static Task ProcessNextAction()
        {
            if (isProcessing) return;

            isProcessing = true;

            if (actionQueue.Count > 0)
            {
                IAction nextAction = actionQueue.Dequeue();

                await nextAction.Execute();
            }

            isProcessing = false;
        }

        public async static void ProcessAllPendingActions()
        {
            while (actionQueue.Count > 0)
            {
                await ProcessNextAction();
            }
        }

        public static void ClearQueue()
        {
            actionQueue.Clear();
        }
    }
}
