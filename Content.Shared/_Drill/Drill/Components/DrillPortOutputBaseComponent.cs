using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Timing;

namespace Content.Shared._Drill.Drill.Components;

/// <summary>
/// Abstract base for an output port
/// Used for intervals and activity states
/// </summary>
public abstract partial class DrillPortOutputBaseComponent : Component
{
    /// <summary>
    /// The output interval
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan OutputInterval = TimeSpan.FromSeconds(10);

    /// <summary>
    /// When the next output should happen
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NextOutput;

    /// <summary>
    /// Is this port currently doing work
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public bool IsActive = false;

    /// <summary>
    /// Sound to play when the port performs an output
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? OutputSound = default!;
}
