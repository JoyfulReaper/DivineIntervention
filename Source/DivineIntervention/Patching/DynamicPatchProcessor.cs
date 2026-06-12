/*
* Divine Intervention RimWorld Modding Framework
* 
* Make Mods the Right Way(tm)
* 
* Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/

using HarmonyLib;
using System;
using System.Reflection;

namespace ForgeCore.Patching
{
    public enum PatchCommand { Enable, Disable, NoOp }

    public class DynamicPatchProcessor<TContext>
    {
        private readonly Harmony _harmony;
        private readonly MethodInfo _targetMethod;
        private readonly MethodInfo _prefixMethod;
        private readonly Func<TContext, PatchCommand> _reducer;

        private PatchCommand _lastCommand = PatchCommand.NoOp;

        public DynamicPatchProcessor(
            Harmony harmony,
            MethodInfo target,
            MethodInfo prefix,
            Func<TContext, PatchCommand> reducer)
        {
            _harmony = harmony;
            _targetMethod = target;
            _prefixMethod = prefix;
            _reducer = reducer;
        }

        public void Update(TContext context)
        {
            //Evaluate pure logic via the passed delegate
            PatchCommand cmd = _reducer(context);

            // Performance Filter / Deduplication
            if (cmd == PatchCommand.NoOp || cmd == _lastCommand)
                return;

            _lastCommand = cmd;

            // Dispatch execution mutations
            switch (cmd)
            {
                case PatchCommand.Enable:
                    _harmony.Patch(_targetMethod, prefix: new HarmonyMethod(_prefixMethod));
                    break;

                case PatchCommand.Disable:
                    _harmony.Unpatch(_targetMethod, _prefixMethod);
                    break;
            }
        }
    }
}