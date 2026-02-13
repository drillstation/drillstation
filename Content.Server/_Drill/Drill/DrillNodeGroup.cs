using System.Linq;

//using Robust.Shared.Utility;
using Robust.Server.GameObjects;
using Robust.Shared.Map.Components;

using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.NodeContainer;
using Content.Shared.NodeContainer.NodeGroups;

using Content.Server._Drill.Drill.EntitySystems;
using Content.Shared._Drill.Drill.Components;

namespace Content.Server._Drill.Drill;

/// <summary>
/// Node group class for the Drill assembly
/// </summary>
[NodeGroup(NodeGroupID.DrillAssembly)]
public sealed class DrillNodeGroup : BaseNodeGroup
{
    [Dependency] private readonly ILogManager _logManager = default!; // tmp
    [Dependency] private readonly IEntityManager _entMan = default!;

    private ISawmill _sawmill = default!; // tmp

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

    
    public override void Initialize(Node sourceNode, IEntityManager entMan) // tmp
    {
        base.Initialize(sourceNode, entMan);
        _sawmill = _logManager.GetSawmill("nodegroup"); // tmp

        _sawmill.Debug("Drill Nodegroup is initialized"); // tmp
    }

    public override void LoadNodes(List<Node> groupNodes)
    {
        base.LoadNodes(groupNodes);
        _sawmill.Debug("Drill Nodegroup is loading nodes"); // tmp

        EntityUid? gridEnt = null;

        // don't think handling body and port logic at the same time is
        // a good or sustainable idea, but i don't see another way right now

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

            _sawmill.Debug($"Drill body node {nodeOwner} : {body.IsCore}"); // tmp

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

            var nodeNeighbors = mapSystem.GetCellsInSquareArea(xform.GridUid.Value, grid, xform.Coordinates, 1)
                .Where(entity => entity != nodeOwner && bodyQuery.HasComponent(entity));

            // perform port adjacency check
            // TODO: this needs *proper logic* for checking adjacency
            // currently this is a simplistic check for how many adjacent
            // body tiles there are, which is correct in like 20% of cases at best
            // in the interest of time and conscious of my skill level i opted not to
            // copy IconSmooth logic or do anything sophisticated
            switch (port.Adjacency) // i am going to code duplication hell
            {
                case adjacencyType.any:
                    if (nodeNeighbors.Count() >= 1)
                    {
                        portSystem.SetValid(nodeOwner, true, port);
                    }
                    else
                    {
                        portSystem.SetValid(nodeOwner, false, port);
                    }
                    break;
                case adjacencyType.corner:
                    if (nodeNeighbors.Count() == 2)
                    {
                        portSystem.SetValid(nodeOwner, true, port);
                    }
                    else
                    {
                        portSystem.SetValid(nodeOwner, false, port);
                    }
                    break;
                case adjacencyType.edge:
                    if (nodeNeighbors.Count() >= 3)
                    {
                        portSystem.SetValid(nodeOwner, true, port);
                    }
                    else
                    {
                        portSystem.SetValid(nodeOwner, false, port);
                    }
                    break;
            }


            _sawmill.Debug($"Drill port node {nodeOwner} : {port.IsValid}"); // tmp
        }

        foreach (var node in groupNodes) // loop thru looking for the computer
        { // there must be some better way to do this
            var nodeOwner = node.Owner;
            if (!controllerQuery.TryGetComponent(nodeOwner, out var controller))
                continue;

            if (_masterController == null)
                _masterController = nodeOwner;

            _sawmill.Debug($"Drill computer node {nodeOwner}"); // tmp
        }

        _sawmill.Debug($"Drill controller {_masterController}"); // tmp
        _sawmill.Debug($"Drill cores {CoreCount}"); // tmp
    }
}
