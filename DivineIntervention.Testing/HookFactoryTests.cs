/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/


using DivineIntervention.Hooking;
using DivineIntervention.Hooking.Internal;
using DivineIntervention.Logging;
using NUnit.Framework;
using System.Reflection;

namespace DivineIntervention.Tests;


[TestFixture]
public class HookFactoryTests
{
    public static class MockStaticTarget
    {
        public static void StaticMethod() { }
    }

    public class MockOverloadedTarget
    {
        public void Execute() { }
        public void Execute(int value) { }
    }

    [SetUp]
    public void Setup()
    {
        //Inject the mock harmony engine
        HookFactory.HarmonyEngine = new MockHarmonyEngine();

        // Redirect the framework logger to standard Console out for headless execution
        DivineLog.ErrorRouter = (message) => System.Console.WriteLine($"[LOG REDIRECT] {message}");
    }

    // Dummy class to use as a target for our hooks
    public class MockTarget
    {
        public void TargetMethod() { }
    }

    [Test]
    public void Create_FirstCall_RegistersHookAndPatches()
    {
        // Act
        IHook hook = HookFactory.Create<MockTarget>(
            nameof(MockTarget.TargetMethod),
            (instance) => { /* Dummy */ }
        );

        // Assert
        Assert.IsNotNull(hook, "Hook should not be null.");

        // If the factory logic is correct, the HookDispatcher 
        // should have registered this hook for the method.
        // (TODO: need to expose a way to check registration in Dispatcher)
    }

    [Test]
    public void Create_SubsequentCalls_DoesNotRePatchHarmonyEngine()
    {
        var mockEngine = (MockHarmonyEngine)HookFactory.HarmonyEngine;
        mockEngine.ClearTracking();

        // Act - Register first hook
        HookFactory.Create<MockTarget>(nameof(MockTarget.TargetMethod), (instance) => { });
        int patchesAfterFirstCall = mockEngine.PatchCallCount;

        // Act - Register second hook on same method
        HookFactory.Create<MockTarget>(nameof(MockTarget.TargetMethod), (instance) => { });

        // Assert
        Assert.AreEqual(patchesAfterFirstCall, mockEngine.PatchCallCount,
            "HarmonyEngine.Patch should not be called again for an already patched method.");
    }

    [Test]
    public void Create_InvalidMethod_ReturnsNullAndLogsError()
    {
        // Act
        IHook hook = HookFactory.Create<MockTarget>("NonExistentMethod", (instance) => { });

        // Assert
        Assert.IsNull(hook, "Factory should return null for invalid methods.");
    }

    [Test]
    public void Create_StaticTypeOverload_SuccessfullyRegistersHook()
    {
        // Act
        IHook hook = HookFactory.Create(
            typeof(MockStaticTarget),
            nameof(MockStaticTarget.StaticMethod),
            onPrefix: (instance, args) => true
        );

        // Assert
        Assert.IsNotNull(hook, "Hook for static method should not be null.");
    }

    [Test]
    public void DispatcherEmptyEvent_TriggersHarmonyUnpatch()
    {
        var mockEngine = (MockHarmonyEngine)HookFactory.HarmonyEngine;
        mockEngine.ClearTracking();

        var targetMethod = typeof(MockTarget).GetMethod(nameof(MockTarget.TargetMethod));

        // Arrange - Register a hook so the framework is actively tracking the target method
        IHook hook = HookFactory.Create<MockTarget>(nameof(MockTarget.TargetMethod), (instance) => { });

        // Act - Force the cleanup event to fire using reflection assignment
        var onMethodEmptyDelegate = typeof(HookDispatcher)
            .GetField("OnMethodEmpty", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
            ?.GetValue(null) as System.Action<MethodBase>;

        onMethodEmptyDelegate?.Invoke(targetMethod);

        // Assert
        Assert.IsTrue(mockEngine.WasUnpatchCalledFor(targetMethod),
            "HarmonyEngine.Unpatch should be called automatically when a method's hook queue becomes empty.");
    }

    [Test]
    public void Create_AmbiguousOverload_HandlesExceptionGracefully()
    {
        // Act & Assert
        // This will let you observe if GetMethod throws an unhandled AmbiguousMatchException
        Assert.DoesNotThrow(() =>
        {
            HookFactory.Create<MockOverloadedTarget>(nameof(MockOverloadedTarget.Execute), (instance) => { });
        });
    }
}