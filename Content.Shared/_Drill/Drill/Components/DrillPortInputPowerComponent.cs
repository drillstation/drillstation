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
    /// Ergo the maximum drawRate that will be set on the <see cref=PowerConsumerComponent>
    /// </summary>
    [DataField("maximumDraw")]
    [ViewVariables(VVAccess.ReadOnly)]
    public uint MaximumDraw = 2000;
}
