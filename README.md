# Divine Intervention

**The Architecture of Sovereignty for RimWorld Modding**
*Make Mods the Right Way™*

> *"In the world of RimWorld modding, Harmony acts as the laws of physics. Divine Intervention is the power to rewrite them. Stop 'patching' and start 'intervening.'"*

---

**PLEASE NOTE: This framework is currently in an early and untested state.**
*Coming "soon" to the Steam Workshop. Any current versions posted on the workshop are unofficial builds.*

---

Welcome to **Divine Intervention**, an ultra-high-performance, developer-friendly C# framework for RimWorld. Designed to eliminate boilerplate, state-machine spaghetti, and memory leaks. This library provides a foundation of decoupled, performant, and heavily documented core utilities, allowing you to focus on building features rather than fighting the engine.

## 🛠 The Divine Instruments (Core Utilities)

Every module is built with strict adherence to RimWorld's performance constraints—keeping your frame rates high and your logs pristine. Internal framework logic is strictly encapsulated, ensuring your public API surface remains stable across updates.

### 1. The Patching Engine (`HookFactory`)

Stop leaving dangling global state patches in your assembly. The `HookFactory` wraps standard Harmony logic into a dynamic, lifecycle-managed engine. It supports both instance methods and static utility classes cleanly.

* **Dynamic Mounting:** Bind and unbind your hooks exactly when you need them (e.g., when a UI panel opens/closes) to save CPU cycles.
* **Universal Coverage:** Explicit type overloads allow you to safely intercept generic instances or core static game classes without fighting the compiler.

```csharp
// Intercepting a static game class dynamically
private static IHook _factionBalanceHook;

public static void Initialize()
{
    _factionBalanceHook = HookFactory.Create(
        typeof(FactionGiftUtility),
        "GiveGiftResult",
        onPrefix: null,
        onPostfix: (object instance, object[] args, ref object result) =>
        {
            DivineLog.Debug("Faction gift calculated. Intervening...");
            // Modify outcome or broadcast data here
        }
    );
}

public static void Shutdown() => _factionBalanceHook?.Unpatch();

```

### 2. The Omniscient Message Bus

Decouple your mod components using a high-performance publish-subscribe pattern. By packaging this as a shared "Core" dependency, your mods can communicate across different assemblies (DLLs) using the exact same memory space.

* **Typed Lane (High-Performance):** Compile-time safe, zero-allocation broadcasting for frequently triggered game events. Perfect for internal mod architecture.
* **Loose Lane (Cross-Mod):** String-based event routing. Mod A can talk to Mod B without either author ever seeing each other's source code or sharing a compiled type.
* **Mutation-Safe:** The bus utilizes a reverse-iteration pattern, allowing subscribers to safely `Unsubscribe` from within their own callback logic without throwing collection modified exceptions.

```csharp
// --- Typed Lane (Best for performance & internal routing) ---
MessageBus.Subscribe<TradeCompletedMessage>(msg => {
    DivineLog.Info($"Trade completed! Net silver: {msg.SilverExchanged}");
});
MessageBus.Publish(new TradeCompletedMessage { SilverExchanged = 1250 });

```

```csharp
// --- Loose Lane (Best for third-party cross-mod events) ---
MessageBus.Subscribe("ModA_SilverCount", (payload) => {
    if (payload is int silverCount) { // Safe pattern-matching cast
        DivineLog.Info($"Mod A reported a balance of {silverCount}");
    }
});
MessageBus.Publish("ModA_SilverCount", 500);

```

### 3. Self-Healing Diagnostic Logging

Stop cluttering the console with messy strings and `#if DEBUG` blocks. The `DivineLog` API is cleanly formatted, visually distinct, and automatically strips itself from production builds.

* **Zero-Overhead Debugging:** `DivineLog.Debug()` uses compiler-level attributes to completely vanish from release builds, saving CPU cycles without requiring manual comment-outs.
* **The Power User State Engine:** Shift console colors dynamically using an `IDisposable` struct. The engine guarantees a return to your baseline color, even if an exception occurs mid-execution.

```csharp
// Initialize once at startup
DivineLog.LoggingPrefix = "MyAwesomeMod";
DivineLog.LoggingColor = "#66CCFF"; 

DivineLog.Info("Core engine services connected smoothly."); // Electric Ice Blue

```

```csharp
using (new DivineLog.ColorScope("#FFDD55")) // Context shift to Neon Amber
{
    DivineLog.Info("Processing sensitive XML payload files...");
    DivineLog.Warning("Optional configuration node missing.");
} // Automatically snaps back to baseline blue here!

```

---

## 🏗 The Incarnation: Quick Start Guide

To enable cross-assembly communication, you must utilize the **Divine Intervention Core** as a shared dependency.

1. **Subscribe:** Ensure your mod depends on the Divine Intervention Core mod via the Steam Workshop *(No official Workshop page yet)*.
2. **Reference:** Add a reference to `DivineIntervention.dll` in your C# project.
3. **Set Copy Local to False:** In your IDE's reference properties, set **Copy Local** to `False`. This ensures your mod uses the Core mod's shared instance in memory.
4. **Add Dependency:** Update your `About.xml` to include the library to ensure it loads before your mod executes:

```xml
<modDependencies>
    <li>
        <packageId>DivineIntervention.Core</packageId>
        <displayName>Divine Intervention Core</displayName>
    </li>
</modDependencies>

```

**The Secret Sauce:** By referencing the *same* `DivineIntervention.dll` without embedding it, every mod you write points to the exact same place in the game's RAM.

---

## 🗺️ Roadmap & Future Vision

* **Generic Replacement Pipeline:** A modularized, standardized logic engine to enable seamless item, building, and entity swapping across the game.
* **State & Lifecycle Management:** Tools to easily track map loads, ticks, and environmental shifts without manual boilerplate.

---

*Copyright (c) 2026 Kyle Givler. Licensed under the MIT License.*
