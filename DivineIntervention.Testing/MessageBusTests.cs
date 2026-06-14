/*
 * Divine Intervention RimWorld Modding Framework
 * 
 * Make Mods the Right Way(tm)
 * 
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using DivineIntervention.Events;
using DivineIntervention.Logging;
using NUnit.Framework;
using System;

namespace DivineIntervention.Tests.Events;

[TestFixture]
public class MessageBusTests
{
    // console sink for testing
    private class TestLogTarget : ILogTarget
    {
        public void WriteMessage(string msg) =>
            Console.WriteLine($"[INFO] {msg}");
        public void WriteWarning(string msg) =>
            Console.WriteLine($"[WARN] {msg}");
        public void WriteError(string msg) =>
            Console.WriteLine($"[ERR] {msg}");
    }

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        // Intercept the log pipelines completely
        DivineLog.Target = new TestLogTarget();
    }

    #region Test Payload Definitions

    // Unique structures to prevent cross-test state pollution in the static registries
    private struct StandardPayload { public string Status; }
    private struct MultiSubscriberPayload { public int Value; }
    private struct UnsubscribePayload { }
    private struct FaultyPayload { }

    #endregion

    #region Typed Lane Tests (High-Performance)

    [Test]
    public void TypedLane_Publish_DeliversMessageToRegisteredSubscriber()
    {
        // Arrange
        string receivedStatus = null;
        Action<StandardPayload> subscriber =
            (msg) => receivedStatus = msg.Status;

        MessageBus.Subscribe(subscriber);

        // Act
        MessageBus.Publish(new StandardPayload { Status = "FrameworkCoreReady" });

        // Assert
        Assert.That(receivedStatus, Is.EqualTo("FrameworkCoreReady"));

        // Teardown
        MessageBus.Unsubscribe(subscriber);
    }

    [Test]
    public void TypedLane_MultipleSubscribers_AllRegisteredCallbacksReceiveBroadcast()
    {
        // Arrange
        int totalValueA = 0;
        int totalValueB = 0;
        Action<MultiSubscriberPayload> subA = (msg) => totalValueA += msg.Value;
        Action<MultiSubscriberPayload> subB = (msg) => totalValueB += msg.Value;

        MessageBus.Subscribe(subA);
        MessageBus.Subscribe(subB);

        // Act
        MessageBus.Publish(new MultiSubscriberPayload { Value = 42 });

        // Assert
        Assert.That(totalValueA, Is.EqualTo(42));
        Assert.That(totalValueB, Is.EqualTo(42));

        // Teardown
        MessageBus.Unsubscribe(subA);
        MessageBus.Unsubscribe(subB);
    }

    [Test]
    public void TypedLane_Unsubscribe_RemovesRegistrationAndStopsMessageDelivery()
    {
        // Arrange
        int executionCount = 0;
        Action<UnsubscribePayload> subscriber = (msg) => executionCount++;

        MessageBus.Subscribe(subscriber);
        MessageBus.Publish(new UnsubscribePayload()); // count -> 1

        // Act
        MessageBus.Unsubscribe(subscriber);
        MessageBus.Publish(new UnsubscribePayload()); // Should bypass

        // Assert
        Assert.That(executionCount, Is.EqualTo(1));
    }

    [Test]
    public void TypedLane_SubscriberThrowsException_IsCaughtSafelyAndDoesNotCrashPublisher()
    {
        // Arrange
        bool companionExecuted = false;
        Action<FaultyPayload> brokenSub = (msg) => throw new InvalidOperationException("Randy structural damage simulation.");
        Action<FaultyPayload> healthySub = (msg) => companionExecuted = true;

        MessageBus.Subscribe(brokenSub);
        MessageBus.Subscribe(healthySub);

        // Act & Assert
        // Verifies the entire publish call runs error-free due to internal catch blocks
        Assert.DoesNotThrow(() => MessageBus.Publish(new FaultyPayload()));
        Assert.That(companionExecuted, Is.True, "The healthy companion subscriber should still execute.");

        // Teardown
        MessageBus.Unsubscribe(brokenSub);
        MessageBus.Unsubscribe(healthySub);
    }

    #endregion

    #region Loose Lane Tests (String-Based)

    [Test]
    public void LooseLane_Publish_DeliversUntypedPayloadToCorrectTopicString()
    {
        // Arrange
        const string topic = "DI_TestTopic_StandardPublish";
        object capturedData = null;
        Action<object> subscriber = (data) => capturedData = data;

        MessageBus.Subscribe(topic, subscriber);

        // Act
        MessageBus.Publish(topic, "Plasteel_Shipment_500");

        // Assert
        Assert.That(capturedData, Is.EqualTo("Plasteel_Shipment_500"));

        // Teardown
        MessageBus.Unsubscribe(topic, subscriber);
    }

    [Test]
    public void LooseLane_Unsubscribe_RemovesSubscriberFromTopicChain()
    {
        // Arrange
        const string topic = "DI_TestTopic_UnsubChain";
        int count = 0;
        Action<object> subscriber = (data) => count++;

        MessageBus.Subscribe(topic, subscriber);
        MessageBus.Publish(topic, null); // count -> 1

        // Act
        MessageBus.Unsubscribe(topic, subscriber);
        MessageBus.Publish(topic, null); // Should bypass

        // Assert
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public void LooseLane_SubscriberThrowsException_IsCaughtSafelyAndContinuesBackwardsIterationLoop()
    {
        // Arrange
        const string topic = "DI_TestTopic_FaultySubLoop";
        bool subsequentExecuted = false;

        Action<object> brokenSub = (data) => throw new Exception("Component stack overflow simulation.");
        Action<object> healthySub = (data) => subsequentExecuted = true;

        MessageBus.Subscribe(topic, brokenSub);
        MessageBus.Subscribe(topic, healthySub);

        // Act & Assert
        Assert.DoesNotThrow(() => MessageBus.Publish(topic, "data_payload"));
        Assert.That(subsequentExecuted, Is.True, "The inverted 'for' loop should continue executing remaining items.");

        // Teardown
        MessageBus.Unsubscribe(topic, brokenSub);
        MessageBus.Unsubscribe(topic, healthySub);
    }

    #endregion
}