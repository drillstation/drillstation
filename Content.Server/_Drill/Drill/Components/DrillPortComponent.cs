namespace Content.Server._Drill.Drill;

/// <summary>
/// Generic Drill input/output port
/// Used for anything every port needs to do
/// Specific input/output behavior is handled in discrete input/output components
/// </summary>
[RegisterComponent]
public sealed partial class DrillPortComponent : Component
{
    /// <summary>
    /// Is this port in a valid position to work
    /// </summary>
    [ViewVariables]
    public bool isActive = false;

    /// <summary>
    /// The type of adjacency check to the body this port should perform 
    /// </summary>
    [ViewVariables]
    public adjacencyType adjacency = adjacencyType.any;
}

private enum adjacencyType
{
    any,
    corner,
    edge
}
