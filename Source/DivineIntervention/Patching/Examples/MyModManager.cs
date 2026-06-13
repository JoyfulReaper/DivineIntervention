using DivineIntervention.Logging;
using DivineIntervention.Patching;
using Verse;

namespace DivineIntervention.DivineIntervention.Patching.Examples;

public class MyModManager
{
    private IHook _myTickHook;

    public void EnableFeature()
    {
        // Store the hook in a class member variable
        _myTickHook = HookFactory.Create<Pawn>(
            "Tick",
            (pawn) => DivineLog.Info("Feature is active!")
        );
    }

    public void DisableFeature()
    {
        // The framework handles the lookup and removes it from the dispatcher
        // safely without affecting other mods' patches on the same method.
        _myTickHook?.Unpatch();
        _myTickHook = null;
    }
}