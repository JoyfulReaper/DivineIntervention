/*
* Divine Intervention RimWorld Modding Framework
* * Make Mods the Right Way(tm)
* * Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/
namespace DivineIntervention.Patching
{
    /// <summary>
    /// Delegate for logic executed before a method runs.
    /// </summary>
    /// <typeparam name="T">The type defining the method.</typeparam>
    /// <param name="instance">The current instance of the object (default if static).</param>
    /// <param name="args">The method arguments array.</param>
    /// <returns>True to continue execution; false to halt the original method.</returns>
    public delegate bool HookPrefix<T>(T instance, object[] args);

    /// <summary>
    /// Delegate for logic executed after a method runs, allowing mutation of the return value.
    /// </summary>
    /// <typeparam name="T">The type defining the method.</typeparam>
    /// <param name="instance">The current instance of the object (default if static).</param>
    /// <param name="args">The method arguments array.</param>
    /// <param name="result">The current result of the method, passed by reference.</param>
    public delegate void HookPostfix<T>(T instance, object[] args, ref object result);
}