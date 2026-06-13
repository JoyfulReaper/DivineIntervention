/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

namespace DivineIntervention.Patching
{
    /// <summary>
    /// Represents a dynamic hook that intercepts game methods.
    /// </summary>
    public interface IHook
    {
        /// <summary>
        /// Logic to execute when the intercepted method is called.
        /// </summary>
        /// <param name="args">The arguments passed to the original method.</param>
        /// <param name="instance">The object instance (if non-static).</param>
        bool InvokePrefix(object[] args, object instance);

        // Postfix: Accepts the result by ref so you can modify it
        void InvokePostfix(object[] args, ref object result, object instance);

        /// <summary>
        /// Removes this hook from the dispatcher registry.
        /// </summary>
        void Unpatch();
    }
}