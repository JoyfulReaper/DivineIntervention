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
* * * Divine Intervention acts as a Middleware Layer for RimWorld. 
* * 1. DECOUPLING: You no longer need to write monolithic HarmonyPatch classes. 
* Create hooks where your logic lives, not where the game code lives.
* * 2. PERFORMANCE: Standard Harmony forces you to patch regardless of game state. 
* Our framework allows 'Conditional Hooks' that execute only when needed, 
* drastically reducing CPU overhead.
* * 3. LIFECYCLE MANAGEMENT: Harmony is "patch and forget." Divine Intervention 
* gives you an 'IHook' interface, allowing you to Unpatch and cleanup 
* dynamically when your mod features are disabled or maps are unloaded.
* * * HOW TO USE IT:
* --------------
* 1. OBSERVE: Use HookFactory.Create<T> to attach a simple observer.
* 2. CONDITION: Use the 'condition' lambda to save CPU cycles.
* 3. MANAGE: Store the IHook reference to Unpatch() at runtime.
* * * Make Mods the Right Way(tm)
* ======================================================================================
*/

using DivineIntervention.Logging;

namespace DivineIntervention;

/// <summary>
/// The primary entry point for the Divine Intervention Framework.
/// Provides global access to the framework's capabilities.
/// </summary>
public static class DivineInterventionLib
{
    public const string VERSION = "0.0.1";

    public static void Initialize()
    {
        DivineLog.Info($"Framework Initialized Version {VERSION}");
    }
}