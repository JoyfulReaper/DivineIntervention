# Divine Intervention

**The Architecture of Sovereignty for RimWorld Modding** *Make Mods the Right Way™ (Because Randy Knows You Haven't Been)*

> ⚠️ IMPORTANT: This framework is brand new and is largely untested. THERE IS CURRENTLY NO OFFICIAL BUILD. If you find a build of this on the Steam Workshop or elsewhere, it is a rogue clone so treat it with the same suspicion as a crashlanded transport pod full of toxic waste packs.
>
> ⚠️ IMPORTANT: Currently all naming including the namespaces and namespace hierarchy, class names, PackageIDs and more are subject to change before the first official release. Don't get attached.

---

Welcome to **Divine Intervention**, an ultra-high-performance, developer-friendly C# framework for RimWorld. Designed to eliminate boilerplate, state-machine spaghetti, and memory leaks. This library provides a foundation of decoupled, performant, and heavily documented core utilities, allowing you to focus on building features rather than fighting the engine (and the urge to harvest your pawns' organs). 

The core framework code is highly commented for much faster understand and we also provide complete examples in the Examples folder.

---

## 🛠️ Core Features

Divine Intervention's primary goal is to abstract the volatile nature of static Harmony patches and manual console logging, giving you a clean, "God-tier" API for managing your mod's lifecycle before your codebase suffers a *Mental Break: Sad Wander*.

### 1. Production-Safe Contextual Logging (`DivineLog`)

Console spam is worse than a Psychic Drone (Extreme). Stop cluttering your console with messy strings and `#if DEBUG` blocks. The `DivineLog` API is cleanly formatted, visually distinct, and automatically handles scope cleanup.

* **Zero-Overhead Diagnostics:** `DivineLog.Debug()` utilizes the `[Conditional("DEBUG")]` attribute to ensure debug traces are completely stripped out of your compiled assembly during production builds. They vanish faster than a stack of components left unroofed.
* **Fail-Safe State Engine (Color Scopes):** Shift console colors dynamically using an `IDisposable` structure. The engine guarantees a safe return to your baseline mod color even if a critical exception crashes the execution thread midway. No more accidentally turning everyone else's logs Neon Amber forever.

```csharp
// 1. Initialization (Set these properties at your mod's entry point)
DivineLog.LoggingPrefix = "MyAwesomeMod";
DivineLog.UseColor = true;
DivineLog.LoggingColor = "#66CCFF"; // Electric Ice Blue default

// 2. Standard Logging Channels
DivineLog.Debug("Subsystem memory allocation mapping initialized."); // Vanishes in Release builds!
DivineLog.Info("Core engine services connected smoothly.");
DivineLog.Warning("Optional configuration node missing. Reverting to factory defaults.");
DivineLog.Error("Critical structural exception! Local database table is unreachable.");

// 3. Context Shifting via ColorScope
DivineLog.Info("This line renders in the default Electric Ice Blue.");

using (new DivineLog.ColorScope("#FFDD55")) // Neon Amber
{
    DivineLog.Info("Context changed: Processing sensitive XML payload files...");
    DivineLog.Debug("This inner debug line is also styled in Neon Amber!");

    // Nesting works out of the box; ColorScope preserves the stack state cleanly
    using (new DivineLog.ColorScope("#FF5555")) // Hot Red Override
    {
        DivineLog.Info("Nested Emergency: Network timeout detected. Retrying handshake...");
    }

    DivineLog.Info("Returned cleanly back to the Neon Amber context.");
}

// Zero leakage into subsequent logs or third-party assemblies
DivineLog.Info("Back to baseline default color.");

```

---

### 2. The Managed Patching Engine (`HookFactory`)

Stop leaving dangling global state patches in your assembly. It's the modding equivalent of leaving an antigrain warhead in a wooden stockpile with a Pyromaniac. The `HookFactory` wraps standard Harmony logic into a dynamic, lifecycle-managed engine.

Every hook created returns an `IHook` handle, tracking its own state and allowing execution branches to switch on or off safely at runtime.

#### Complete Hooking Feature Set & Patterns

```csharp
// PATTERN 1: The Observer
// Best for: Logging, tracking stats, or non-intrusive runtime monitoring.
HookFactory.Create<Pawn>(
    "Tick",
    (pawn) => DivineLog.Debug($"Observer: {pawn.LabelShort} is ticking!")
);

// PATTERN 2: The Conditional Guard
// Best for: Performance-critical hooks. Saves valuable CPU cycles by evaluating 
// a condition predicate BEFORE running any patching logic.
HookFactory.Create<Pawn>(
    "Tick",
    (pawn) => DivineLog.Debug($"Conditional: {pawn.LabelShort} is working."),
    condition: () => !Find.TickManager.Paused
);

// PATTERN 3: The Dynamic Lifecycle
// Best for: Toggling features dynamically based on user settings, UI buttons, or map switches.
private IHook _myDynamicHook;

public void EnableFeature()
{
    _myDynamicHook = HookFactory.Create<Pawn>(
        "Tick",
        (pawn) => DivineLog.Debug("Dynamic: Feature active.")
    );
}

public void DisableFeature()
{
    _myDynamicHook?.Unpatch(); // Cleans up hooks instantly with zero dangling reference overhead
    _myDynamicHook = null;
}

// PATTERN 4: The Hijacker (Return Value Mutation)
// Best for: Stat rebalancing, forced state overrides, or forcing debug mock results.
HookFactory.Create<Pawn>(
    "get_HealthScale",
    onPostfix: (Pawn instance, object[] args, ref object result) =>
    {
        result = 100f; // Intercepts and forces the final returned health scale float
    }
);

// PATTERN 5: The Argument Mutator
// Best for: Intercepting inputs to alter damage calculations, item costs, or AI values.
HookFactory.Create<Pawn>(
    "TakeDamage",
    onPrefix: (instance, args) =>
    {
        if (args[0] is DamageInfo dinfo)
        {
            DivineLog.Debug($"Pawn {instance.LabelShort} is taking {dinfo.Amount} damage!");
            // You can mutate elements inside the args array here before returning true!
        }
        return true; // Return true to let the original method proceed with arguments
    }
);

// PATTERN 6: The Method Interceptor (Full Execution Override)
// Best for: Completely bypassing broken core game logic or introducing entirely custom AI routines.
HookFactory.Create<Pawn>(
    "Tick",
    onPrefix: (instance, args) =>
    {
        // Returning 'false' entirely halts the execution of the original game code
        return false; 
    }
);

// PATTERN 7: Explicit Type Overloads
// Best for: Targeting static classes, internal methods, or third-party code where generic constraints don't fit.
IHook staticHook = HookFactory.Create(
    typeof(FactionGiftUtility),
    "GiveGiftResult",
    onPrefix: null,
    onPostfix: (object instance, object[] args, ref object result) => 
    {
        // Custom static patch logic goes here
    }
);

```

---

### 3. State-Driven Patch Processing (`DynamicPatchProcessor`)

For when your TPS (Ticks Per Second) is dropping faster than the colony's mood during a toxic fallout. Instead of hammering expensive `Harmony.Patch()` and `Harmony.Unpatch()` operations inside active update tick loops, the `DynamicPatchProcessor` caches instructions and executes mutations exclusively when state boundary changes are crossed.

#### Real-World Implementation: Third-Party Mod Minimap Optimizer

```csharp
public struct MiniMapContext
{
    public bool IsOpen;
    public bool MapChanged;
    public bool JustLoaded;
}

public static class DubsPatchController
{
    private static DynamicPatchProcessor<MiniMapContext> _optimizer;

    public static void Initialize(Harmony harmony)
    {
        var tryUpdate = AccessTools.Method(typeof(Section), "TryUpdate");
        var dubsPrefix = AccessTools.Method("DubsMintMinimap.Harmony_TryUpdate:Prefix");

        _optimizer = new DynamicPatchProcessor<MiniMapContext>(
            harmony,
            tryUpdate,
            dubsPrefix,
            context =>
            {
                // The Pure Functional Reduction Layer: Returns desired layout states safely
                if (context.MapChanged || context.JustLoaded)
                    return PatchCommand.Enable;

                return context.IsOpen
                    ? PatchCommand.Enable
                    : PatchCommand.Disable;
            }
        );
    }

    public static void LogStateUpdate(MiniMapContext currentContext)
    {
        // Internal caching mechanisms evaluate commands automatically. 
        // If the map state hasn't visually transitioned, this call exits immediately as a NoOp.
        _optimizer.Update(currentContext);
    }
}

```

---

### 4. The Omniscient Message Bus

Keep your mod components more separated than a jealous pawn and their rival. Decouple your systems entirely using a high-performance publish-subscribe pattern. By shifting dependencies out of hard class links, you eliminate cross-mod load dependency loops.

#### Lane A: The Typed Lane (Internal & Performance Critical)

The Typed Lane is compile-time safe and handles data casting natively without boxing value types. This is the optimal route for broadcasting rapidly recurring gameplay events across systems inside your mod, moving faster than a deathcore blast beat.

```csharp
// 1. Define an immutable, behavior-less data payload structure
public struct TradeCompletedMessage
{
    public string TraderName;
    public int SilverExchanged;
    public int TicksGame;
}

// 2. Publisher Component: Broadcasts data packages across the bus
public static class TradeNotifier
{
    public static void OnTradeExecuted(string name, int silver)
    {
        var payload = new TradeCompletedMessage
        {
            TraderName = name,
            SilverExchanged = silver,
            TicksGame = Find.TickManager.TicksGame
        };

        // Fire and forget cleanly
        MessageBus.Publish(payload);
    }
}

// 3. Subscriber Component: Listens and logs across save files safely
public class EconomyTracker : GameComponent
{
    public int TotalSilverTraded = 0;

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        // Bind event processing delegate directly to the message type signature
        MessageBus.Subscribe<TradeCompletedMessage>(RecordTrade);
    }

    private void RecordTrade(TradeCompletedMessage msg)
    {
        TotalSilverTraded += Math.Abs(msg.SilverExchanged);
        DivineLog.Debug($"Lifetime economy footprint updated: {TotalSilverTraded} silver.");
    }

    public void Teardown()
    {
        // Always unsubscribe to prevent memory address retention leaks
        MessageBus.Unsubscribe<TradeCompletedMessage>(RecordTrade);
    }
}

```

#### Lane B: The Loose Lane (Cross-Mod Extension Network)

The Loose Lane operates via raw magic-string channel identifiers. This allows Mod A to communicate with Mod B even if neither developer has access to the other’s source code. It's like trading with a bulk goods orbital ship using walkie-talkies.

```csharp
// ==========================================
// PUBLISHER CONTEXT (Mod A - Independent Project)
// ==========================================
int silverValue = 500;
// Dispatches an standard 'object' package out over the global string wire
MessageBus.Publish("ModA_FactionSilverCount", silverValue);

// ==========================================
// SUBSCRIBER CONTEXT (Mod B - External Addon)
// ==========================================
MessageBus.Subscribe("ModA_FactionSilverCount", (payload) =>
{
    // Mandatory runtime validation check & unboxing pattern matching
    if (payload is int silverCount)
    {
        DivineLog.Info($"Cross-Mod Captured! Mod A reported balance: {silverCount}");
    }
});

```

---

## 🏗️ Quick Start Integration

To enable global inter-mod communication via the shared memory network, you must utilize the **Divine Intervention Core** assembly deployment structure. Do not skip these steps, or your mod will throw more red errors than a manhunting boomalope pack.

1. **Subscribe:** Add a dependency reference to the **Divine Intervention Core** mod framework inside your mod distribution profile.
2. **Reference:** Link `DivineIntervention.dll` into your local IDE project solution workspace dependencies.
3. **Set Copy Local to False:** Inside your project assembly reference compilation properties settings panel, explicitly set **Copy Local** (`Private` metadata flag) to `False`.

> **CRITICAL:** Skipping this step forces your compilation build setup to package local instances. This breaks isolated system memory linking, drops global messaging operations, and practically guarantees Randy will drop a meteorite on your colony's hospital.

4. **Add Dependency Tag:** Edit your mod's `About/About.xml` file definition schema layout directory tree to enforce loading sequences explicitly:

```xml
<modDependencies>
    <li>
        <packageId>DivineIntervention.Core</packageId>
        <displayName>Divine Intervention Core</displayName>
    </li>
</modDependencies>

```

---

*Copyright (c) 2026 Kyle Givler. Licensed under the MIT License (And definitely not made of human leather).*
