# [DI-CORE: SYSTEMS ARCHITECTURE MANUAL]

**PROTOCOL ID:** DI-FRAMEWORK-01  
**CODENAME:** Divine Intervention Core  
**SYSTEM STATUS:** UNSTABLE / PRE-RELEASE TESTING  
**MAINTAINER:** K. GIVLER (ADMIN)  

---

## 1.0 SYSTEM OVERVIEW

The **Divine Intervention Core (DI)** framework is a decoupled, high-performance runtime utility library engineered for the C# environment within the *RimWorld* simulation matrix. This framework provides an optimized, thread-safe, and extensively documented foundation layer designed to abstract core systems away from direct engine dependency, mitigating structural layout complications and stabilizing modular extensions.

The primary codebase features deep inline developer documentation. Complete, executable usage specifications are maintained in the `/Examples/` directory tree.

```text
================================================================================
                                ⚠️ SYSTEM ALERTS
================================================================================
[ALERT 01] PHASE ZERO DEVELOPMENT: This codebase is unverified and completely 
           untested. No official system binaries have been compiled or certified 
           for public deployment. Treat unverified distribution channels as 
           corrupted, volatile memory blocks.
[ALERT 02] MUTABLE IDENTIFIERS: All namespaces, class naming structures, and 
           PackageIDs are fluid and subject to structural mutation prior to 
           baseline version finalization. Do not develop long-term dependencies 
           on the current API layout.
================================================================================

```

---

## 2.0 INTEGRATED DIAGNOSTIC SUBSYSTEM (`DivineLog`)

The `DivineLog` system replaces legacy, high-allocation logging scripts that introduce serialization latency during active execution ticks. It implements strict cross-mod context isolation while eliminating output string processing bottlenecks.

### 2.1 Performance Characteristics

* **Zero-Overhead Compilation:** The `DivineLog.Debug()` pipeline is wrapped in the `[Conditional("DEBUG")]` preprocessor attribute. Diagnostic traces are completely excised from production binaries during release compilation cycles, minimizing assembly size and instruction overhead.
* **Static Execution Paths:** Traditional rich-text styling forces string manipulation routines on every execution pass. The `DivineLog` architecture calculates text prefix tags precisely once during the initialization or reassignment of the `Color` property, converting runtime log evaluations into pure pointer read operations.
* **Memory Allocation Matrix:** Developers may choose between two lifecycle strategies based on target resource budgets:

| Operational Strategy | Memory Footprint | Reference Management | Best Application |
| --- | --- | --- | --- |
| **Managed Registry** | Shared / Pooled Allocations | Internally cached via unique string prefixes | Rapid global file access with zero reference passing. |
| **DIY Configuration** | Isolated Static Instance | Explicit manual reference lifecycle tracking | Single-allocation lifecycle bounds linked to mod entry entry points. |

### 2.2 Subsystem Implementation Guide

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

CustomLog.Color = "#666CCFF"; // Return safely to baseline profile with zero cross-mod configuration leak

```

---

## 3.0 INTER-PROCESS COMMUNICATION NETWORK (THE MESSAGE BUS)

To prevent severe class-linking dependency loops and decoupling failures between discrete software modules, the framework utilizes an asynchronous publish-subscribe message bus architecture split into two targeted transport channels.

### 3.1 Lane A: The Strongly-Typed Pipeline

Designed for high-frequency internal data transit. This pipeline bypasses runtime object boxing for value types and remains compile-time safe. It is the designated path for continuous, rapid-fire gameplay event broadcasting.

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

### 3.2 Lane B: The Loose-String Integration Network

Designed for multi-assembly cross-mod bridging where source files cannot be shared. Communication routes are established via magic-string channel tokens.

```csharp
// =============================================================================
// PUBLISHER CONTEXT (Mod A - Independent Project)
// =============================================================================
private static readonly DivineLog LogA = new DivineLog("ModA_Publisher", "cyan");

int silverValue = 500;
MessageBus.Publish("ModA_FactionSilverCount", silverValue);
LogA.Debug("Dispatched balance payload update across Loose Lane.");

// =============================================================================
// SUBSCRIBER CONTEXT (Mod B - External Addon)
// =============================================================================
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

## 4.0 DEPLOYMENT & LINKING PROCEDURES

To establish valid data linking across the shared simulation memory plane, compiling assemblies must adhere strictly to the following linking configuration protocol.

1. **Dependency Assignment:** Register a hard dependency pointing to the `Divine Intervention Core` mod distribution profile in the target mod configuration layout.
2. **Assembly Reference:** Map `DivineIntervention.dll` into the local IDE project workspace reference index.
3. **Compilation Reference Override:** Within the properties panel of the local project assembly reference, the **Copy Local** (`Private` metadata flag) property **MUST BE EXPLICITLY SET TO `FALSE**`.

> **CRITICAL ARCHITECTURAL REQUIREMENT:** Failure to isolate assembly packaging by leaving Copy Local set to True results in localized instantiation loops. This breaks memory isolation bounds, halts inter-assembly message bus communications, and induces unpredictable state exceptions within the active runtime engine.

4. **Schema Structuring:** Inject the tracking configuration schema directly into the project's `/About/About.xml` metadata definition file:

```xml
<modDependencies>
    <li>
        <packageId>DivineIntervention.Core</packageId>
        <displayName>Divine Intervention Core</displayName>
    </li>
</modDependencies>

```

---

## 5.0 LEGAL & COMPLIANCE

**COPYRIGHT NOTICE:** © 2026 KYLE GIVLER.

**DISTRIBUTION:** This software architecture specification and its associated binaries are distributed under the strict guidelines of the MIT Open Source License agreement. The maintainer guarantees no structural stability regarding localized organic asset tracking or colony simulation anomalies caused by runtime script deviations.

---

**[DI-CORE: SYSTEMS ARCHITECTURE MANUAL]**
