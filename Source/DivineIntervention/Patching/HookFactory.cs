/*
* Divine Intervention RimWorld Modding Framework
* * Make Mods the Right Way(tm)
* * Copyright (c) 2026 Kyle Givler
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
    /// Primary API entry point for creating and managing dynamic method hooks.
    /// </summary>
    public static class HookFactory
    {
        private static readonly Harmony _harmony = new Harmony("com.kgivler.divineIntervention");
        private static readonly HashSet<MethodBase> _patchedMethods = new HashSet<MethodBase>();

        /// <summary>
        /// Initializes the factory and subscribes to dispatcher cleanup events.
        /// </summary>
        static HookFactory()
        {
            HookDispatcher.OnMethodEmpty += (method) =>
            {
                _harmony.Unpatch(method, HarmonyPatchType.All);
                _patchedMethods.Remove(method);
            };
        }

        /// <summary>
        /// Creates a simple Observer hook that runs logic before a method executes without altering control flow.
        /// </summary>
        /// <typeparam name="T">The type containing the target method.</typeparam>
        /// <param name="methodName">The exact string name of the method.</param>
        /// <param name="onPrefix">An action to perform using the object instance.</param>
        /// <param name="condition">Optional condition; hook only executes if this returns true.</param>
        /// <returns>An <see cref="IHook"/> handle that can be used to unpatch.</returns>
        public static IHook Create<T>(string methodName, Action<T> onPrefix, Func<bool> condition = null)
        {
            HookPrefix<T> prefixWrapper = (instance, args) =>
            {
                onPrefix(instance);
                return true;
            };

            return Create<T>(methodName, prefixWrapper, null, condition);
        }

        /// <summary>
        /// Creates a Power User hook capable of intercepting execution, mutating arguments, and altering return values.
        /// </summary>
        /// <typeparam name="T">The type containing the target method.</typeparam>
        /// <param name="methodName">The exact string name of the method.</param>
        /// <param name="onPrefix">Logic executed before the method; return false to skip the original method.</param>
        /// <param name="onPostfix">Logic executed after the method; allows modification of the final result via reference.</param>
        /// <param name="condition">Optional condition; hook only executes if this returns true.</param>
        /// <returns>An <see cref="IHook"/> handle that can be used to unpatch.</returns>
        public static IHook Create<T>(string methodName, HookPrefix<T> onPrefix = null, HookPostfix<T> onPostfix = null, Func<bool> condition = null)
        {
            var targetMethod = AccessTools.Method(typeof(T), methodName);

            if (targetMethod == null)
            {
                DivineLog.Error($"[HookFactory] Could not find method {methodName} on type {typeof(T).Name}");
                return null;
            }

            var hook = new GenericHook<T>(targetMethod, onPrefix, onPostfix, condition);
            HookDispatcher.Register(targetMethod, hook);

            // Apply Harmony patches only if this is the first hook for this specific method
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