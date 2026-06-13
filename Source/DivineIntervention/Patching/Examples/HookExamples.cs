///*
//* Divine Intervention RimWorld Modding Framework
//* * Make Mods the Right Way(tm)
//* * Copyright (c) 2026 Kyle Givler
//* Licensed under the MIT License.
//*/

//using DivineIntervention.Logging;
//using DivineIntervention.Patching;
//using Verse;

//namespace DivineIntervention.Examples
//{
//    /// <summary>
//    /// Demonstrates usage patterns for the Divine Intervention Framework.
//    /// This file serves as both documentation and a cheat-sheet for standard hooking architectures.
//    /// </summary>
//    public static class HookExamples
//    {
//        private static IHook _myDynamicHook;

//        #region Basic Patterns

//        /// <summary>
//        /// PATTERN 1: The Observer
//        /// Best for: Logging, tracking stats, or non-intrusive monitoring.
//        /// </summary>
//        public static void RunObserverExample()
//        {
//            HookFactory.Create<Pawn>(
//                "Tick",
//                (pawn) => DivineLog.Debug($"Observer: {pawn.LabelShort} is ticking!")
//            );
//        }

//        /// <summary>
//        /// PATTERN 2: The Conditional Guard
//        /// Best for: Performance-critical hooks that should only run under specific game states.
//        /// </summary>
//        public static void RunConditionalExample()
//        {
//            HookFactory.Create<Pawn>(
//                "Tick",
//                (pawn) => DivineLog.Debug($"Conditional: {pawn.LabelShort} is working."),
//                condition: () => !Find.TickManager.Paused
//            );
//        }

//        /// <summary>
//        /// PATTERN 3: The Dynamic Lifecycle
//        /// Best for: Features that toggle on/off based on settings, UI buttons, or map state.
//        /// </summary>
//        public static void EnableFeature()
//        {
//            _myDynamicHook = HookFactory.Create<Pawn>(
//                "Tick",
//                (pawn) => DivineLog.Debug("Dynamic: Feature active.")
//            );
//        }

//        public static void DisableFeature()
//        {
//            _myDynamicHook?.Unpatch();
//            _myDynamicHook = null;
//        }

//        #endregion

//        #region Power User Patterns

//        /// <summary>
//        /// PATTERN 4: The Hijacker (Return Value Mutation)
//        /// Best for: Balancing tweaks, forced overrides, or debugging specific outputs.
//        /// </summary>
//        public static void RunHijackExample()
//        {
//            HookFactory.Create<Pawn>(
//                "get_HealthScale",
//                // Explicitly declare types for all parameters to satisfy the 'ref' constraint
//                onPostfix: (Pawn instance, object[] args, ref object result) =>
//                {
//                    // Force the return value to be a hardcoded 100f
//                    result = 100f;
//                }
//            );
//        }

//        /// <summary>
//        /// PATTERN 5: The Argument Mutator
//        /// Best for: Changing how the game calculates damage, costs, or behavior inputs.
//        /// </summary>
//        public static void RunMutatorExample()
//        {
//            HookFactory.Create<Pawn>(
//                "TakeDamage",
//                onPrefix: (instance, args) =>
//                {
//                    if (args[0] is DamageInfo dinfo)
//                    {
//                        DivineLog.Debug($"Pawn {instance.LabelShort} is taking {dinfo.Amount} damage!");
//                    }
//                    return true;
//                }
//            );
//        }

//        /// <summary>
//        /// PATTERN 6: The Method Interceptor (Full Override)
//        /// Best for: Completely replacing broken game logic or implementing custom AI.
//        /// </summary>
//        public static void RunInterceptorExample()
//        {
//            HookFactory.Create<Pawn>(
//                "Tick",
//                onPrefix: (instance, args) =>
//                {
//                    // Returning 'false' halts the execution of the original game code.
//                    return false;
//                }
//            );
//        }

//        #endregion
//    }
//}