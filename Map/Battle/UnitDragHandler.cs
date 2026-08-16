using Godot;
using System;
using System.Collections.Generic;

public partial class UnitDragHandler : Node
{
    public WorldCursorControl WorldCursorControl { get; set; }

    Dictionary<IMovable, Vector3> MeshInstances = new();

    /// <summary>
    /// Contains mouse offset limits
    /// x - minX, y - maxX , z - minZ, w - maxZ
    /// </summary>
    Vector4 OffsetLimits = new Vector4(float.MaxValue,float.MinValue,float.MaxValue,float.MinValue);

    /// <summary>
    /// Contains deployment zone bondary box where
    /// x - minX, y - maxX , z - minZ, w - maxZ
    /// </summary>
    Vector4 DeployZoneLimits = Vector4.Zero;
    [Export]
    public Material MeshMaterial { get; set; }

    public bool Drag { get; private set; }

    public void OnDrag()
    {
        if(!Drag){
            foreach (var item in WorldCursorControl.Select.SelectedUnits)
            {
                if(item is IMovable movable)
                {
                    CreateMeshFromMovable(movable);
                    CalculateMaxOffsets(MeshInstances[movable]);
                }
            }
        }
        GD.Print("On drag");
        Drag = true;
    }

    void CreateMeshFromMovable(IMovable movable)
    {

        // GD.Print("Mesh pos "+ mesh.Position +"  item pos" + movable.Position);
        // GD.Print("Mesh pos "+ mesh.Position +"  item pos" + movable.Position);
        var offset = movable.Position - WorldCursorControl.Instance.MousePosition;
        offset.Y *= 0;
        MeshInstances[movable] = offset;
    }

    public void UpdateDeploymentZoneLimits(Vector3 Position, Vector3 Size)
    {
        // calculate bondary box
        var minX = Position.X - Size.X / 2;
        var maxX = Position.X + Size.X / 2;

        var minZ = Position.Z - Size.Z / 2;
        var maxZ = Position.Z + Size.Z / 2;

        DeployZoneLimits = new Vector4(minX, maxX, minZ, maxZ);
    }

    void CalculateMaxOffsets(Vector3 mouseOffset)
    {
        // Find the limits of the whole group relative to the mouse
        OffsetLimits.X = Mathf.Min(OffsetLimits.X, mouseOffset.X);
        OffsetLimits.Y = Mathf.Max(OffsetLimits.Y, mouseOffset.X);

        OffsetLimits.Z = Mathf.Min(OffsetLimits.Z, mouseOffset.Z);
        OffsetLimits.W = Mathf.Max(OffsetLimits.W, mouseOffset.Z);
    }

    void UpdateMeshes(Vector3 mousePosition)
    {
        // Clamp X independently
        mousePosition.X = Mathf.Clamp(
            mousePosition.X,
            DeployZoneLimits.X - OffsetLimits.X,
            DeployZoneLimits.Y - OffsetLimits.Y
        );

        // Clamp Z independently
        mousePosition.Z = Mathf.Clamp(
            mousePosition.Z,
            DeployZoneLimits.Z - OffsetLimits.Z,
            DeployZoneLimits.W - OffsetLimits.W
        );

        foreach (var item in MeshInstances)
        {
            var pos = new Vector3(
                mousePosition.X,
                item.Key.Position.Y,
                mousePosition.Z
            ) + item.Value;

            item.Key.Position = pos;
        }
    }


    public override void _Input(InputEvent inputEvent){
        // if(inputEvent is InputEventKey key){
        //     KeyboardAction(key);
        // }
        if(inputEvent is InputEventMouseButton button){
            if(button.ButtonIndex == MouseButton.Left && inputEvent.IsReleased() && Drag){
                Drag = false;
                OffsetLimits.X = float.MaxValue;
                OffsetLimits.Y = float.MinValue;
                OffsetLimits.Z = float.MaxValue;
                OffsetLimits.W = float.MinValue;
                MeshInstances.Clear();
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Drag)
        {
            var p = WorldCursorControl.Instance.MousePosition;
            if(p == Vector3.Zero)
                p = WorldCursorControl.Instance.GetMouseWorldPosition();
            UpdateMeshes(p);
        }
    }

}
