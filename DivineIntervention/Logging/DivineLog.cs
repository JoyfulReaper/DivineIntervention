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

namespace DivineIntervention.Logging;

public class DivineLog
{
    private static readonly ConcurrentDictionary<string, DivineLog> _registry = new();

    /// <summary>
    /// Gets or sets the target where all log instances route their output.
    /// Can be swapped at runtime to isolate testing environments.
    /// </summary>
    public static ILogTarget Target { get; set; } = new RimWorldLogTarget();

    private readonly string _prefix;
    private readonly bool _useColor;
    private string _color;
    private string _formattedPrefix;

    public static DivineLog GetLogger(string prefix, string colorHex = "#66CCFF", bool useColor = true)
    {
        return _registry.GetOrAdd(prefix, (key) => new DivineLog(key, colorHex, useColor));
    }

    public Action<string> ErrorRouter { get; set; }

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

    public DivineLog(string prefix, string colorHex = "#66CCFF", bool useColor = true)
    {
        _prefix = prefix;
        _useColor = useColor;
        _color = colorHex;

        UpdateFormattedPrefix();

        // Default the router to point at our target instead of hardcoding Verse.Log
        ErrorRouter = (message) => Target.WriteError(message);
    }

    private void UpdateFormattedPrefix()
    {
        _formattedPrefix = _useColor ? $"<color={_color}>[{_prefix}]</color>" : $"[{_prefix}]";
    }

    [Conditional("DEBUG")]
    public void Debug(string message) =>
        Target.WriteMessage($"{_formattedPrefix}: {message}");

    public void Info(string message) =>
        Target.WriteMessage($"{_formattedPrefix}: {message}");

    public void Warning(string message) =>
        Target.WriteWarning($"{_formattedPrefix} Warning: {message}");

    public void Error(string message) =>
        ErrorRouter?.Invoke($"{_formattedPrefix} Error: {message}");
}