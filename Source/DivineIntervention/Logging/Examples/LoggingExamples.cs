/*
* Divine Intervention RimWorld Modding Framework
* Make Mods the Right Way(tm)
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/
using DivineIntervention.Logging;

namespace DivineIntervention.Examples
{
    public static class LoggingDemonstration
    {
        public static void RunAllExamples()
        {
            // Configuration: Set these at Mod initialization
            DivineLog.LoggingPrefix = "MyAwesomeMod";
            DivineLog.UseColor = true;

            // The Log Cases

            // Debug: Only appears in Debug builds (no extra work needed here)
            DivineLog.Debug("Verbose technical data (hidden in production)");

            // Info: Standard status updates
            DivineLog.Info("System initialized successfully.");

            // Warning: Non-fatal issues (e.g., config file missing, using defaults)
            DivineLog.Warning("Settings file not found, loading default profile.");

            // Error: Critical failures (e.g., failed to patch core method)
            DivineLog.Error("Failed to initialize Harmony hooks!");
        }
    }
}