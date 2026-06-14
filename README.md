# Divine Intervention

**The Architecture of Sovereignty for RimWorld Modding** *Make Mods the Right Way™ (Because Randy Knows You Haven't Been)*

> ⚠️ IMPORTANT: This framework is brand new and is largely untested. THERE IS CURRENTLY NO OFFICIAL BUILD. If you find a build of this on the Steam Workshop or elsewhere, it is a rogue clone so treat it with the same suspicion as a crashlanded transport pod full of toxic waste packs.
> ⚠️ IMPORTANT: Currently all naming including the namespaces and namespace hierarchy, class names, PackageIDs and more are subject to change before the first official release. Don't get attached.

---

Welcome to **Divine Intervention**, a high-performance, developer-friendly C# framework for RimWorld. This library provides a foundation of decoupled, performant, and heavily documented core utilities, allowing you to focus on building features rather than fighting the engine (and the urge to harvest your pawns' organs).

* The core framework code is heavily commented for much faster understanding.
* Extensive, fully compiling example usage for all features can be found in the Examples folder.

---

## 🛠️ Core Features

### 1. Production-Safe Contextual Logging (`DivineLog`)

Console spam is worse than a Psychic Drone (Extreme). Stop cluttering your console with messy strings and manually managed `#if DEBUG` blocks. The `DivineLog` engine supports complete cross-mod isolation while maximizing runtime performance.

* **Zero-Overhead Diagnostics:** `DivineLog.Debug()` utilizes the `[Conditional("DEBUG")]` attribute to ensure debug traces are completely stripped out of your compiled assembly during production builds. They vanish faster than a stack of components left unroofed.
* **Pre-Computed Hot Paths:** Unlike traditional logging scripts that format strings on every game tick, updating the `Color` property on an instance recalculates your rich text prefix tags *exactly once* during assignment. Logging calls remain pure, lightning-fast pointer reads.
* **Architectural Flexibility:** Power users can choose the **DIY Route** to completely manage reference lifetimes explicitly, while developers looking for fast, global file access can use the **Managed Registry** to pool instances cleanly without allocations.

```csharp
// APPROACH 1: THE MANAGED REGISTRY (Zero Reference Passing)
// Fetch an isolated instance from any namespace, file, or thread instantly.
// Internal caching maps this so identical prefixes share a single allocation memory footprint.
var log = DivineLog.GetLogger("MyAwesomeMod", "#66CCFF", true);

log.Info("Core engine services connected smoothly."); // Default Electric Ice Blue

// APPROACH 2: THE DIY ROUTE (Explicit Lifecycle Ownership)
// Instantiate your logger once at your mod entry point and hold a static reference.
public static readonly DivineLog CustomLog = new DivineLog("MyDiyMod", "cyan");

// Standard Channels
CustomLog.Debug("Subsystem memory allocation mapping initialized."); // Stripped in Release!
CustomLog.Warning("Optional configuration node missing. Reverting to factory defaults.");
CustomLog.Error("Critical structural exception! Local database table is unreachable.");

// Dynamic Color Modification on Existing Instances
// Natively accepts hexadecimal code strings or literal color names parsed by Unity's markup engine.
CustomLog.Color = "#FFDD55"; // Shift to Neon Amber
CustomLog.Info("Context changed: Processing sensitive XML payload files...");

CustomLog.Color = "red"; // Switch to text-defined literal red
CustomLog.Info("Emergency context: Operation timed out.");

CustomLog.Color = "#66CCFF"; // Return safely to baseline profile with zero cross-mod configuration leak

```

---

### 2. The Omniscient Message Bus

Keep your mod components more separated than a jealous pawn and their rival. Decouple your systems entirely using a high-performance publish-subscribe pattern. By shifting dependencies out of hard class links, you eliminate cross-mod load dependency loops.

#### Lane A: The Typed Lane (Internal & Performance Critical)

The Typed Lane is compile-time safe and handles data casting natively without boxing value types. This is the optimal route for broadcasting rapidly recurring gameplay events across systems inside your mod, moving faster than a technical metal drum solo.

```csharp
namespace MyEconomyMod
{
    // A single, coherent module log definition used throughout our namespace
    internal static class ModLog
    {
        public static readonly DivineLog Instance = new DivineLog("MyEconomyMod", "#00FF88", true);
    }
}

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
        MyEconomyMod.ModLog.Instance.Debug($"Lifetime footprint updated: {TotalSilverTraded} silver.");
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
private static readonly DivineLog LogA = new DivineLog("ModA_Publisher", "cyan");

int silverValue = 500;
MessageBus.Publish("ModA_FactionSilverCount", silverValue);
LogA.Debug("Dispatched balance payload update across Loose Lane.");

// ==========================================
// SUBSCRIBER CONTEXT (Mod B - External Addon)
// ==========================================
private static readonly DivineLog LogB = new DivineLog("ModB_Subscriber", "orange");

public static void RegisterListener()
{
    MessageBus.Subscribe("ModA_FactionSilverCount", (payload) =>
    {
        // Mandatory runtime validation check & unboxing pattern matching
        if (payload is int silverCount)
        {
            LogB.Info($"Cross-Mod Captured! Mod A reported balance: {silverCount}");
        }
    });
}

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