using System.Collections;
using System.Collections.Generic;
using GameCore.Events;
using Unity.Netcode;
using UnityEngine;


namespace Game.Logic.Actions.UI
{
    public class UIActionQueueManager : MonoBehaviour
    {
        private Queue<IUIAction> uiActionQueue = new Queue<IUIAction>();
        private Queue<IUIAction> uiPriorityActionQueue = new Queue<IUIAction>();
        private bool isProcessing = false;

        public static UIActionQueueManager Instance { get; private set; }

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
        }

        private void OnDestroy()
        {
            ClearQueue();
        }

        public void EnqueueAction(IUIAction action)
        {
            uiActionQueue.Enqueue(action);

            if (!isProcessing)
            {
                StartCoroutine(ProcessActions());
            }
        }

        public void EnqueuePriorityAction(IUIAction action)
        {
            uiPriorityActionQueue.Enqueue(action);

            if (!isProcessing)
            {
                StartCoroutine(ProcessActions());
            }
        }

        private IEnumerator ProcessActions()
        {
            isProcessing = true;

            while (uiPriorityActionQueue.Count > 0 || uiActionQueue.Count > 0)
            {
                IUIAction nextAction = null;

                if (uiPriorityActionQueue.Count > 0)
                {
                    nextAction = uiPriorityActionQueue.Dequeue();
                }
                else if (uiActionQueue.Count > 0)
                {
                    nextAction = uiActionQueue.Dequeue();
                }

                if (nextAction != null)
                {
                    yield return StartCoroutine(nextAction.Execute());

                    EventManager.TriggerEvent(GameEventsEnum.ActionExecuted, nextAction);
                }
                else
                {
                    yield return null;
                }
            }

            isProcessing = false;
        }

        public void ClearQueue()
        {
            uiActionQueue.Clear();
            uiPriorityActionQueue.Clear();
        }
    }
}