namespace Quartermaster.Ipc.Capabilities;

/// <summary>
/// State types for AutoRetainer capability.
/// </summary>
public class RetainerState
{
    public string Name { get; set; } = string.Empty;
    public int VentureTypeId { get; set; }
    public DateTimeOffset? VentureCompleteAt { get; set; }
}

public class SubmarineState
{
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public DateTimeOffset? ReturnAt { get; set; }
}

/// <summary>
/// Read ventures, suppress retainer during QM-managed sequences, submarine state.
/// Failures are caught and logged — never propagated up.
/// </summary>
public class AutoRetainerCapability
{
    /// <summary>
    /// Suppress AutoRetainer from firing during a QM-managed sequence.
    /// </summary>
    public void Suppress(bool suppressed)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Read all current offline character/retainer data.
    /// </summary>
    public IReadOnlyList<RetainerState> GetRetainerStates()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Read all submarine states.
    /// </summary>
    public IReadOnlyList<SubmarineState> GetSubmarineStates()
    {
        throw new NotImplementedException();
    }
}
