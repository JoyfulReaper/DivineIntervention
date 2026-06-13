/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* Copyright (c) 2026 Kyle Givler
* 
* Licensed under the MIT License.
*/

#if DEBUG
using DivineIntervention.Events;
using DivineIntervention.Hooking;
using DivineIntervention.Logging;
using RimWorld.Planet;
using System;

namespace DivineIntervention.Examples;

/*
 * ======================================================================================
 * [FEATURE EXAMPLE] THE LOOSE MESSAGE BUS LANE (Cross-Mod Communication)
 * ======================================================================================
 * * WHY USE THIS LANE?
 * ------------------
 * The standard Typed Lane (MessageBus.Subscribe<T>) requires both the publisher and 
 * subscriber mods to reference the exact same compiled assembly or type definition. 
 * The Loose Lane breaks this dependency by using arbitrary string identifiers ("Channels").
 * * This allows Mod A to communicate with Mod B even if neither mod author has ever 
 * seen each other's source code!
 * * * CRITICAL: The Runtime Casting "Gotcha"
 * ---------------------------------------
 * Because the Loose Lane utilizes the generic 'object' type for maximum flexibility:
 * 1. CASTING IS MANDATORY: The subscriber must manually cast or unbox the payload object.
 * 2. RUNTIME RISK: If Mod A publishes an 'int' but Mod B attempts to cast it to a 'float', 
 * the CLR runtime will throw an InvalidCastException at runtime.
 * * * PERFORMANCE TIP: 
 * If you control both the publishing and subscribing logic within your own mod systems, 
 * use the Typed Lane instead. It bypasses heap allocation "boxing" of value types entirely.
 * ======================================================================================
 */

/// <summary>
/// PUBLISHER CONTEXT (Mod A - The Sender)
/// This represents an isolated mod tracking a resource value like faction silver.
/// It uses HookFactory to intercept the game loop and broadcast values across the loose channel.
/// </summary>
public static class ModAPublisherHub
{
    private static IHook _factionBalanceHook;

    /// <summary>
    /// Instantiates a dynamic interceptor hook that publishes internal state updates globally.
    /// </summary>
    public static void Initialize()
    {
        // Intercepting a STATIC Faction core function using the explicit Type overload
        _factionBalanceHook = HookFactory.Create(
            typeof(FactionGiftUtility),
            "GiveGiftResult",
            onPrefix: null, // We only care about the postfix result here
            onPostfix: (object instance, object[] args, ref object result) =>
            {
                // For demonstration, let's grab a calculated silver balance modification value
                int currentSilverValue = 500;

                // Broadcast the value out over the string-keyed global channel. 
                // Any third-party mod listening to "ModA_SilverCount" will receive this value instantly.
                MessageBus.Publish("ModA_SilverCount", currentSilverValue);
                DivineLog.Debug($"[Mod A] Dispatched balance update of {currentSilverValue} silver over Loose Lane.");
            }
        );
    }

    /// <summary>
    /// Safely tears down the pipeline.
    /// </summary>
    public static void Shutdown()
    {
        _factionBalanceHook?.Unpatch();
        _factionBalanceHook = null;
    }
}

/// <summary>
/// SUBSCRIBER CONTEXT (Mod B - The Independent Receiver)
/// A completely distinct third-party mod listening for Mod A's updates without direct type dependencies.
/// </summary>
public static class ModBSubscriberHub
{
    // Explicitly maintain a reference to our action delegate so we can safely unregister it later!
    private static readonly Action<object> OnSilverChangedHandler = (payload) =>
    {
        try
        {
            // CRITICAL REQUIREMENT: Explicit unboxing/casting via pattern matching
            if (payload is int silverCount)
            {
                DivineLog.Info($"[Mod B] Cross-Mod Event Captured! Mod A reported a clean balance of {silverCount} silver.");
            }
            else
            {
                DivineLog.Warning($"[Mod B] Received unexpected data payload schema type on channel 'ModA_SilverCount'.");
            }
        }
        catch (Exception ex)
        {
            DivineLog.Error($"[Mod B] Execution crash processing third-party data frame: {ex.Message}");
        }
    };

    /// <summary>
    /// Binds the callback tracking listener structure directly to the string identifier channel name.
    /// </summary>
    public static void Initialize()
    {
        // Subscribe using the shared magic-string token key
        MessageBus.Subscribe("ModA_SilverCount", OnSilverChangedHandler);
        DivineLog.Debug("[Mod B] Registered loose listener tracking channel.");
    }

    /// <summary>
    /// CLEANUP: Crucial to execute during scene/map changes or when features disable 
    /// to entirely eliminate memory retention or leaking pointer addresses.
    /// </summary>
    public static void Shutdown()
    {
        MessageBus.Unsubscribe("ModA_SilverCount", OnSilverChangedHandler);
        DivineLog.Debug("[Mod B] Cleanly disconnected loose tracking channel.");
    }
}
#endif