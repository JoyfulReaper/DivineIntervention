/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

namespace DivineIntervention.Hooking;

/// <summary>
/// Represents a dynamic, unpatchable hook that intercepts game methods.
/// </summary>
public interface IHook
{
    /// <summary>
    /// Logic to execute before the intercepted method is called. 
    /// </summary>
    /// <param name="args">The mutable arguments passed to the original method. Changing an item in this array changes it for the game.</param>
    /// <param name="instance">The object instance (null if the method is static).</param>
    /// <returns><c>true</c> to allow the original method to execute; <c>false</c> to skip it.</returns>
    bool InvokePrefix(object[] args, object instance);

    /// <summary>
    /// Logic to execute after the intercepted method has completed.
    /// </summary>
    /// <param name="args">The arguments passed to the original method.</param>
    /// <param name="result">The mutable result of the method. Modify this by reference to change the game's final calculation.</param>
    /// <param name="instance">The object instance (null if the method is static).</param>
    void InvokePostfix(object[] args, ref object result, object instance);

    /// <summary>
    /// Removes this hook from the dispatcher registry and cleans up Harmony patches if no hooks remain.
    /// </summary>
    void Unpatch();
}