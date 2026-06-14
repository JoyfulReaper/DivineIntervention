/*
 * Divine Intervention RimWorld Modding Framework
 * 
 * Make Mods the Right Way(tm)
 * 
 *  Copyright (c) 2026 Kyle Givler
 *  Licensed under the MIT License.
 */

using DivineIntervention.Events;
using DivineIntervention.Logging;
using HarmonyLib;
using RimWorld;
using System;
using Verse;

#if DEBUG
namespace MyEconomyMod.Messages
{
    /// <summary>
    /// An immutable, transient data payload representing a completed trade transaction.
    /// This structure contains no behavior, only state.
    /// </summary>
    public struct TradeCompletedMessage
    {
        public string TraderName;
        public int SilverExchanged;
        public int TicksGame;
    }
}

namespace MyEconomyMod.Patches
{
    using MyEconomyMod.Messages;

    /// <summary>
    /// HARMONY PATCH: Intercepts the trade execution window directly.
    /// Perfectly readable by Dubs Performance Analyzer without enterprise wrappers.
    /// </summary>
    [HarmonyPatch(typeof(TradeDeal), "TryExecute")]
    public static class Patch_TradeDeal_TryExecute
    {
        /// <summary>
        /// Postfix hook. Runs unconditionally when TryExecute is called.
        /// </summary>
        /// <param name="confirmed">The boolean return value representing if the transaction completed.</param>
        public static void Postfix(bool confirmed)
        {
            // If the transaction failed or was aborted by the game loop, exit immediately
            if (!confirmed)
                return;

            // Calculate transaction value data frame profiles safely
            int silverNet = 1250; // Simplified placeholder logic for the payload calculation

            var msg = new TradeCompletedMessage
            {
                TraderName = TradeSession.trader?.TraderName ?? "Unknown Trader",
                SilverExchanged = silverNet,
                TicksGame = Find.TickManager.TicksGame
            };

            // Broadcast the payload globally to all listening subscribers
            MessageBus.Publish(msg);
        }
    }
}

namespace MyEconomyMod.UI
{
    using MyEconomyMod.Messages;

    /// <summary>
    /// THE SERVICE HUB: Binds independent UI notifications to the event pipeline.
    /// </summary>
    public static class TradeNotifier
    {
        /// <summary>
        /// Registers our static event pipeline listeners. Called during mod startup.
        /// </summary>
        public static void Initialize()
        {
            MessageBus.Subscribe<TradeCompletedMessage>(OnTradeCompleted);
            DivineLog.Debug("TradeNotifier execution engine hooks successfully mounted.");
        }

        /// <summary>
        /// Safely unbinds from the event pipeline stream during unloading or mod shutdown.
        /// </summary>
        public static void Shutdown()
        {
            MessageBus.Unsubscribe<TradeCompletedMessage>(OnTradeCompleted);
            DivineLog.Debug("TradeNotifier execution engine cleanly unmounted.");
        }

        private static void OnTradeCompleted(TradeCompletedMessage msg)
        {
            if (msg.SilverExchanged > 1000)
            {
                Verse.Messages.Message(
                    $"Massive trade completed with {msg.TraderName}! ({msg.SilverExchanged} silver net)",
                    MessageTypeDefOf.PositiveEvent,
                    historical: true
                );
            }
        }
    }
}

namespace MyEconomyMod.Tracking
{
    using MyEconomyMod.Messages;

    /// <summary>
    /// DATA LAYER SUBSCRIBER: Tracks transaction values over the lifetime of a save game file.
    /// </summary>
    public class EconomyTracker : GameComponent
    {
        public int TotalSilverTraded = 0;

        // Required public constructor for RimWorld engine serialization processing
        public EconomyTracker(Game game) : base()
        {
        }

        /// <summary>
        /// Invoked by RimWorld when starting a new game session or loading a save file.
        /// </summary>
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            MessageBus.Subscribe<TradeCompletedMessage>(RecordTrade);
            DivineLog.Debug("EconomyTracker attached to MessageBus event router stream.");
        }

        private void RecordTrade(TradeCompletedMessage msg)
        {
            TotalSilverTraded += Math.Abs(msg.SilverExchanged);
            DivineLog.Debug($"Economy tracking state updated. Lifetime economy footprint: {TotalSilverTraded} silver.");
        }

        /// <summary>
        /// Invoked by RimWorld during game state saving tasks or return-to-menu teardowns.
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref TotalSilverTraded, "TotalSilverTraded", 0);

            // Safe automated teardown execution if clearing the active state context
            if (Scribe.mode == LoadSaveMode.Saving && Current.ProgramState == ProgramState.Entry)
            {
                Teardown();
            }
        }

        public void Teardown()
        {
            MessageBus.Unsubscribe<TradeCompletedMessage>(RecordTrade);
            DivineLog.Debug("EconomyTracker detached completely from event router streams.");
        }
    }
}
#endif