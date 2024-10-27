using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameModel
{
    public static class ActionQueueManager
    {
        private static Queue<Action> actionQueue = new Queue<Action>();
        private static bool isProcessing = false;

        public static void AddAction(Action action)
        {
            actionQueue.Enqueue(action);
        }

        public static void AddPriorityAction(Action action)
        {
            var newQueue = new Queue<Action>();
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
                Action nextAction = actionQueue.Dequeue();

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
