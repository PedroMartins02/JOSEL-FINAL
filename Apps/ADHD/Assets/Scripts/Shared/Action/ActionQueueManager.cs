using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCore.Events;
using UnityEngine;

namespace Game.Logic.Actions
{
    public class ActionQueueManager : MonoBehaviour
    {
        public static ActionQueueManager Instance { get; private set; }

        private Queue<IAction> actionQueue;
        private Queue<IAction> priorityActionQueue;

        private static bool isProcessing = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            actionQueue = new();
            priorityActionQueue = new();
        }

        public void AddAction(IAction action)
        {
            actionQueue.Enqueue(action);

            if (!isProcessing)
            {
                Debug.Log("Processing Actions");
                StartCoroutine(ProcessActions());
            }
        }

        public void AddPriorityAction(IAction action)
        {
            actionQueue.Enqueue(action);

            if (!isProcessing)
            {
                StartCoroutine(ProcessActions());
            }
        }

        public IEnumerator ProcessActions()
        {
            isProcessing = true;

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
                    yield return StartCoroutine(nextAction.Execute());

                    EventManager.TriggerEvent(GameEventsEnum.ActionExecuted, nextAction);
                }
                else
                {
                    Debug.Log("Action was Illegal");
                    // Illegal Action
                    yield return null;
                }
            }

            isProcessing = false;
        }

        public void ClearQueue()
        {
            actionQueue.Clear();
        }
    }
}
