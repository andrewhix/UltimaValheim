using System;
using System.Collections.Generic;

namespace UltimaValheim.Core
{
    /// <summary>
    /// Provides a type-safe event system built on top of the Core EventBus.
    /// </summary>
    public class CoreEventRouter
    {
        private readonly Dictionary<Type, List<Delegate>> _typedListeners = new();

        /// <summary>
        /// Subscribe to a specific typed event.
        /// </summary>
        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_typedListeners.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _typedListeners[type] = list;
            }
            list.Add(handler);
        }

        /// <summary>
        /// Unsubscribe from a specific typed event.
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_typedListeners.TryGetValue(type, out var list))
            {
                list.Remove(handler);
            }
        }

        /// <summary>
        /// Publish a typed event payload to all listeners of its type.
        /// </summary>
        public void Publish<T>(T payload)
        {
            var type = typeof(T);
            if (!_typedListeners.TryGetValue(type, out var list)) return;

            foreach (var handler in list)
            {
                try
                {
                    ((Action<T>)handler)?.Invoke(payload);
                }
                catch (Exception ex)
                {
                    CoreAPI.Log.LogError($"[EventRouter] Error dispatching {type.Name}: {ex}");
                }
            }
        }

        /// <summary>
        /// Clears all typed event listeners (called on Core shutdown if needed).
        /// </summary>
        public void Clear()
        {
            _typedListeners.Clear();
        }
    }
}
