namespace Content.Server._Drill.Drill;

/// <summary>
/// The body of the Drill
/// </summary>

[RegisterComponent]
public sealed partial class DrillBodyComponent : Component
{
    /// <summary>
    /// Does this body tile count as a core
    /// </summary>
    [ViewVariables]
    public bool isCore = false;
}
