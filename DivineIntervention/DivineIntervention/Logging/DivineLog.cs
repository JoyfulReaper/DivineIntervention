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

using System;
using System.Diagnostics;
using Verse;

namespace DivineIntervention.Logging;

/// <summary>
/// A static class for logging messages with customizable color and prefix.
/// </summary>
public static class DivineLog
{
    // Default execution route uses RimWorld's native logger
    public static Action<string> ErrorRouter { get; set; } = (message) =>
        Log.Error(message);

    /// <summary>
    /// The prefix for all log messages.
    /// </summary>
    public static string LoggingPrefix { get; set; } = "DivineIntervention";

    /// <summary>
    /// Whether to use color in log messages or not.
    /// </summary>
    public static bool UseColor { get; set; } = true;

    /// <summary>
    /// The color used for log messages if UseColor is true.
    /// </summary>
    public static string LoggingColor { get; set; } = "#66CCFF";

    /// <summary>
    /// The formatted prefix for log messages.
    /// </summary>
    private static string FormattedPrefix => UseColor
        ? $"<color={LoggingColor}>[{LoggingPrefix}]</color>"
        : $"[{LoggingPrefix}]";

    /// <summary>
    /// A struct for creating a scope where LoggingColor can be changed and will revert back automatically.
    /// </summary>
    /// <param name="newColor">The color to set for the scope.</param>
    public struct ColorScope : IDisposable
    {
        private readonly string _previousColor;

        /// <summary>
        /// Create a new ColorScope with the specified color.
        /// </summary>
        /// <param name="newColor">The color to set for the scope.</param>
        public ColorScope(string newColor)
        {
            _previousColor = LoggingColor;
            LoggingColor = newColor;
        }

        /// <summary>
        /// Dispose of the ColorScope, reverting LoggingColor back to the previous value.
        /// </summary>
        public void Dispose()
        {
            LoggingColor = _previousColor;
        }
    }

    /// <summary>
    /// Log a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    [Conditional("DEBUG")]
    public static void Debug(string message) =>
        Log.Message($"{FormattedPrefix}: {message}");

    /// <summary>
    /// Log an info message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Info(string message) =>
        Log.Message($"{FormattedPrefix}: {message}");

    /// <summary>
    /// Log a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Warning(string message) =>
        Log.Warning($"[{LoggingPrefix} Warning]: {message}");

    /// <summary>
    /// Log an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Error(string message) =>
        ErrorRouter?.Invoke(message);
}