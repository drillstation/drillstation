using Content.Shared._Drill.Drill.Components;
using Robust.Server.GameObjects;

namespace Content.Server._Drill.Drill.EntitySystems;

public sealed class DrillPortSystem : EntitySystem
{
    public void SetValid(EntityUid uid, bool value, DrillPortComponent? port = null)
    {
        if (!Resolve(uid, ref port))
            return;
        if (value == port.IsValid)
            return;

        port.IsValid = value;
    }
}
