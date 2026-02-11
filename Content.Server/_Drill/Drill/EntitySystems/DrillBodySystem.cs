using Content.Shared._Drill.Drill.Components;
using Robust.Server.GameObjects;

namespace Content.Server._Drill.Drill.EntitySystems;

public sealed class DrillBodySystem : EntitySystem
{
    [Dependency] private readonly AppearanceSystem _appearanceSystem = default!;
/*
    public override void Initialize()
    {
        SubscribeLocalEvent<DrillBodyComponent, ComponentInit>(OnBodyInit);
    }
*/
    public void SetCore(EntityUid uid, bool value, DrillBodyComponent? body = null)
    {
        if (!Resolve(uid, ref body))
            return;
        if (value == body.IsCore)
            return;

        body.IsCore = value;
        _appearanceSystem.SetData(uid, DrillBodyVisuals.Core, value);
    }
/*
    public void OnBodyInit(EntityUid uid, DrillBodyComponent component, ComponentInit args)
    {
        SetCore(uid, true, component);
    }
*/
}
