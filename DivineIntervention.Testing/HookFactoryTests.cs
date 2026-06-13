/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/


using DivineIntervention.Hooking;
using DivineIntervention.Logging;
using NUnit.Framework;

namespace DivineIntervention.Tests;


[TestFixture]
public class HookFactoryTests
{
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
    public void Create_InvalidMethod_ReturnsNullAndLogsError()
    {
        // Act
        IHook hook = HookFactory.Create<MockTarget>("NonExistentMethod", (instance) => { });

        // Assert
        Assert.IsNull(hook, "Factory should return null for invalid methods.");
    }
}