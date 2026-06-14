/*
 * Divine Intervention RimWorld Modding Framework
 * 
 * Make Mods the Right Way(tm)
 * 
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

#if DEBUG
using DivineIntervention.Logging;

namespace DivineIntervention.Examples;

/// <summary>
/// Showcases the flexible capabilities of the Divine Intervention logging subsystem.
/// Demonstrates how developers can choose between self-managed loggers (DIY) or 
/// global cached access (Managed Registry) while ensuring absolute thread safety and cross-mod isolation.
/// </summary>
public static class LoggingDemonstration
{
    public static void RunAllExamples()
    {
        RunDiyExample();
        RunRegistryExample();
    }

    // ==================================================================================
    // APPROACH 1: THE DIY ROUTE (Explicit Lifecycle Ownership)
    // ==================================================================================
    // Best for power users who want complete control over reference scoping. 
    // Instantiated once at your mod's entry point and held as a private/internal reference.
    private static readonly DivineLog DiyLog = new DivineLog("MyDiyMod", "#66CCFF", true);

    private static void RunDiyExample()
    {
        DiyLog.Info("--- Starting DIY Logger Example ---");

        // Zero-overhead diagnostic tracking. Stripped entirely out of production Release builds.
        DiyLog.Debug("DIY Subsystem memory allocation mapping initialized.");

        // Standard channels preserving native console formatting with your custom branding
        DiyLog.Warning("Optional configuration node missing. Reverting to factory defaults.");
        DiyLog.Error("Critical structural exception! Local database table is unreachable.");

        // Changing instance colors dynamically via the Color property.
        // This pre-computes the tag formatting behind the scenes to keep hot paths allocation-free.
        DiyLog.Color = "#FFDD55"; // Shift to Neon Amber hex
        DiyLog.Info("Context changed: Processing sensitive XML payload files inside DIY mod...");

        // Unity's rich text processor natively understands literal names as well!
        DiyLog.Color = "red"; // Switch to text-defined literal red
        DiyLog.Info("Emergency context: Operation timed out.");

        // Return cleanly back to the baseline color profile
        DiyLog.Color = "#66CCFF";
        DiyLog.Info("DIY baseline color perfectly restored. Zero cross-mod configuration leaks.");
    }

    // ==================================================================================
    // APPROACH 2: THE MANAGED REGISTRY ROUTE (Zero Reference Passing)
    // ==================================================================================
    // Best for fast development. You can pull the exact same cached instance from 
    // any file, any namespace, or any thread within your assembly instantly.
    private static void RunRegistryExample()
    {
        // Fetching a logger from anywhere. 
        // The concurrent cache handles allocations, so calling this repeatedly is incredibly cheap.
        DivineLog registryLog = DivineLog.GetLogger("MyRegistryMod", "#AA77FF", true);

        registryLog.Info("--- Starting Managed Registry Logger Example ---");
        registryLog.Info("Core registry services connected smoothly.");

        // Demonstration of global file fetch equivalence:
        // Pretend this line is in an entirely separate file, deep inside a Harmony patch.
        // Because the prefix matches, the underlying framework hands back the exact same instance!
        DivineLog sameLogFromAnotherFile = DivineLog.GetLogger("MyRegistryMod");
        sameLogFromAnotherFile.Debug("Fetched safely from a decoupled codebase context without allocating new memory.");

        // Modifying a registry-cached instance's color profile at runtime (e.g., from an options window toggle)
        registryLog.Color = "cyan";
        registryLog.Info("Registry mod is executing a high-priority database sync task...");

        registryLog.Color = "#AA77FF";
        registryLog.Info("Registry baseline color restored cleanly.");
    }
}
#endif