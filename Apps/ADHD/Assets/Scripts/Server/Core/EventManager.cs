using System;
using System.Collections.Generic;

namespace GameCore
{
    public static class EventManager
    {
        private static Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>();

        public static void Subscribe(string eventName, Action<object> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                eventDictionary[eventName] = thisEvent + listener;
            }
            else
            {
                eventDictionary.Add(eventName, listener);
            }
        }

        public static void Unsubscribe(string eventName, Action<object> listener)
        {
            if (eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                eventDictionary[eventName] -= listener;
            }
        }

        public static void TriggerEvent(string eventName, object parameter = null)
        {
            if (eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent?.Invoke(parameter);
            }
        }
    }
}