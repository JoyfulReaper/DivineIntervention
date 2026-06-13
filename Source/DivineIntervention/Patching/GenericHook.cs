using DivineIntervention.Patching;
using System;
using System.Reflection;

public class GenericHook<T> : IHook
{
    private readonly Action<T> _onPrefix;
    private readonly Func<bool> _condition;
    private readonly MethodBase _targetMethod; // Store the method

    /// <summary>
    /// A concrete implementation of <see cref="IHook"/> that handles type-safe casting.
    /// </summary>
    public GenericHook(MethodBase method, Action<T> onPrefix, Func<bool> condition)
    {
        _targetMethod = method;
        _onPrefix = onPrefix;
        _condition = condition;
    }

    /// <inheritdoc />
    public void InvokePrefix(object[] args, object instance)
    {
        if (_condition != null && !_condition()) return;
        if (instance is T typedInstance) _onPrefix(typedInstance);
    }

    /// <inheritdoc />
    public void Unpatch()
    {
        HookDispatcher.Unregister(_targetMethod, this);
    }
}