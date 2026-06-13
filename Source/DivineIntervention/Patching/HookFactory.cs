/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

using DivineIntervention.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DivineIntervention.Patching
{
    /// <summary>
    /// Factory for creating and managing dynamic method hooks.
    /// </summary>
    public static class HookFactory
    {
        private static readonly Harmony _harmony = new Harmony("com.kgivler.divineIntervention");
        private static readonly HashSet<MethodBase> _patchedMethods = new HashSet<MethodBase>();

        // OBSERVER OVERLOAD: Accepts Action<T>
        public static IHook Create<T>(string methodName, Action<T> onPrefix, Func<bool> condition = null)
        {
            // Explicitly map this Action to a Func that returns 'true'
            return Create<T>(methodName,
                onPrefix: (instance) =>
                {
                    onPrefix(instance);
                    return true;
                },
                onPostfix: null,
                condition: condition);
        }

        // INTERCEPTOR OVERLOAD: Accepts Func<T, bool>
        public static IHook Create<T>(string methodName, Func<T, bool> onPrefix = null, Action<T, object> onPostfix = null, Func<bool> condition = null)
        {
            var targetMethod = AccessTools.Method(typeof(T), methodName);

            if (targetMethod == null)
            {
                DivineLog.Error($"[HookFactory] Could not find method {methodName} on type {typeof(T).Name}");
                return null;
            }

            var hook = new GenericHook<T>(targetMethod, onPrefix, onPostfix, condition);
            HookDispatcher.Register(targetMethod, hook);

            if (_patchedMethods.Add(targetMethod))
            {
                _harmony.Patch(targetMethod,
                    prefix: new HarmonyMethod(typeof(HookDispatcher), nameof(HookDispatcher.PrefixForwarder)),
                    postfix: new HarmonyMethod(typeof(HookDispatcher), nameof(HookDispatcher.PostfixForwarder))
                );
            }
            return hook;
        }
    }
}