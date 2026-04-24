using System.Linq;

using Robust.Shared.Map.Components;
using Robust.Server.GameObjects;

using Content.Shared.NodeContainer;
using Content.Shared.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.NodeContainer.NodeGroups;

using Content.Shared._Drill.Drill.Components;
using Content.Shared._Drill.Drill.EntitySystems;

namespace Content.Server._Drill.Drill;

/// <summary>
/// Node group class for the Drill assembly
/// </summary>
[NodeGroup(NodeGroupID.DrillAssembly)]
public sealed class DrillNodeGroup : BaseNodeGroup
{
    [Dependency] private readonly IEntityManager _entMan = default!;

    /// <summary>
    /// The computer control module connected to the assembly
    /// </summary>
    [ViewVariables]
    private EntityUid? _masterController;

    public EntityUid? MasterController => _masterController;

    /// <summary>
    /// The set of body tiles that currently count as cores
    /// </summary>
    private readonly List<EntityUid> _cores = new();

    [ViewVariables(VVAccess.ReadOnly)]
    public int CoreCount => _cores.Count;

    /// <summary>
    /// Iterate through all connected nodes and set flags on their components
    /// </summary>
    /// <remarks>
    /// Currently this is using 3 separate foreach loops for body, port, and controller logic
    /// And port adjacency is a rudimentary placeholder with a simple switch statement
    /// </remarks>
    public override void LoadNodes(List<Node> groupNodes)
    {
        base.LoadNodes(groupNodes);

        EntityUid? gridEnt = null;

        // query systems
        var bodySystem = _entMan.System<DrillBodySystem>();
        var portSystem = _entMan.System<DrillPortSystem>();
        var mapSystem = _entMan.System<MapSystem>();

        // query components
        var bodyQuery = _entMan.GetEntityQuery<DrillBodyComponent>();
        var portQuery = _entMan.GetEntityQuery<DrillPortComponent>();
        var controllerQuery = _entMan.GetEntityQuery<DrillPortComputerComponent>();
        var xformQuery = _entMan.GetEntityQuery<TransformComponent>();

        foreach (var node in groupNodes) // loop thru looking for body
        {
            var nodeOwner = node.Owner;
            // get relevant components
            if (!bodyQuery.TryGetComponent(nodeOwner, out var body))
                continue;
            if (!xformQuery.TryGetComponent(nodeOwner, out var xform))
                continue;
            if (!_entMan.TryGetComponent(xform.GridUid, out MapGridComponent? grid))
                continue;

            // get grid node is on
            if (gridEnt == null)
                gridEnt = xform.GridUid;
            else if (gridEnt != xform.GridUid)
                continue;

            // get neighboring body tiles
            var nodeNeighbors = mapSystem.GetCellsInSquareArea(xform.GridUid.Value, grid, xform.Coordinates, 1)
                .Where(entity => entity != nodeOwner && bodyQuery.HasComponent(entity));

            if (nodeNeighbors.Count() >= 8)
            {
                _cores.Add(nodeOwner);
                bodySystem.SetCore(nodeOwner, true, body);
                // Core visuals will be updated later.
            }
            else
            {
                bodySystem.SetCore(nodeOwner, false, body);
            }

        }

        foreach (var node in groupNodes) // loop thru looking for port
        { // hella code duplication but whatever
            var nodeOwner = node.Owner;
            if (!portQuery.TryGetComponent(nodeOwner, out var port))
                continue;
            if (!xformQuery.TryGetComponent(nodeOwner, out var xform))
                continue;
            if (!_entMan.TryGetComponent(xform.GridUid, out MapGridComponent? grid))
                continue;

            if (gridEnt == null)
                gridEnt = xform.GridUid;
            else if (gridEnt != xform.GridUid)
                continue;

            // get neighboring body tiles - checking if the port is on an edge or corner
            var nodeNeighbors = mapSystem.GetCellsInSquareArea(xform.GridUid.Value, grid, xform.Coordinates, 1)
                .Where(entity => entity != nodeOwner && bodyQuery.HasComponent(entity));

            // perform port adjacency check
            // TODO: this needs *proper logic* for checking adjacency
            // currently this is a simplistic check for how many adjacent
            // body tiles there are, which is correct in like 20% of cases at best
            // in the interest of time and conscious of my skill level i opted not to
            // copy IconSmooth logic or do anything sophisticated
            bool adjCheck = false;
            switch (port.Adjacency) // i am going to code duplication hell
            {
                case adjacencyType.any:
                    adjCheck = (nodeNeighbors.Count() >= 1);
                    break;
                case adjacencyType.corner:
                    adjCheck = (nodeNeighbors.Count() == 2);
                    break;
                case adjacencyType.edge:
                    adjCheck = (nodeNeighbors.Count() >= 3);
                    break;
            }
            portSystem.SetValid(nodeOwner, adjCheck, port);

        }

        foreach (var node in groupNodes) // loop thru looking for the computer
        { // there must be some better way to do this
            var nodeOwner = node.Owner;
            if (!controllerQuery.TryGetComponent(nodeOwner, out var controller))
                continue;

            if (_masterController == null)
                _masterController = nodeOwner;
        }
    }
}
