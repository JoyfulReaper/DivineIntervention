/*
* Divine Intervention RimWorld Modding Framework
* * Make Mods the Right Way(tm)
* * Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DivineIntervention.Hooking.Internal;

/// <summary>
/// Internal dispatcher that routes Harmony-injected game method calls to registered <see cref="IHook"/> instances.
/// </summary>
internal static class HookDispatcher
{
    private static readonly Dictionary<MethodBase, List<IHook>> _registry = new();

    /// <summary>
    /// Event triggered when a method no longer has any active hooks, allowing the factory to clean up Harmony patches.
    /// </summary>
    public static event Action<MethodBase> OnMethodEmpty;

    /// <summary>
    /// Registers a new hook for a specific method.
    /// </summary>
    internal static void Register(MethodBase method, IHook hook)
    {
        if (!_registry.ContainsKey(method)) _registry[method] = new();
        _registry[method].Add(hook);
    }

    /// <summary>
    /// Unregisters a hook and triggers cleanup if it was the last active hook for the method.
    /// </summary>
    public static void Unregister(MethodBase method, IHook hook)
    {
        if (_registry.TryGetValue(method, out var hooks))
        {
            bool removed = hooks.Remove(hook);
            if (hooks.Count == 0)
            {
                _registry.Remove(method);
                OnMethodEmpty?.Invoke(method);
            }
        }
    }

    /// <summary>
    /// Injected by Harmony prior to the original method executing. Routes logic to all registered Prefix hooks.
    /// </summary>
    public static bool PrefixForwarder(MethodBase __originalMethod, object[] __args, object __instance)
    {
        if (_registry.TryGetValue(__originalMethod, out var hooks))
        {
            foreach (var hook in hooks)
            {
                if (!hook.InvokePrefix(__args, __instance)) return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Injected by Harmony after the original method executing. Routes logic to all registered Postfix hooks.
    /// </summary>
    public static void PostfixForwarder(MethodBase __originalMethod, object[] __args, ref object __result, object __instance)
    {
        if (_registry.TryGetValue(__originalMethod, out var hooks))
        {
            foreach (var hook in hooks)
            {
                hook.InvokePostfix(__args, ref __result, __instance);
            }
        }
    }
}