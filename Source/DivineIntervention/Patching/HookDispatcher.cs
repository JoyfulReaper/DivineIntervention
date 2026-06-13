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

        /// <summary>
        /// Internal bridge method called by Harmony. Do not call directly.
        /// </summary>
        public static void PrefixForwarder(MethodBase __originalMethod, object[] __args, object __instance)
        {
            if (_registry.TryGetValue(__originalMethod, out var hooks))
            {
                // Create a copy or use a for-loop to prevent collection modified errors
                // if a hook unpatches itself while iterating
                for (int i = hooks.Count - 1; i >= 0; i--)
                {
                    hooks[i].InvokePrefix(__args, __instance);
                }
            }
        }
    }
}