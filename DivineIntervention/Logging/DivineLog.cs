/*
 * Divine Intervention RimWorld Modding Framework
 * 
 * Make Mods the Right Way(tm)
 * 
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Verse;

namespace DivineIntervention.Logging
{
    /// <summary>
    /// A high-performance, isolated logging engine supporting both self-managed (DIY) 
    /// and framework-cached (Registry) patterns. Allows dynamic runtime color modification 
    /// while preserving hot-path string optimization.
    /// </summary>
    public class DivineLog
    {
        private static readonly ConcurrentDictionary<string, DivineLog> _registry = new();

        private readonly string _prefix;
        private readonly bool _useColor;
        private string _color;
        private string _formattedPrefix;

        /// <summary>
        /// The Managed Registry: Safely retrieves an existing logger or creates a new one.
        /// Prevents duplicate memory allocations across decoupled files.
        /// </summary>
        public static DivineLog GetLogger(string prefix, string colorHex = "#66CCFF", bool useColor = true)
        {
            return _registry.GetOrAdd(prefix, (key) => new DivineLog(key, colorHex, useColor));
        }

        /// <summary>
        /// Isolated execution route for handling error redirections safely per mod instance.
        /// </summary>
        public Action<string> ErrorRouter { get; set; }

        /// <summary>
        /// Gets or sets the hex color code used for this logger instance.
        /// Automatically recalculates the optimized string cache on update.
        /// </summary>
        public string Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    UpdateFormattedPrefix();
                }
            }
        }

        /// <summary>
        /// The DIY Route: Public constructor for explicit lifecycle and reference management.
        /// </summary>
        public DivineLog(string prefix, string colorHex = "#66CCFF", bool useColor = true)
        {
            _prefix = prefix;
            _useColor = useColor;
            _color = colorHex;

            UpdateFormattedPrefix();
            ErrorRouter = (message) => Log.Error(message);
        }

        /// <summary>
        /// Generates and caches the prefix. This ensures string manipulation only happens 
        /// when initializing the logger or explicitly changing its color.
        /// </summary>
        private void UpdateFormattedPrefix()
        {
            _formattedPrefix = _useColor ? $"<color={_color}>[{_prefix}]</color>" : $"[{_prefix}]";
        }

        /// <summary>
        /// Log a debug message. Automatically stripped entirely out of production Release builds.
        /// </summary>
        [Conditional("DEBUG")]
        public void Debug(string message) => Log.Message($"{_formattedPrefix}: {message}");

        /// <summary>
        /// Log an info message.
        /// </summary>
        public void Info(string message) => Log.Message($"{_formattedPrefix}: {message}");

        /// <summary>
        /// Log a warning message.
        /// </summary>
        public void Warning(string message) => Log.Warning($"{_formattedPrefix} Warning: {message}");

        /// <summary>
        /// Log an error message using the localized instance router.
        /// </summary>
        public void Error(string message) => ErrorRouter?.Invoke($"{_formattedPrefix} Error: {message}");
    }
}