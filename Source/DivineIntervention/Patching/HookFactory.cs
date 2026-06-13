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

        /// <summary>
        /// Creates a new dynamic hook for a specific method.
        /// </summary>
        /// <typeparam name="T">The type containing the method.</typeparam>
        /// <param name="methodName">The name of the method to hook.</param>
        /// <param name="onPrefix">The logic to run before the method executes.</param>
        /// <param name="condition">Optional condition; hook only fires if this returns true.</param>
        /// <returns>An <see cref="IHook"/> instance which can be used to <see cref="IHook.Unpatch"/> later.</returns>
        public static IHook Create<T>(string methodName, Action<T> onPrefix, Func<bool> condition = null)
        {
            var targetMethod = AccessTools.Method(typeof(T), methodName);

            if (targetMethod == null)
            {
                DivineLog.Error($"[HookFactory] Could not find method {methodName} on type {typeof(T).Name}");
                return null;
            }

            var hook = new GenericHook<T>(targetMethod, onPrefix, condition);

            HookDispatcher.Register(targetMethod, hook);

            if (_patchedMethods.Add(targetMethod))
            {
                // Ensure we only patch the actual game method once
                _harmony.Patch(targetMethod, prefix: new HarmonyMethod(typeof(HookDispatcher), nameof(HookDispatcher.PrefixForwarder)));
            }

            return hook;
        }
    }
}