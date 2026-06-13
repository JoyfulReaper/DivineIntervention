/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

using DivineIntervention.Hooking;
using System.Reflection;

namespace DivineIntervention.Tests
{
    public class MockHarmonyEngine : IHarmonyEngine
    {
        public bool PatchCalled => PatchCallCount > 0;
        public int PatchCallCount { get; private set; }
        public bool UnpatchCalled { get; private set; }
        public MethodBase LastUnpatchedMethod { get; private set; }

        public void Patch(MethodBase original, HookMethodInfo prefix = null, HookMethodInfo postfix = null)
        {
            PatchCallCount++;
        }

        public void Unpatch(MethodBase method, HookPatchType type)
        {
            UnpatchCalled = true;
            LastUnpatchedMethod = method;
        }

        /// <summary>
        /// Verification helper for lifecycle assertions.
        /// </summary>
        public bool WasUnpatchCalledFor(MethodBase method)
        {
            return UnpatchCalled && LastUnpatchedMethod == method;
        }

        public void ClearTracking()
        {
            PatchCallCount = 0;
            UnpatchCalled = false;
            LastUnpatchedMethod = null;
        }
    }
}