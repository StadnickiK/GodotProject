using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DrawingLines3DController : Node3D
{
    [Export]
    public string ScenePath { get; set; } = ScenePaths.Instance.Line3DPath;

    public bool DrawVelocity { get; set; } = true;

    public delegate void SpawnOrderMarkersHandler(IMovable orderQueue);

    // public delegate void RedrawOrderMarkersHandler(MeshInstance3D lineMesh, Vector3 start, Vector3 end);

    [Export]
    Godot.Collections.Dictionary<string, Material> LineMaterials = new Godot.Collections.Dictionary<string, Material>();

    Dictionary<IMovable, HashSet<MeshInstance3D>> OrderMarkers = new Dictionary<IMovable, HashSet<MeshInstance3D>>();

    public bool DrawForces { get; set; } = true;

    Line3d line3D;

    PackedScene packedScene;

    public override void _Ready()
    {
        packedScene = ResourceLoader.Load<PackedScene>(ScenePath);
        line3D = GetNode<Line3d>("Line3D");
    }

    public void _on_SpawnMarkers(IMovable unit)
    {
        if(OrderMarkers.ContainsKey(unit))
            RespawnMarkers(unit);
        else
            SpawnMarkers(unit);
    }

    public void _on_ClearMarkers(IMovable unit)
    {
        if(OrderMarkers.ContainsKey(unit))
            ClearMarkers(unit);
    }

    void AddMarkers(IMovable unit, HashSet<MeshInstance3D> Markers)
    {
        if(!OrderMarkers.ContainsKey(unit))
            OrderMarkers.Add(unit, Markers);
        else
            OrderMarkers[unit].UnionWith(Markers);
    }

    void AddMarkers(IMovable unit, MeshInstance3D Marker)
    {
        if(!OrderMarkers.ContainsKey(unit))
            OrderMarkers.Add(unit, new HashSet<MeshInstance3D>(){Marker});
        else
            OrderMarkers[unit].Add(Marker);
    }

    void SpawnMarkers(IMovable unit)
    {

        if(unit.OrderQueue != null)
            SpawnNewMarkers(unit);
        if(DrawForces && unit.VelocityController != null)
            DrawNewForces(unit);
    }

    void SpawnNewMarkers(IMovable unit)
    {
        Vector3 target = unit.Position;
        var Markers = new HashSet<MeshInstance3D>();
        foreach (var item in unit.OrderQueue.Targets)
        {
            var mesh = line3D.LineWithPoint(target, item.Point);
            Markers.UnionWith(mesh);
            target = item.Point;
        }
        AddMarkers(unit, Markers);
    }

    void SpawnNewMarkers(IMovable unit, Vector3 end)
    {
        var mesh = line3D.LineWithPoint(unit.Position, end);
        AddMarkers(unit, mesh);
    }

    void DrawNewForces(IMovable unit)
    {
        if(unit.Velocity != Vector3.Zero)
            DrawForce(unit, unit.Velocity, LineMaterials[GlobalStatNames.VelocityColor]);
        // if(unit is Unit3D unit3D)
        // {
        //     var pos = unit3D.NavAgent3D.GetNextPathPosition();
        //     SpawnNewMarkers(unit3D, pos);
        // }
            
        DrawForce(unit, unit.VelocityController.SeparationForce * 5, LineMaterials[GlobalStatNames.SeparationColor]);
        DrawForce(unit, unit.VelocityController.AligmentForce, LineMaterials[GlobalStatNames.AligmentColor]);
        DrawForce(unit, unit.VelocityController.AvoidanceForce, LineMaterials[GlobalStatNames.AvoidanceColor]);
    }

    void DrawForce(IMovable unit, Vector3 force, Material material)
    {
        var Markers = line3D.DrawBox(material, unit.Position, unit.Position + force);
        AddMarkers(unit, Markers);
    }

    void RespawnMarkers(IMovable unit)
    {
        ClearMarkers(unit);
        SpawnNewMarkers(unit); 
        if(DrawForces && unit.VelocityController != null) DrawNewForces(unit);
    }

    void ClearMarkers(IMovable unit)
    {
        foreach (var item in OrderMarkers[unit])
        {
            item.QueueFree();
        }
        OrderMarkers.Remove(unit);
    }

    public void ConnectUnit3D(IUnit3D unit3D)
    {
        unit3D.SpawnMarkers -= _on_SpawnMarkers;
        unit3D.SpawnMarkers += _on_SpawnMarkers;
        unit3D.ClearMarkers -= _on_ClearMarkers;
        unit3D.ClearMarkers += _on_ClearMarkers;
    }

}
