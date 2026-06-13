using DivineIntervention.Patching;
using System;
using System.Reflection;

public class GenericHook<T> : IHook
{
    private readonly Func<T, bool> _onPrefix;
    private readonly Action<T, object> _onPostfix;
    private readonly Func<bool> _condition;
    private readonly MethodBase _targetMethod;

    /// <summary>
    /// A concrete implementation of <see cref="IHook"/> that handles type-safe casting.
    /// </summary>
    public GenericHook(
        MethodBase method,
        Func<T, bool> onPrefix,
        Action<T, object> onPostfix,
        Func<bool> condition)
    {
        _targetMethod = method;
        _onPrefix = onPrefix;
        _onPostfix = onPostfix;
        _condition = condition;
    }

    /// <inheritdoc />
    public bool InvokePrefix(object[] args, object instance)
    {
        // If condition fails, we assume "True" (let the original method run)
        if (_condition != null && !_condition())
            return true;

        if (instance is T typedInstance)
        {
            if (instance is null)
                return true;

            // Execute the logic and return the result
            return _onPrefix(typedInstance);
        }

        return true; // Default: let original run
    }

    public void InvokePostfix(object[] args, ref object result, object instance)
    {
        if (_condition != null && !_condition()) return;
        if (instance is T typedInstance)
        {
            _onPostfix?.Invoke(typedInstance, result);
        }
    }

    /// <inheritdoc />
    public void Unpatch() =>
        HookDispatcher.Unregister(_targetMethod, this);
}