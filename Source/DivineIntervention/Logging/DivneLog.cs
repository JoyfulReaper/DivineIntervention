/*
 * Divine Intervention RimWorld Modding Framework
 * 
 * This code is partially based on Replace Stuff
 * Copyright (c) 2025 Alex Tearse-Doyle
 * Licensed under the MIT License.
 *
 * Modified by Kyle Givler
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using System.Diagnostics;

namespace DivineIntervention.Logging
{
    /// <summary>
    /// Core framework logging utility. Provides unified, throttled, and conditionally compiled 
    /// logging channels across all framework modules and consuming mods.
    /// </summary>
    public static class DivineLog
    {
        /// <summary>
        /// Gets or sets the active textual identifier used to tag log statements.
        /// Defaults to the framework signature but should be reassigned at mod initialization.
        /// </summary>
        public static string LoggingPrefix { get; set; } = "DivineIntervention";

        /// <summary>
        /// Gets or sets whether rich text hexadecimal color tags should be injected into standard log output streams.
        /// </summary>
        public static bool UseColor { get; set; } = true;

        /// <summary>
        /// Generates the stylized, context-aware prefix string for message routing.
        /// </summary>
        private static string FormattedPrefix => UseColor
            ? $"<color=#66CCFF>[{LoggingPrefix}]</color>"
            : $"[{LoggingPrefix}]";

        /// <summary>
        /// Writes a verbose message to the engine log console only if the assembly is compiled under a Debug configuration flag.
        /// </summary>
        /// <param name="message">The informational text payload to display.</param>
        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            Verse.Log.Message($"{FormattedPrefix}: {message}");
        }

        /// <summary>
        /// Writes an informational message to the core engine log console across all build targets.
        /// </summary>
        /// <param name="message">The informational text payload to display.</param>
        public static void Info(string message)
        {
            Verse.Log.Message($"{FormattedPrefix}: {message}");
        }

        /// <summary>
        /// Appends a non-fatal anomaly alert warning to the engine's tracking database.
        /// </summary>
        /// <param name="message">The warning text payload describing the execution anomaly.</param>
        public static void Warning(string message)
        {
            // Bypasses color tags entirely to keep native warning highlighting completely clean
            Verse.Log.Warning($"[{LoggingPrefix} Warning]: {message}");
        }

        /// <summary>
        /// Forces a critical, high-visibility engine error registration execution sequence.
        /// </summary>
        /// <param name="message">The descriptive failure context payload explaining the exception.</param>
        public static void Error(string message)
        {
            // Bypasses color tags entirely to keep native error tracing and stack markers precise
            Verse.Log.Error($"[{LoggingPrefix} Error]: {message}");
        }
    }
}