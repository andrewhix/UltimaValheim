using System;
using System.Collections.Generic;

namespace UltimaValheim.Core
{
    /// <summary>
    /// Simple modular event routing system for cross-module communication.
    /// </summary>
    public class EventBus
    {
        private readonly Dictionary<string, Action<object>> _events = new();

        /// <summary>
        /// Subscribe a listener to a named event.
        /// </summary>
        public void Subscribe(string eventName, Action<object> handler)
        {
            if (!_events.ContainsKey(eventName))
                _events[eventName] = delegate { };

            _events[eventName] += handler;
        }

        /// <summary>
        /// Unsubscribe a listener from a named event.
        /// </summary>
        public void Unsubscribe(string eventName, Action<object> handler)
        {
            if (_events.ContainsKey(eventName))
                _events[eventName] -= handler;
        }

        /// <summary>
        /// Publish an event to all subscribers.
        /// </summary>
        public void Publish(string eventName, object data = null)
        {
            if (_events.TryGetValue(eventName, out var handlers))
            {
                try
                {
                    handlers?.Invoke(data);
                }
                catch (Exception ex)
                {
                    CoreAPI.Log.LogError($"[UVC Core] EventBus error on '{eventName}': {ex}");
                }
            }
        }

        /// <summary>
        /// Clears all registered event listeners.
        /// </summary>
        public void Clear() => _events.Clear();
    }
}
