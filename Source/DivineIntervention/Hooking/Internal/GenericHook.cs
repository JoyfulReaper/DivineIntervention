/*
* Divine Intervention RimWorld Modding Framework
* * Make Mods the Right Way(tm)
* * Copyright (c) 2026 Kyle Givler
* Licensed under the MIT License.
*/
using DivineIntervention.Logging;
using System;
using System.Reflection;

namespace DivineIntervention.Patching
{
    /// <summary>
    /// A concrete implementation of <see cref="IHook"/> that handles type-safe casting and static method resolution.
    /// Includes robust exception handling to prevent mod-level crashes from halting the game loop.
    /// </summary>
    /// <typeparam name="T">The type that declares the method being patched.</typeparam>
    internal class GenericHook<T> : IHook
    {
        private readonly HookPrefix<T> _onPrefix;
        private readonly HookPostfix<T> _onPostfix;
        private readonly Func<bool> _condition;
        private readonly MethodBase _targetMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericHook{T}"/> class.
        /// </summary>
        /// <param name="method">The method being patched.</param>
        /// <param name="onPrefix">The prefix delegate to execute.</param>
        /// <param name="onPostfix">The postfix delegate to execute.</param>
        /// <param name="condition">An optional condition function. If it evaluates to false, the hook is ignored.</param>
        public GenericHook(MethodBase method, HookPrefix<T> onPrefix, HookPostfix<T> onPostfix, Func<bool> condition)
        {
            _targetMethod = method;
            _onPrefix = onPrefix;
            _onPostfix = onPostfix;
            _condition = condition;
        }

        /// <inheritdoc />
        public bool InvokePrefix(object[] args, object instance)
        {
            if (_condition != null && !_condition())
                return true;

            try
            {
                if (instance == null)
                {
                    return _onPrefix?.Invoke(default, args) ?? true;
                }
                else if (instance is T typedInstance)
                {
                    return _onPrefix?.Invoke(typedInstance, args) ?? true;
                }
            }
            catch (Exception ex)
            {
                DivineLog.Error($"[DivineIntervention] Exception in Prefix hook for {_targetMethod.Name}: {ex.Message}\n{ex.StackTrace}");
            }

            return true; // Default to 'true' so the game doesn't hang on a failed hook
        }

        /// <inheritdoc />
        public void InvokePostfix(object[] args, ref object result, object instance)
        {
            if (_condition != null && !_condition()) return;

            try
            {
                if (instance == null)
                {
                    _onPostfix?.Invoke(default, args, ref result);
                }
                else if (instance is T typedInstance)
                {
                    _onPostfix?.Invoke(typedInstance, args, ref result);
                }
            }
            catch (Exception ex)
            {
                DivineLog.Error($"[DivineIntervention] Exception in Postfix hook for {_targetMethod.Name}: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <inheritdoc />
        public void Unpatch() => HookDispatcher.Unregister(_targetMethod, this);
    }
}