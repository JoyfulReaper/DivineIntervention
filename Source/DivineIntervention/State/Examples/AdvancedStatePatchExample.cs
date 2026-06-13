///*
//* Divine Intervention RimWorld Modding Framework
//* Make Mods the Right Way(tm)
//* Copyright (c) 2026 Kyle Givler
//* Licensed under the MIT License.
//*/

//using DivineIntervention.Logging;
//using DivineIntervention.Patching;
//using HarmonyLib;
//using System.Reflection;
//using Verse;

//namespace DivineIntervention.Examples
//{
//    /// <summary>
//    /// Current minimap state used to determine whether
//    /// Dubs' update patch should be enabled.
//    /// </summary>
//    public struct MiniMapContext
//    {
//        public bool IsOpen;
//        public bool MapChanged;
//        public bool JustLoaded;
//    }

//    /// <summary>
//    /// ADVANCED PATTERN: Dynamic State-Driven Patching
//    /// Demonstrates how to use a state engine to dynamically patch and unpatch 
//    /// performance-heavy third-party methods based on real-time game state.
//    /// </summary>
//    public static class DubsPatchController
//    {
//        private static DynamicPatchProcessor<MiniMapContext> _optimizer;
//        private static Map _lastMap;
//        private static bool _isDubsAvailable;
//        private static bool _justLoaded;

//        // Cached reflection fields to avoid crashing when the target mod is missing
//        private static PropertyInfo _settingsOpenProperty;
//        private static object _settingsInstance;

//        /// <summary>
//        /// Locates third-party assembly hooks and instantiates the generic optimization pipeline engine.
//        /// </summary>
//        public static void Initialize(Harmony harmony)
//        {
//            var tryUpdate = AccessTools.Method(typeof(Section), "TryUpdate");
//            var dubsType = AccessTools.TypeByName("DubsMintMinimap.Harmony_TryUpdate");

//            if (dubsType == null || tryUpdate == null)
//            {
//                _isDubsAvailable = false;
//                DivineLog.Info("Dubs Mint Minimap not detected. Optimization pipeline skipped.");
//                return;
//            }

//            var prefix = AccessTools.Method(dubsType, "Prefix");

//            // Safe Reflection Setup to read settings without a hard assembly reference dependency
//            var modType = AccessTools.TypeByName("DubsMintMinimap.DubsMintMinimapMod");
//            if (modType != null)
//            {
//                var settingsField = AccessTools.Field(modType, "Settings");
//                _settingsInstance = settingsField?.GetValue(null);
//                if (_settingsInstance != null)
//                {
//                    _settingsOpenProperty = AccessTools.Property(_settingsInstance.GetType(), "MinimapOpen");
//                }
//            }

//            if (_settingsOpenProperty == null)
//            {
//                DivineLog.Error("Failed to reflect into Dubs Mint Minimap settings structures.");
//                return;
//            }

//            // Initialize the framework engine with our context data schema and inline reducer rules
//            _optimizer = new DynamicPatchProcessor<MiniMapContext>(
//                harmony,
//                tryUpdate,
//                prefix,
//                context =>
//                {
//                    // The Pure Reduction Layer: Computes required state seamlessly
//                    if (context.MapChanged || context.JustLoaded)
//                        return PatchCommand.Enable;

//                    return context.IsOpen
//                        ? PatchCommand.Enable
//                        : PatchCommand.Disable;
//                }
//            );

//            _isDubsAvailable = true;
//            DivineLog.Debug("Dynamic DubsPatchController optimization pipeline successfully initialized.");
//        }

//        public static void OnGameLoaded()
//        {
//            _justLoaded = true;
//        }

//        /// <summary>
//        /// Call this from a central component Update loop (e.g., a ModWorldComponent or Root update).
//        /// </summary>
//        public static void Update()
//        {
//            if (!_isDubsAvailable || _optimizer == null)
//                return;

//            var mapChanged = Find.CurrentMap != _lastMap;
//            if (mapChanged)
//                _lastMap = Find.CurrentMap;

//            // Gather environmental info and pack the transient state frame
//            var context = new MiniMapContext
//            {
//                IsOpen = IsMiniMapOpen(),
//                MapChanged = mapChanged,
//                JustLoaded = _justLoaded
//            };

//            // Consume transient frame flags immediately
//            _justLoaded = false;

//            // Dispatch context state directly into the pipeline engine
//            _optimizer.Update(context);
//        }

//        private static bool IsMiniMapOpen()
//        {
//            if (_settingsOpenProperty == null || _settingsInstance == null)
//                return false;

//            // Safely resolve via reflection wrapper
//            bool isOpen = (bool)_settingsOpenProperty.GetValue(_settingsInstance, null);
//            return isOpen && Find.CurrentMap != null;
//        }
//    }
//}