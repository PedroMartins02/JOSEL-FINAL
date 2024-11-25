using System;
using System.Collections.Generic;

namespace GameCore.Events
{
    public static class EventManager
    {
        private static Dictionary<GameEventsEnum, Action<object>> eventDictionary = new Dictionary<GameEventsEnum, Action<object>>();

        private static readonly object _lock = new object();

        public static void Subscribe(GameEventsEnum eventType, Action<object> listener)
        {
            lock (_lock)
            {
                if (eventDictionary.TryGetValue(eventType, out var thisEvent))
                {
                    eventDictionary[eventType] = thisEvent + listener;
                }
                else
                {
                    eventDictionary.Add(eventType, listener);
                }
            }
        }

        public static void Unsubscribe(GameEventsEnum eventType, Action<object> listener)
        {
            lock (_lock)
            {
                if (eventDictionary.TryGetValue(eventType, out var thisEvent))
                {
                    thisEvent -= listener;

                    if (thisEvent == null)
                    {
                        eventDictionary.Remove(eventType);
                    }
                    else
                    {
                        eventDictionary[eventType] = thisEvent;
                    }
                }
            }
        }

        public static void TriggerEvent(GameEventsEnum eventType, object parameter = null)
        {
            lock (_lock)
            {
                if (eventDictionary.TryGetValue(eventType, out var thisEvent))
                {
                    thisEvent?.Invoke(parameter);
                }
            }
        }
    }
}