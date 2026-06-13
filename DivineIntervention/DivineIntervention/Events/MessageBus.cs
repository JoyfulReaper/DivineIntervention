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
        /// <summary>
        /// Internal registry for the Typed Lane, mapping event <see cref="Type"/>s to their respective performance-optimized callbacks.
        /// </summary>
        private static readonly Dictionary<Type, Delegate[]> _subscribers = new Dictionary<Type, Delegate[]>();

        /// <summary>
        /// Internal registry for the Loose Lane, mapping arbitrary string topics to flexible, runtime-cast callbacks.
        /// </summary>
        private static readonly Dictionary<string, List<Action<object>>> _looseSubscribers = new Dictionary<string, List<Action<object>>>();


        #region Loose Lane (Dynamic, String-Based)

        /// <summary>
        /// Subscribes a callback delegate to a specific string topic on the "Loose Lane".
        /// </summary>
        /// <param name="topic">The unique string identifier matching the publisher's topic key.</param>
        /// <param name="action">The callback execution delegate executed when a message is published to this topic. Receives the payload as an untyped <see cref="object"/>.</param>
        /// <remarks>
        /// <para><b>Caution:</b> Because this lane passes data as an untyped <see cref="object"/>, compile-time safety is bypassed. 
        /// The subscriber is entirely responsible for safely casting the data payload back to its expected type. Incorrect casting will throw an <see cref="InvalidCastException"/> at runtime.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// MessageBus.Subscribe("ModA_SilverCountChanged", (data) => 
        /// {
        ///     int silver = (int)data; // Manual unboxing/casting required
        ///     DivineLog.Info($"Received silver count: {silver}");
        /// });
        /// </code>
        /// </example>
        public static void Subscribe(string topic, Action<object> action)
        {
            if (!_looseSubscribers.TryGetValue(topic, out var subs))
            {
                subs = new List<Action<object>>();
                _looseSubscribers[topic] = subs;
            }
            if (!subs.Contains(action))
            {
                subs.Add(action);
            }
        }

        /// <summary>
        /// Publishes a data payload to the "Loose Lane" using a string identifier. 
        /// Allows cross-mod communication without requiring shared type contracts.
        /// </summary>
        /// <param name="topic">The unique string identifier for the event channel. To avoid collisions, recommend prefixing with your ModName (e.g., "MyMod_InventoryChanged").</param>
        /// <param name="data">The payload object to broadcast. Can be any reference or value type.</param>
        /// <remarks>
        /// <para><b>Performance Note:</b> Value types passed into this method will trigger boxing allocations, making this far less optimal for high-frequency tick loops compared to the Typed Lane.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// int silverAmount = 1500;
        /// MessageBus.Publish("ModA_SilverCountChanged", silverAmount);
        /// </code>
        /// </example>
        public static void Publish(string topic, object data)
        {
            if (_looseSubscribers.TryGetValue(topic, out var subs))
            {
                for (int i = subs.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        subs[i](data);
                    }
                    catch (Exception ex)
                    {
                        // Grab the class name where the method lives, or default to "Unknown" if it fails
                        string subscriberName = subs[i].Method.DeclaringType?.Name ?? "Unknown";

                        DivineLog.Debug($"Exception in loose lane subscriber Topic: {topic}\n" +
                            $" Subscriber: {subscriberName}\n" +
                            $" Object HashCode: {(data != null ? data.GetHashCode().ToString() : "null")}\n" +
                            $" Message: {ex.Message}");
                    }
                }
            }
        }


        /// <summary>
        /// Removes a previously registered callback from a specific string topic on the "Loose Lane".
        /// </summary>
        /// <param name="topic">The unique string identifier matching the publisher's topic key.</param>
        /// <param name="action">The callback execution delegate to remove.</param>
        /// <remarks>
        /// This is critical to call during component teardown or map unloading to prevent stale references from causing memory leaks.
        /// </remarks>
        public static void Unsubscribe(string topic, Action<object> action)
        {
            if (_looseSubscribers.TryGetValue(topic, out var subs))
            {
                subs.Remove(action);
                if (subs.Count == 0)
                {
                    _looseSubscribers.Remove(topic);
                }
            }
        }

        #endregion

        #region Typed Lane (High-Performance, Compile-Time Safe)

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
        #endregion
    }
}