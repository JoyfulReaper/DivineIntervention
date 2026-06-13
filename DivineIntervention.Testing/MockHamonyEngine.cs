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
        public bool PatchCalled = false;

        // Updated signatures to match your pure abstractions
        public void Patch(MethodBase original, HookMethodInfo prefix = null, HookMethodInfo postfix = null)
            => PatchCalled = true;

        public void Unpatch(MethodBase method, HookPatchType type) { }
    }
}