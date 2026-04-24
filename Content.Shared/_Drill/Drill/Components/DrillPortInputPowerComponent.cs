namespace Content.Shared._Drill.Drill.Components;

/// <summary>
/// Drill power input component
/// Draws power for the drill limited by a maximum wattage
/// </summary>
[RegisterComponent]
public sealed partial class DrillPortInputPowerComponent : Component
{
    /// <summary>
    /// Maximum draw rate for this port
    /// </summary>
    [DataField("maximumDraw")]
    [ViewVariables(VVAccess.ReadOnly)]
    public uint MaximumDraw = 2000;
}
