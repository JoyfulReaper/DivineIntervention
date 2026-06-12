# Divine Intervention
Mr. God's RimWorld Modding Library

Make Mods the Right Way™
<sup><sub>(Not really a trademark)</sub></sup>

Welcome to Divine Intervention, an ultra-high-performance, developer-friendly C# framework for RimWorld. Designed to eliminate boilerplate, state-machine spaghetti, and memory leaks. This library provides a foundation of decoupled, performant, and heavily documented core utilities, allowing you to focus on building features rather than fighting the engine.

🛠 Core Utilities
Every module is built with strict adherence to RimWorld's performance constraints—keeping your frame rates high and your logs clean.

1. Beautiful Logging API
Stop cluttering the console with messy strings. The DivineLog API is cleanly formatted and provides developer-friendly color tagging.

C#
```
DivineLog.LoggingPrefix = "MiniMapPerformance";
DivineLog.UseColor = true;

DivineLog.Info("Mod loaded utilizing Divine Intervention Framework.");
DivineLog.Warning("Something unusual happened.");
DivineLog.Error("Critical failure in the patching engine!");
```
2. Zero-Allocation Message Bus
Decouple your mod components using a high-performance publish-subscribe pattern.

- Hot Path Optimization: The MessageBus separates the "Cold Path" (subscribing) from the "Hot Path" (publishing).
- GC-Safe: Broadcasting messages creates zero memory allocations, ensuring you never trigger garbage collection stutters during intense tick loops.

C#
```
MessageBus.Subscribe<TradeMessage>(msg => {
    DivineLog.Info($"Trade completed for {msg.SilverNet} silver!");
});

MessageBus.Publish(new TradeMessage { SilverNet = 500 });
```
3. Dynamic Patch Processor
Stop managing complex state machines to track Harmony patch status.

- Functional Reducer Pattern: Pass a simple Func<TContext, PatchCommand> delegate. The processor acts as a pure reducer, evaluating your environmental context and automatically handling the Patch/Unpatch lifecycle.
- Deduplication: Automatically ignores redundant instructions, ensuring your engine hooks are only mutated when a state boundary is actually crossed.

C#
```
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
🗺️ Roadmap & Future Vision
Divine Intervention is a living project designed to standardize the most common "pain points" in RimWorld development:
- Generic Replacement Pipeline: A modularized, standardized version of the Replace Stuff logic to enable seamless item, building, and entity swapping across the game.
- Inter-Mod Communication: Expanding the MessageBus to allow cross-assembly event routing with minimal performance overhead.
- Extended Patching Utilities: A suite of high-level helpers to make interacting with other third-party mods safer, faster, and less reliant on volatile reflection.

Copyright (c) 2026 Kyle Givler. Licensed under the MIT License.
