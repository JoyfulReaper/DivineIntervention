///*
// * Divine Intervention RimWorld Modding Framework
// * Make Mods the Right Way(tm)
// * Copyright (c) 2026 Kyle Givler
// * Licensed under the MIT License.
// */

//using DivineIntervention.Logging;

//namespace DivineIntervention.Examples
//{
//    /// <summary>
//    /// Showcases the comprehensive capabilities of the Divine Intervention logging subsystem.
//    /// Demonstrates context shifting, automatic resource cleanup, and target-agnostic diagnostic routing.
//    /// </summary>
//    public static class LoggingDemonstration
//    {
//        public static void RunAllExamples()
//        {
//            // ==================================================================================
//            // 1. MOD INITIALIZATION (Set these properties at your mod's entry point)
//            // ==================================================================================
//            DivineLog.LoggingPrefix = "MyAwesomeMod";
//            DivineLog.UseColor = true;
//            DivineLog.LoggingColor = "#66CCFF"; // Electric Ice Blue default

//            // ==================================================================================
//            // 2. STANDARD LOGGING CHANNELS
//            // ==================================================================================

//            // Zero-overhead diagnostic tracking. 
//            // The [Conditional("DEBUG")] attribute handles the "intervening" here—
//            // it is physically removed from the compiled assembly during production builds.
//            DivineLog.Debug("Subsystem memory allocation mapping initialized.");

//            // Standard status updates
//            DivineLog.Info("Core engine services connected smoothly.");

//            // Warnings and Errors preserve native RimWorld console behavior 
//            // while retaining your custom mod branding
//            DivineLog.Warning("Optional configuration node missing. Reverting to factory defaults.");
//            DivineLog.Error("Critical structural exception! Local database table is unreachable.");

//            // ==================================================================================
//            // 3. THE POWER USER STATE ENGINE (Color Toggling via Scopes)
//            // ==================================================================================

//            DivineLog.Info("This line renders in the default Electric Ice Blue.");

//            // Shift context safely using the IDisposable pattern. 
//            // The 'using' block guarantees the color resets to the previous state,
//            // even if the block exits prematurely due to an exception.
//            using (new DivineLog.ColorScope("#FFDD55")) // Neon Amber
//            {
//                DivineLog.Info("Context changed: Processing sensitive XML payload files...");
//                DivineLog.Debug("This inner debug line is also styled in Neon Amber!");

//                // Nesting works out of the box because the ColorScope struct preserves the stack state
//                using (new DivineLog.ColorScope("#FF5555")) // Hot Red Override
//                {
//                    DivineLog.Info("Nested Emergency: Network timeout detected. Retrying handshake...");
//                }

//                DivineLog.Info("Returned cleanly back to the Neon Amber context.");
//            }

//            // Zero leak. Zero accidental color persistence across third-party mods.
//            DivineLog.Info("Back to baseline. Zero leakage into subsequent global logs.");
//        }
//    }
//}