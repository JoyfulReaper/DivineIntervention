# Divine Intervention

**Mr. God's RimWorld Modding Library**

*Make Mods the Right Way™*
(Not really a trademark)


Coming "soon" to the Steam Workshop. Any current versions posted on the workshop are unoffical builds.


---

Welcome to **Divine Intervention**, an ultra-high-performance, developer-friendly C# framework for RimWorld. Designed to eliminate boilerplate, state-machine spaghetti, and memory leaks. This library provides a foundation of decoupled, performant, and heavily documented core utilities, allowing you to focus on building features rather than fighting the engine.

---

## 🛠 Core Utilities

Every module is built with strict adherence to RimWorld's performance constraints—keeping your frame rates high and your logs clean.

### 1. Beautiful Logging API

Stop cluttering the console with messy strings. The `DivineLog` API is cleanly formatted, and provides developer-friendly color tagging.

```csharp
// Configure the shared framework logger for your mod
DivineLog.LoggingPrefix = "MiniMapPerformance";
DivineLog.UseColor = true;

// Clean, context-aware logging
DivineLog.Info("Mod loaded utilizing Divine Intervention Framework.");
DivineLog.Warning("Something unusual happened.");
DivineLog.Error("Critical failure in the patching engine!");

```

### 2. Zero-Allocation Message Bus

Decouple your mod components using a high-performance publish-subscribe pattern. By packaging this as a shared "Core" dependency, your mods can communicate across different assemblies (DLLs) using the same memory space.

* **Typed Lane (High-Performance):** Compile-time safe, zero-allocation broadcasting for frequently triggered game events.
* **Loose Lane (Dynamic):** String-based event routing for cross-mod communication without needing shared type contracts.
* **Safety First:** Both lanes feature robust exception handling with detailed diagnostic logging (including payload hash codes and subscriber context) to prevent one rogue subscriber from crashing the game.
* **Mutation-Safe:** The bus utilizes a reverse-iteration pattern, allowing subscribers to safely `Unsubscribe` from within their own callback logic.

```csharp
// --- Typed Lane (Best for performance) ---
MessageBus.Subscribe<TradeMessage>(msg => {
    DivineLog.Info($"Trade completed for {msg.SilverNet} silver!");
});
MessageBus.Publish(new TradeMessage { SilverNet = 500 });

// --- Loose Lane (Best for dynamic/cross-mod events) ---
MessageBus.Subscribe("ModA_InventoryChanged", (data) => {
    int count = (int)data; // Cast required
    DivineLog.Info($"Received inventory update: {count}");
});
MessageBus.Publish("ModA_InventoryChanged", 1500);

```

### 3. Dynamic Patch Processor

Stop managing complex state machines to track Harmony patch status.

* **Functional Reducer Pattern:** Pass a simple `Func<TContext, PatchCommand>` delegate. The processor acts as a pure reducer, evaluating your environmental context and automatically handling the `Patch`/`Unpatch` lifecycle.
* **Deduplication:** Automatically ignores redundant instructions, ensuring your engine hooks are only mutated when a state boundary is actually crossed.

```csharp
_optimizer = new DynamicPatchProcessor<MiniMapContext>(
    harmony,
    tryUpdate,
    prefix,
    context =>
    {
        if (context.MapChanged || context.JustLoaded)
            return PatchCommand.Enable;

        return context.IsOpen
            ? PatchCommand.Enable
            : PatchCommand.Disable;
    }
);

_optimizer.Update(currentContext);

```

## 🏗 Quick Start: Integrating with Divine Intervention

To enable cross-assembly communication (like using the `MessageBus` across different mods), you need to utilize the **Divine Intervention Core** as a shared dependency.

Follow these four steps:

1. **Subscribe:** Ensure your mod depends on the **Divine Intervention Core** (Name not finalzied) mod via the Steam Workshop (No offical Workshop page yet).
2. **Reference:** Add a reference to `DivineIntervention.dll` in your project. You will find this DLL inside your local Steam Workshop content folder for the Core mod.
3. **Set Copy Local to False:** In your project's reference properties, set **Copy Local** to `False`. This is critical—it ensures your mod uses the Core mod's shared instance in memory rather than creating a private (and broken) duplicate.
4. **Add Dependency:** Update your `About.xml` to include the library as a dependency to ensure it loads before your mod:

Note: packageId not yet finalized.
```xml
<modDependencies>
    <li>
        <packageId>DivineIntervention.Core</packageId>
        <displayName>Divine Intervention Core</displayName>
    </li>
</modDependencies>

```

---

**Why do this?**
By referencing the *same* `DivineIntervention.dll` without embedding it, every mod you write points to the exact same place in memory. This is the "secret sauce" that allows your `MessageBus` to relay data between totally different mods seamlessly.

---

## 🗺️ Roadmap & Future Vision

Divine Intervention is a living project designed to standardize the most common "pain points" in RimWorld development:

* **Generic Replacement Pipeline:** A modularized, standardized version of the *Replace Stuff* logic to enable seamless item, building, and entity swapping across the game.
* **Inter-Mod Communication:** Expanding the `MessageBus` to allow cross-assembly event routing with minimal performance overhead.
* **Extended Patching Utilities:** A suite of high-level helpers to make interacting with other third-party mods safer, faster, and less reliant on volatile reflection.

---

*Copyright (c) 2026 Kyle Givler. Licensed under the MIT License.*
