using Robust.Shared.GameObjects;

using Content.Shared._Drill.Drill.Components;

namespace Content.Shared._Drill.Drill.EntitySystems;

public sealed class DrillPortSystem : EntitySystem
{
    public void SetValid(EntityUid uid, bool value, DrillPortComponent? port = null)
    {
        if (!Resolve(uid, ref port))
            return;
        if (value == port.IsValid)
            return;

        port.IsValid = value;

        Dirty(uid, port);
    }

    public void SetPowered(EntityUid uid, bool value, DrillPortComponent? port = null)
    {
        if (!Resolve(uid, ref port))
            return;
        if (value == port.IsPowered)
            return;

        port.IsPowered = value;

        Dirty(uid, port);
    }
/*
    public void SetActive(EntityUid uid, bool value, DrillPortComponent? port = null)
    {
        if (!Resolve(uid, ref port))
            return;
        if (value == port.IsActive)
            return;

        port.IsActive = value;

        Dirty(uid, port);
    }
*/
}
