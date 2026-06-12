/*
 * Divine Intervention RimWorld Modding Framework
 * 
 * Make Mods the Right Way(tm)
 * 
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

/*
 * [FEATURE EXAMPLE] The Loose Message Bus Lane
 * 
 * CRITICAL: The Runtime Casting "Gotcha"
 * Because the Loose Lane utilizes the generic 'object' type for maximum 
 * cross-mod flexibility, compile-time type safety is completely bypassed.
 * 
 * - Subscriber Responsibility: The consuming mod is entirely responsible for 
 *   explicitly casting the payload back into its expected concrete type.
 * - Silent/Runtime Risk: If Mod A publishes a 'string' but Mod B attempts to 
 *   cast it to an 'int', the runtime engine will throw an InvalidCastException.
 * 
 * PERFORMANCE & SAFETY TIP: 
 * If you control both the publishing and subscribing mods, skip this lane entirely. 
 * Use the Typed Lane (MessageBus.Subscribe<T>) instead. It completely eliminates 
 * casting errors and prevents CPU-heavy "boxing" (wrapping value types like 'int' 
 * into heap-allocated 'object' containers) on the hot path.
 */

// ==========================================
//  SUBSCRIBER EXAMPLE (Mod B - The Receiver)
// ==========================================

// 1. Define the callback method or store a reference to the action so we can unsubscribe later!
//using DivineIntervention.Events;
//using System;

//Action<object> onSilverChanged = (data) =>
//{
//    // Explicit unboxing/casting is mandatory here
//    int silver = (int)data;
//    DivineLog.Info($"Mod A reports a net balance of {silver} silver.");
//};

// 2. Listen for updates on the shared "ModA_SilverCount" channel
// MessageBus.Subscribe("ModA_SilverCount", onSilverChanged);

// 3. CLEANUP: Unsubscribe when the component, UI panel, or Map is destroyed to avoid memory leaks
// MessageBus.Unsubscribe("ModA_SilverCount", onSilverChanged);


// ==========================================
//  PUBLISHER EXAMPLE (Mod A - The Sender)
// ==========================================

// Broadcast the state update to any listening assemblies
//int currentSilver = 500;
//MessageBus.Publish("ModA_SilverCount", currentSilver);