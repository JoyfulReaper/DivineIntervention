/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

using HarmonyLib;
using System.Reflection;

namespace DivineIntervention.Hooking.Internal;

public class HarmonyEngine : IHarmonyEngine
{
    private readonly Harmony _instance;
    public HarmonyEngine(string id) => _instance = new Harmony(id);

    public void Patch(MethodBase original, HookMethodInfo prefix = null, HookMethodInfo postfix = null)
    {
        // We map our clean types to Harmony types right before calling Harmony
        _instance.Patch(original,
            prefix: prefix != null ? new HarmonyMethod(prefix.Method) : null,
            postfix: postfix != null ? new HarmonyMethod(postfix.Method) : null
        );
    }

    public void Unpatch(MethodBase method, HookPatchType type)
    {
        var harmonyType = type == HookPatchType.All ? HarmonyPatchType.All :
                          type == HookPatchType.Prefix ? HarmonyPatchType.Prefix : HarmonyPatchType.Postfix;
        _instance.Unpatch(method, harmonyType);
    }
}