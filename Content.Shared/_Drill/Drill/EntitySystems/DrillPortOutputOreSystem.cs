using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;

using Content.Shared._Drill.Drill.Components;
using Content.Shared.EntityTable;

namespace Content.Shared._Drill.Drill.EntitySystems;

public sealed partial class DrillPortOutputOreSystem : EntitySystem
{

    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly EntityTableSystem _entityTable = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        var query = EntityQueryEnumerator<DrillPortOutputOreComponent>();
        while (query.MoveNext(out var uid, out var port))
        {
            if ((curTime < port.NextOutput) | (!port.IsActive))
                continue;

            port.NextOutput = curTime + port.OutputInterval;

            var xform = Transform(uid);
            var spawns = _entityTable.GetSpawns(port.SpawnTable);
            foreach (var id in spawns)
            {
                PredictedSpawnAtPosition(id, xform.Coordinates);
            }

            _audio.PlayPvs(port.OutputSound, uid);
        }
    }

    public void SetActive(EntityUid uid, bool value, DrillPortOutputOreComponent? port = null)
    {
        if (!Resolve(uid, ref port))
            return;
        if (value == port.IsActive)
            return;

        port.IsActive = value;

        Dirty(uid, port);
    }

}
