using Robust.Shared.Audio;
using Robust.Shared.GameStates;

using Content.Shared.EntityTable.EntitySelectors;

namespace Content.Shared._Drill.Drill.Components;

/// <summary>
/// A Drill port outputting ore items
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DrillPortOutputOreComponent : DrillPortOutputBaseComponent
{
    /// <summary>
    /// Sound to play when the port performs an output
    /// </summary>
    [DataField]
    [ViewVariables(VVAccess.ReadOnly)]
    public EntityTableSelector? SpawnTable = default!;
}
