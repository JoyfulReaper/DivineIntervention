/*
 * Divine Intervention RimWorld Modding Framework
 * 
 * Make Mods the Right Way(tm)
 * 
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

// Type Safe Message Bus Example

//using DivineIntervention.Events;
//using DivineIntervention.Logging;
//using HarmonyLib;
//using MyEconomyMod.Messages;
//using RimWorld;
//using Verse;

//namespace MyEconomyMod.Messages
//{

//    // Message Payload
//    public struct TradeCompletedMessage
//    {
//        public string TraderName;
//        public int SilverExchanged;
//        public int TicksGame;
//    }
//}

//////////////////////////

//// Example Publisher
//namespace MyEconomyMod.Patches
//{
//    [HarmonyPatch(typeof(TradeDeal), "TryExecute")]
//    public static class TradeDeal_TryExecute_Patch
//    {
//        public static void Postfix(bool __result, TradeDeal __instance)
//        {
//            // If the trade failed or canceled, do nothing
//            if (!__result) return;

//            // Calculate silver (simplified for the example)
//            int silverNet = 0; // Logic to calculate silver diff goes here

//            var msg = new TradeCompletedMessage
//            {
//                TraderName = TradeSession.trader?.TraderName ?? "Unknown",
//                SilverExchanged = silverNet,
//                TicksGame = Find.TickManager.TicksGame
//            };

//            // Broadcast it to the framework
//            MessageBus.Publish(msg);
//        }
//    }
//}

////////////////////////

//// Example Subsciber
//namespace MyEconomyMod.UI
//{
//    public static class TradeNotifier
//    {
//        // Call this during your Mod's Initialize phase
//        public static void Initialize()
//        {
//            MessageBus.Subscribe<TradeCompletedMessage>(OnTradeCompleted);
//        }

//        private static void OnTradeCompleted(TradeCompletedMessage msg)
//        {
//            if (msg.SilverExchanged > 1000)
//            {
//                Verse.Messages.Message($"Massive trade with {msg.TraderName}!", MessageTypeDefOf.PositiveEvent);
//            }
//        }
//    }
//}


////////////////////////

//// Example Subscriber
//namespace MyEconomyMod.Tracking
//{
//    public class EconomyTracker : GameComponent
//    {
//        public int TotalSilverTraded = 0;

//        public EconomyTracker(Game game)
//        {
//            // Subscribe when the game component is created
//            MessageBus.Subscribe<TradeCompletedMessage>(RecordTrade);
//        }

//        private void RecordTrade(TradeCompletedMessage msg)
//        {
//            TotalSilverTraded += System.Math.Abs(msg.SilverExchanged);
//            DivineLog.Debug($"Economy updated. Lifetime silver traded: {TotalSilverTraded}");
//        }

//        // Extremely important: GameComponents are destroyed when returning to the main menu. 
//        // We must unsubscribe, or the next game load will cause a memory leak and duplicate events!
//        public override void FinalizeInit()
//        {
//            // (Rimworld calls this when game is tearing down/starting up)
//            // It's safer to implement IDisposable or handle teardown in standard map/game lifecycle hooks
//        }

//        public void Teardown()
//        {
//            MessageBus.Unsubscribe<TradeCompletedMessage>(RecordTrade);
//        }
//    }
//}