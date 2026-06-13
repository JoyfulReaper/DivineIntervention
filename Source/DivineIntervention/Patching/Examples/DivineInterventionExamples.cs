/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

/*
* ======================================================================================
* DIVINE INTERVENTION: MODDING ARCHITECTURE
* ======================================================================================
* * WHY USE THIS FRAMEWORK?
* -----------------------
* Standard Harmony usage requires static classes and attributes which can lead to 
* "spaghetti patches"—hard to debug, difficult to manage, and prone to performance 
* issues when multiple mods collide on the same method.
* * Divine Intervention acts as a Middleware Layer for RimWorld. 
* * 1. DECUPPLING: You no longer need to write monolithic HarmonyPatch classes. 
* Create hooks where your logic lives, not where the game code lives.
* * 2. PERFORMANCE: Standard Harmony forces you to patch regardless of game state. 
* Our framework allows 'Conditional Hooks' that execute only when needed, 
* drastically reducing CPU overhead.
* * 3. LIFECYCLE MANAGEMENT: Harmony is "patch and forget." Divine Intervention 
* gives you an 'IHook' interface, allowing you to Unpatch and cleanup 
* dynamically when your mod features are disabled or maps are unloaded.
* * HOW TO USE IT:
* --------------
* 1. OBSERVE: Use HookFactory.Create<T> to attach a simple observer.
* 2. CONDITION: Use the 'condition' lambda to save CPU cycles.
* 3. MANAGE: Store the IHook reference to Unpatch() at runtime.
* * Make Mods the Right Way(tm)
* ======================================================================================
*/

using DivineIntervention.Logging;
using DivineIntervention.Patching;
using Verse;

namespace DivineIntervention.Examples
{
    public static class HookExamples
    {
        private static IHook _myDynamicHook;

        // Pattern 1: The Observer
        public static void RunObserverExample()
        {
            HookFactory.Create<Pawn>(
                "Tick",
                (pawn) => DivineLog.Debug($"Observer: {pawn.LabelShort} is ticking!")
            );
        }

        // Pattern 2: The Conditional Guard
        public static void RunConditionalExample()
        {
            HookFactory.Create<Pawn>(
                "Tick",
                (pawn) => DivineLog.Debug($"Conditional: {pawn.LabelShort} is working."),
                condition: () => !Find.TickManager.Paused
            );
        }

        // Pattern 3: The Dynamic Lifecycle
        public static void EnableFeature()
        {
            _myDynamicHook = HookFactory.Create<Pawn>(
                "Tick",
                (pawn) => DivineLog.Debug("Dynamic: Feature active.")
            );
        }

        public static void DisableFeature()
        {
            _myDynamicHook?.Unpatch();
            _myDynamicHook = null;
        }
    }
}