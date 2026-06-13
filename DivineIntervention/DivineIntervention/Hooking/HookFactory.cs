/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* Copyright (c) 2026 Kyle Givler
* 
* Licensed under the MIT License.
*/

using DivineIntervention.Hooking.Internal;
using DivineIntervention.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DivineIntervention.Hooking;

/// <summary>
/// Primary API entry point for creating and managing dynamic method hooks.
/// </summary>
public static class HookFactory
{
    private static readonly HashSet<MethodBase> _patchedMethods = new HashSet<MethodBase>();
    private static IHarmonyEngine _harmonyEngine;


    /// <summary>
    /// Lazy property ensures HarmonyEngine is only created when requested in production.
    /// When tests assign a Mock engine via the setter, the real HarmonyEngine is never instantiated.
    /// </summary>
    public static IHarmonyEngine HarmonyEngine
    {
        get => _harmonyEngine ??= new HarmonyEngine("com.kgivler.divineIntervention");
        set => _harmonyEngine = value;
    }

    /// <summary>
    /// Initializes the factory and subscribes to dispatcher cleanup events.
    /// </summary>
    static HookFactory()
    {
        HookDispatcher.OnMethodEmpty += (method) =>
        {
            HarmonyEngine.Unpatch(method, HookPatchType.All);
            _patchedMethods.Remove(method);
        };
    }

    #region Generic Overloads (For Instance Classes)

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
        var targetMethod = typeof(T).GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        if (targetMethod == null)
        {
            DivineLog.Error($"[HookFactory] Could not find method {methodName} on type {typeof(T).Name}");
            return null;
        }

        var hook = new GenericHook<T>(targetMethod, onPrefix, onPostfix, condition);
        HookDispatcher.Register(targetMethod, hook);

        if (_patchedMethods.Add(targetMethod))
        {
            HarmonyEngine.Patch(targetMethod,
                prefix: new HookMethodInfo(typeof(HookDispatcher).GetMethod(nameof(HookDispatcher.PrefixForwarder))),
                postfix: new HookMethodInfo(typeof(HookDispatcher).GetMethod(nameof(HookDispatcher.PostfixForwarder)))
            );
        }

        return hook;
    }

    #endregion

    #region Explicit Type Overloads (For Static Classes)

    /// <summary>
    /// Creates a Power User hook using explicit Type structures. Necessary for intercepting static classes.
    /// </summary>
    public static IHook Create(Type targetType, string methodName, HookPrefix<object> onPrefix = null, HookPostfix<object> onPostfix = null, Func<bool> condition = null)
    {
        var targetMethod = targetType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        if (targetMethod == null)
        {
            DivineLog.Error($"[HookFactory] Could not find method {methodName} on type {targetType.Name}");
            return null;
        }

        var hook = new GenericHook<object>(targetMethod, onPrefix, onPostfix, condition);
        HookDispatcher.Register(targetMethod, hook);

        if (_patchedMethods.Add(targetMethod))
        {
            HarmonyEngine.Patch(targetMethod,
                prefix: new HookMethodInfo(typeof(HookDispatcher).GetMethod(nameof(HookDispatcher.PrefixForwarder))),
                postfix: new HookMethodInfo(typeof(HookDispatcher).GetMethod(nameof(HookDispatcher.PostfixForwarder)))
            );
        }

        return hook;
    }
}
    #endregion