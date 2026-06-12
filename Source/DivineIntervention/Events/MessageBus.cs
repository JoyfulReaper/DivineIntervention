/*
 * Divine Intervention RimWorld Modding Framework
 * 
 * Make Mods the Right Way(tm)
 * 
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using DivineIntervention.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DivineIntervention.Events
{
    /// <summary>
    /// A high-performance, zero-allocation message broker.
    /// Facilitates decoupled communication between independent mod components by enabling
    /// publish-subscribe event patterns without requiring direct class references.
    /// </summary>
    public static class MessageBus
    {
        // Store an Array instead of a List. Arrays are immutable in size.
        private static readonly Dictionary<Type, Delegate[]> _subscribers = new Dictionary<Type, Delegate[]>();

        /// <summary>
        /// Registers a callback function to be executed when a message of type <typeparamref name="TMessage"/> is published.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to subscribe to.</typeparam>
        /// <param name="action">The callback delegate to execute.</param>
        /// <remarks>
        /// This is a "Cold Path" operation. It incurs a memory allocation to update the internal 
        /// subscription cache and should be used during mod initialization, not frame-by-frame updates.
        /// </remarks>
        public static void Subscribe<TMessage>(Action<TMessage> action)
        {
            var type = typeof(TMessage);
            if (_subscribers.TryGetValue(type, out var existing))
            {
                // COLD PATH: Pay the allocation cost here for now
                var list = existing.ToList();
                if (!list.Contains(action))
                {
                    list.Add(action);
                    _subscribers[type] = list.ToArray();
                }
            }
            else
            {
                _subscribers[type] = new Delegate[] { action };
            }
        }

        /// <summary>
        /// Removes a previously registered callback for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to unsubscribe from.</typeparam>
        /// <param name="action">The callback delegate to remove.</param>
        /// <remarks>
        /// This is a "Cold Path" operation. It incurs a memory allocation to update the internal 
        /// subscription cache and should be used during cleanup phases.
        /// </remarks>
        public static void Unsubscribe<TMessage>(Action<TMessage> action)
        {
            var type = typeof(TMessage);
            if (_subscribers.TryGetValue(type, out var existing))
            {
                // COLD PATH: Pay the allocation cost here.
                var list = existing.ToList();
                if (list.Remove(action))
                {
                    if (list.Count == 0)
                        _subscribers.Remove(type);
                    else
                        _subscribers[type] = list.ToArray(); // Replace with new array
                }
            }
        }

        /// <summary>
        /// Broadcasts a message payload to all registered subscribers for the given type.
        /// </summary>
        /// <typeparam name="TMessage">The type of message to publish.</typeparam>
        /// <param name="message">The message payload to distribute to subscribers.</param>
        /// <remarks>
        /// This is a "Hot Path" operation. It is highly optimized for zero-allocation performance,
        /// ensuring game performance is not impacted by frequent event broadcasting. 
        /// Exceptions thrown by subscribers are caught and logged to the <see cref="DivineLog"/> system.
        /// </remarks>
        public static void Publish<TMessage>(TMessage message)
        {
            var type = typeof(TMessage);
            if (!_subscribers.TryGetValue(type, out var snapshot))
                return;

            // HOT PATH: Zero memory allocation!
            // We iterate over the array reference directly using a standard 'for' loop.
            // If someone Unsubscribes during this loop, they replace the dictionary entry 
            // with a NEW array. Our 'snapshot' reference remains completely safe and intact.

            for (int i = 0; i < snapshot.Length; i++)
            {
                try
                {
                    ((Action<TMessage>)snapshot[i])(message);
                }
                catch (Exception ex)
                {
                    DivineLog.Error($"Exception during MessageBus publish for {type.Name}: {ex.Message}");
                }
            }
        }
    }
}