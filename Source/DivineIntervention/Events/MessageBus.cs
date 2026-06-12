/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

using System;
using System.Collections.Generic;

namespace ForgeCore.Events
{
    public static class MessageBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        public static void Subscribe<TMessage>(Action<TMessage> action)
        {
            var type = typeof(TMessage);
            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<Delegate>();
            }
            _subscribers[type].Add(action);
        }

        public static void Publish<TMessage>(TMessage message)
        {
            var type = typeof(TMessage);
            if (_subscribers.TryGetValue(type, out var actions))
            {
                foreach (var action in actions)
                {
                    ((Action<TMessage>)action)(message);
                }
            }
        }
    }
}