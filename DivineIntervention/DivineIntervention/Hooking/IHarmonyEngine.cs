/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

namespace DivineIntervention.Hooking;

public interface IHarmonyEngine
{
    void Patch(System.Reflection.MethodBase original, HookMethodInfo prefix = null, HookMethodInfo postfix = null);
    void Unpatch(System.Reflection.MethodBase method, HookPatchType type);
}