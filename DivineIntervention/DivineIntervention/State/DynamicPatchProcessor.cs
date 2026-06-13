/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

using HarmonyLib;
using System;
using System.Reflection;

namespace DivineIntervention.Patching;

/// <summary>
/// Specifies the execution mutation mutation instructions dispatched by the patching engine.
/// </summary>
public enum PatchCommand
{
    /// <summary>Injects or maintains the active state of the Harmony method patch.</summary>
    Enable,
    /// <summary>Removes or maintains the detached state of the Harmony method patch.</summary>
    Disable,
    /// <summary>Indicates that no change should occur to the current patch configuration state.</summary>
    NoOp
}

/// <summary>
/// A highly optimized generic patch orchestration lifecycle controller.
/// Evaluates structural environmental contexts via user-defined business rule logic delegates (reducers) 
/// and dynamically hooks or detaches third-party Harmony patches at runtime while eliminating duplicate execution calls.
/// </summary>
/// <typeparam name="TContext">The structured state data model container containing relevant game context data.</typeparam>
public class DynamicPatchProcessor<TContext>
{
    private readonly Harmony _harmony;
    private readonly MethodInfo _targetMethod;
    private readonly MethodInfo _prefixMethod;
    private readonly Func<TContext, PatchCommand> _reducer;

    private PatchCommand _lastCommand = PatchCommand.NoOp;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicPatchProcessor{TContext}"/> class.
    /// </summary>
    /// <param name="harmony">The central Harmony metadata identifier instance assigned to the calling mod.</param>
    /// <param name="target">The targeted original engine method reflection metadata definition to be patched.</param>
    /// <param name="prefix">The local patch method definition to hook dynamically into execution paths.</param>
    /// <param name="reducer">A pure conditional mapping function delegate designed to evaluate frame state context data profiles and yield processing commands.</param>
    public DynamicPatchProcessor(
        Harmony harmony,
        MethodInfo target,
        MethodInfo prefix,
        Func<TContext, PatchCommand> reducer)
    {
        _harmony = harmony;
        _targetMethod = target;
        _prefixMethod = prefix;
        _reducer = reducer;
    }

    /// <summary>
    /// Evaluates a transient state snapshot frame packet, processes lifecycle conditions, and dynamically mutates engine hooks.
    /// </summary>
    /// <param name="context">The transient structural context snapshot frame instance to consume and evaluate.</param>
    /// <remarks>
    /// This represents a high-frequency execution method designed for the frame loop layer. 
    /// Internal caching strategies deduplicate commands to ensure expensive <see cref="Harmony.Patch"/> and 
    /// <see cref="Harmony.Unpatch(MethodBase, MethodInfo)"/> calls only trigger upon direct mutation edge state boundaries.
    /// </remarks>
    public void Update(TContext context)
    {
        //Evaluate pure logic via the passed delegate
        PatchCommand cmd = _reducer(context);

        // Performance Filter / Deduplication
        if (cmd == PatchCommand.NoOp || cmd == _lastCommand)
            return;

        _lastCommand = cmd;

        // Dispatch execution mutations
        switch (cmd)
        {
            case PatchCommand.Enable:
                _harmony.Patch(_targetMethod, prefix: new HarmonyMethod(_prefixMethod));
                break;

            case PatchCommand.Disable:
                _harmony.Unpatch(_targetMethod, _prefixMethod);
                break;
        }
    }
}