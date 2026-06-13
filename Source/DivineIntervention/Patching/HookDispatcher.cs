/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

using System.Collections.Generic;
using System.Reflection;

namespace DivineIntervention.Patching
{
    /// <summary>
    /// Internal dispatcher that routes game method calls to registered hooks.
    /// </summary>
    public static class HookDispatcher
    {
        private static readonly Dictionary<MethodBase, List<IHook>> _registry = new();

        internal static void Register(MethodBase method, IHook hook)
        {
            if (!_registry.ContainsKey(method)) _registry[method] = new();
            _registry[method].Add(hook);
        }

        /// <summary>
        /// Internal dispatcher that routes game method calls to registered hooks.
        /// </summary>
        public static void Unregister(MethodBase method, IHook hook)
        {
            if (_registry.TryGetValue(method, out var hooks))
            {
                hooks.Remove(hook);
                // TODO: You could add a check here to log if the list is empty
            }
        }

        // Prefix forwarder (Logic to stop/continue)
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

        // Postfix forwarder (Logic to observe/modify results)
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
}