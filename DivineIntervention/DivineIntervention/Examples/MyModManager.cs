/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

#if DEBUG
using DivineIntervention.Hooking;
using DivineIntervention.Logging;
using Verse;

namespace DivineIntervention.Examples
{
    /// <summary>
    /// A professional reference implementation for managing mod features 
    /// using the Divine Intervention Framework.
    /// </summary>
    public class MyModManager
    {
        private IHook _tickHook;
        private IHook _damageHook;

        /// <summary>
        /// Registers a simple observer and an advanced data mutator.
        /// Demonstrates how to manage multiple hooks within a single manager class.
        /// </summary>
        public void EnableFeature()
        {
            // Basic Observer: Only runs when the game is not paused (Performance Optimization)
            _tickHook = HookFactory.Create<Pawn>(
                "Tick",
                (pawn) => DivineLog.Info($"Pawn {pawn.LabelShort} is being processed."),
                condition: () => !Find.TickManager.Paused
            );

            // Power User Mutator: Hijacks damage output
            // Demonstrates use of the 'ref result' to modify game calculations
            _damageHook = HookFactory.Create<Pawn>(
                "TakeDamage",
                onPostfix: (Pawn instance, object[] args, ref object result) =>
                {
                    // If we wanted to modify the damage, we could cast the result
                    // and apply our custom logic here.
                    DivineLog.Info($"Processed damage for {instance.LabelShort}.");
                }
            );
        }

        /// <summary>
        /// Safely unregisters all active hooks.
        /// The framework automatically handles the cleanup and unpatching.
        /// </summary>
        public void DisableFeature()
        {
            _tickHook?.Unpatch();
            _tickHook = null;

            _damageHook?.Unpatch();
            _damageHook = null;

            DivineLog.Info("All hooks removed. Zero overhead.");
        }
    }
}
#endif