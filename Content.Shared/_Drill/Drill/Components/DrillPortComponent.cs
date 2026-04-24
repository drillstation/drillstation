using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Drill.Drill.Components;

/// <summary>
/// Generic Drill input/output port
/// Used for anything every port needs to do
/// Specific input/output behavior is handled in discrete input/output components
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DrillPortComponent : Component
{
    /// <summary>
    /// Is this port in a valid position to work
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadOnly)]
    public bool IsValid = false;

    /// <summary>
    /// Is this port supplied with power
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadOnly)]
    public bool IsPowered = false;

    /// <summary>
    /// The type of adjacency check to the body this port should perform 
    /// </summary>
    [DataField]
    [ViewVariables]
    public adjacencyType Adjacency = adjacencyType.any;

    /// <summary>
    /// Power, in Watts, this port demands from the Drill
    /// </summary>
    [DataField]
    [ViewVariables]
    public uint PowerDemand = 600;
}

[Serializable, NetSerializable]
public enum adjacencyType : byte
{
    any,
    corner,
    edge
}
