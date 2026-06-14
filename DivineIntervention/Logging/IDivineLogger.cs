namespace DivineIntervention.Logging;

/// <summary>
/// Defines the target endpoint for formatted logs. 
/// Decouples the log-formatting engine from Unity's native ECall methods.
/// </summary>
public interface ILogTarget
{
    void WriteMessage(string formattedMessage);
    void WriteWarning(string formattedMessage);
    void WriteError(string formattedMessage);
}

/// <summary>
/// Default production target that hooks straight into RimWorld's core engine.
/// </summary>
public class RimWorldLogTarget : ILogTarget
{
    public void WriteMessage(string formattedMessage) =>
        Verse.Log.Message(formattedMessage);
    public void WriteWarning(string formattedMessage) =>
        Verse.Log.Warning(formattedMessage);
    public void WriteError(string formattedMessage) =>
        Verse.Log.Error(formattedMessage);
}