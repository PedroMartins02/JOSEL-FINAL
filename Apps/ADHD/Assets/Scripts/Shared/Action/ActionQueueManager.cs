using System.Collections;
using System.Collections.Generic;
using GameCore.Events;

namespace Game.Logic.Actions
{
    public class ActionQueueManager
    {
        public static ActionQueueManager Instance { get; private set; } = new ActionQueueManager();

        private Queue<IAction> actionQueue;
        private Queue<IAction> priorityActionQueue;

        private ActionQueueManager() 
        {
            actionQueue = new();
            priorityActionQueue = new();
        }

        public void AddAction(IAction action)
        {
            actionQueue.Enqueue(action);
            ProcessActions();
        }

        public void AddPriorityAction(IAction action)
        {
            actionQueue.Enqueue(action);
            ProcessActions();
        }

        public void ProcessActions()
        {
            while (priorityActionQueue.Count > 0 || actionQueue.Count > 0)
            {
                IAction nextAction = null;

                if (priorityActionQueue.Count > 0)
                {
                    nextAction = priorityActionQueue.Dequeue();
                }
                else if (actionQueue.Count > 0)
                {
                    nextAction = actionQueue.Dequeue();
                }

                if (nextAction != null && nextAction.IsLegal())
                {
                    nextAction.Execute();

                    EventManager.TriggerEvent(GameEventsEnum.ActionExecuted, nextAction);
                }
                else
                {
                    // Handle Illegal actions here
                }
            }
        }

        public void ClearQueue()
        {
            actionQueue.Clear();
        }
    }
}
