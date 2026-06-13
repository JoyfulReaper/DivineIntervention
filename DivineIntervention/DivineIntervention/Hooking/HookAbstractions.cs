/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* Copyright (c) 2026 Kyle Givler
* 
* Licensed under the MIT License.
*/

namespace DivineIntervention.Hooking;

// Enum to replace HarmonyPatchType
public enum HookPatchType { All, Prefix, Postfix }

// Wrapper to replace HarmonyMethod
public class HookMethodInfo
{
    public System.Reflection.MethodInfo Method { get; set; }
    public HookMethodInfo(System.Reflection.MethodInfo method) => Method = method;
}