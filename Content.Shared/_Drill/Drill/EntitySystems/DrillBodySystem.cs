using Robust.Shared.GameObjects;

using Content.Shared._Drill.Drill.Components;

namespace Content.Shared._Drill.Drill.EntitySystems;

public sealed class DrillBodySystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;

    public void SetCore(EntityUid uid, bool value, DrillBodyComponent? body = null)
    {
        if (!Resolve(uid, ref body))
            return;
        if (value == body.IsCore)
            return;

        body.IsCore = value;

        Dirty(uid, body);

        _appearanceSystem.SetData(uid, DrillBodyVisuals.Core, value);
    }
}
