using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public interface ISelection : INode3D, ISelectCircle
{
    public OrderQueue OrderQueue { get; set; }
    public void MoveToPosition(Godot.Vector3 position);
    public void MoveToTarget(OrderQueue.Target target);
    public void ClearTargets();
}

public partial class WorldCursorControl : Node3D
{

    GameLogger gameLogger = GameLogger.Instance;
    // public Ship Attacker { get; set; } = null;

    [Export]
    public bool Logging { get; set; } = true;

    Select select = null;

    public Select Select { get => select; set => select = value; }

    public int LocalPlayerID { get; set; }
    public Camera3D camera = null;

    bool Drag = false;

    [Signal]
    public delegate void DeselectEventHandler();

    Callable _SelectUnitCallable;

    Callable _AddUnitCallable;

    Callable _SelectTargetCallable;

    Callable _AddTargetCallable;

    public Vector3 MousePosition { get; set; } = Vector3.Zero;

    public Vector3 MousePositionRounded { get {return new Vector3(Mathf.Round(MousePosition.X), Mathf.Round(MousePosition.Y), Mathf.Round(MousePosition.Z));} }

    public Callable OnGroundInputCallable { get; private set; }

    HashSet<Node> Markers { get; set; } = new HashSet<Node>();

    private static WorldCursorControl _instance;
    public static WorldCursorControl Instance
    {
        get
        {
            return _instance;
        }
        private set { _instance = value; }
    } 

    PhysicsRayQueryParameters3D _querry = new PhysicsRayQueryParameters3D()
    {
        CollideWithBodies = true,
        CollideWithAreas = true
    };
    
    [Export]
    public float ray_length { get; set; } = 1000f;

    void GetNodes()
    {
        Select = GetNode<Select>("Select");
    }

    public void OnDrag()
    {
        Drag = !Drag;
    }

    public override void _Ready()
    {
        _instance = this;
        GetNodes();
        _SelectUnitCallable = new Callable(this, nameof(_SelectUnit));
        _AddUnitCallable = new Callable(this, nameof(_AddUnit));
        _SelectTargetCallable = new Callable(this, nameof(_SelectTarget));
        OnGroundInputCallable = new Callable(this, nameof(_on_Ground_input_event));
    }

    
    public void _SelectUnit(ISelection unit)
    {
        Select.SelectUnit(unit);
        //SpawnMarkers(unit);
        //if (Logging) gameLogger.LogInfo("Selected node " + unit.Name);
        //GD.Print("Selected node " + unit.Name);
    }

    void SpawnMarkers(ISelection unit)
    {

        if(unit.OrderQueue != null)
        {
            Vector3 target = unit.Position;
            foreach (var item in unit.OrderQueue.Targets)
            {
                var mesh = new Line3d();
                mesh.LineWithPoint(target, item.Point);
                AddChild(mesh);
                Markers.Add(mesh);
                target = item.Point;
            }
        }
    }

    void SpawnMarkers()
    {
        ClearMarkers();
        foreach (var item in Select.SelectedUnits)
            SpawnMarkers(item);
    }

    void ClearMarkers()
    {
        foreach (var item in Markers)
        {
            item.QueueFree();
        }
        Markers.Clear();
    }

    public void _DeselectUnit(ISelection unit)
    {
        Select.DeselectUnit(unit);
        //ClearMarkers();
        //if (Logging) gameLogger.LogInfo("Selected node " + unit.Name);
        //GD.Print("Selected node " + unit.Name);
    }

    public void _AddUnit(ISelection unit)
    {
        Select.AddSelectedUnit(unit);
        //SpawnMarkers(unit);
    }

    public void _SelectTarget(INode3D target)
    {
        Select.AddTarget(target);
    }

    public void SetTask(INode3D target, CmdPanel.CmdPanelOption task)
    {
        ;
        Select.AddTarget(target, task);
    }

    public Vector3 GetMouseWorldPosition()
    {
        return Tools.GetMouseWorldPosition(this, camera, _querry, ray_length);
    }

    void _on_Ground_input_event(Node camera, InputEvent inputEvent, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        //MousePosition = click_position;
        if (inputEvent is InputEventMouseButton button)
        {
            if (HasSelected())
            { // mouse
                if (button.ButtonIndex == MouseButton.Right)
                {   // right click
                    Select.MoveToPosition(click_position);
                    //SpawnMarkers();
                }
                if (button.ButtonIndex == MouseButton.Left && Select != null && inputEvent.IsPressed()) // && inputEvent.IsReleased()
                {    // left click
                    Select.ClearSelection();
                    EmitSignal(nameof(SignalName.Deselect));
                }
            }
        }
    }

    // void _on_Mouse_input_event(Node camera, InputEvent inputEvent, Vector3 click_position, Vector3 click_normal, int shape_idx)
    // {
    //     MousePosition = click_position;
    // }

    public bool HasSelected()
    {
        return Select.HasSelected();
    }

    public void ClearSelection()
    {
        Select.ClearSelection();
    }

    internal void ConnectUnit3D(Unit3D unit3D)
    {
        throw new NotImplementedException();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
     public override void _Process(double delta)
     {
         MousePosition = GetMouseWorldPosition();
     }
}
