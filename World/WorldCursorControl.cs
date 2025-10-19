using Godot;
using System;


public interface ISelection : INode, ISelectCircle
{
    public Vector3 GlobalPosition { get; set; }

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

    public int LocalPlayerID { get; set; }
    public Camera3D camera = null;

    [Signal]
    public delegate void DeselectEventHandler();

    Callable _SelectUnitCallable;

    Callable _AddUnitCallable;

    Callable _SelectTargetCallable;

    Callable _AddTargetCallable;

    public Callable OnGroundInputCallable { get; private set; }

    private static WorldCursorControl _instance;
    public static WorldCursorControl Instance
    {
        get
        {
            if (_instance == null) _instance = new WorldCursorControl();
            return _instance;
        }
        private set { _instance = value; }
    } 

    // public void ConnectToSelectUnit(IInputController controller)
    // {
    //     controller.InputController.SelectUnit -= _SelectUnit;
    //     controller.InputController.SelectUnit += _SelectUnit;
    // }

    // public void ConnectToDeselectUnit(IInputController controller)
    // {
    //     controller.InputController.DeselectUnit -= _DeselectUnit;
    //     controller.InputController.DeselectUnit += _DeselectUnit;
    // }

    // public void ConnectToAddUnit(IInputController controller)
    // {
    //     controller.InputController.AddUnit -= _AddUnit;
    //     controller.InputController.AddUnit += _AddUnit;
    // }

    // public void ConnectToSelectTarget(IInputController controller)
    // {
    //     controller.InputController.SelectTarget -= _SelectTarget;
    //     controller.InputController.SelectTarget += _SelectTarget;
    // }

    // public void ConnectToAddTarget(Node node)
    // {
    //     if (!node.IsConnected(InputController.SignalName.AddTarget, _AddTargetCallable))
    //         node.Connect(InputController.SignalName.AddTarget, _AddTargetCallable);
    // }

    void GetNodes()
    {
        select = GetNode<Select>("Select");
    }

    public override void _Ready()
    {
        _instance = this;
        GetNodes();
        _SelectUnitCallable = new Callable(this, nameof(_SelectUnit));
        _AddUnitCallable = new Callable(this, nameof(_AddUnit));
        _SelectTargetCallable = new Callable(this, nameof(_SelectTarget));
        //_SelectTargetCallable = new Callable(this, nameof(_AddTarget));
        OnGroundInputCallable = new Callable(this, nameof(_on_Ground_input_event));
        /*
        foreach(Node n in GetTree().GetNodesInGroup("Selectable")){
            n.Connect("SelectUnit", new Callable(this, nameof(_SelectUnit)));
        }
        foreach(Node n in GetTree().GetNodesInGroup("Targetable")){
            n.Connect("SelectTarget", new Callable(this, nameof(_SelectTarget)));
        }
        //GetNode<Ship>("/root/World/Ship").Connect("SelectUnit", this, nameof(_SelectUnit)); 
        //*/
    }

    public void _SelectUnit(ISelection unit)
    {
        select.SelectUnit(unit);
        //if (Logging) gameLogger.LogInfo("Selected node " + unit.Name);
        //GD.Print("Selected node " + unit.Name);
    }

    public void _DeselectUnit(ISelection unit)
    {
        select.DeselectUnit(unit);
        //if (Logging) gameLogger.LogInfo("Selected node " + unit.Name);
        //GD.Print("Selected node " + unit.Name);
    }

    public void _AddUnit(ISelection unit)
    {
        select.AddSelectedUnit(unit);
    }

    public void _SelectTarget(CollisionObject3D target)
    {
        ;
        select.AddTarget(target);
    }

    public void SetTask(CollisionObject3D target, CmdPanel.CmdPanelOption task)
    {
        ;
        select.AddTarget(target, task);
    }

    public Vector3 GetMouseWorldPosition()
    {
        var ray_length = 1000;
        var mousePos = GetViewport().GetMousePosition();
        var from = camera.ProjectRayOrigin(mousePos);
        var to = from + camera.ProjectRayNormal(mousePos) * ray_length;
        Vector3 p = Vector3.Zero;
        var space_state = GetWorld3D().DirectSpaceState;
        var state = space_state.IntersectRay(new PhysicsRayQueryParameters3D()
        {
            From = from,
            To = to
        });
        if (state.ContainsKey("position"))
        {
            p = (Vector3)state["position"];
        }
        return p;
    }

    void _on_Ground_input_event(Node camera, InputEvent inputEvent, Vector3 click_position, Vector3 click_normal, int shape_idx)
    {
        if (inputEvent is InputEventMouseButton button)
        {
            if (HasSelected())
            { // mouse
                if (button.ButtonIndex == MouseButton.Right)
                {   // right click
                    select.MoveToPosition(click_position);
                }
                if (button.ButtonIndex == MouseButton.Left && select != null && inputEvent.IsPressed())
                {    // left click
                    select.ClearSelection();
                    EmitSignal(nameof(SignalName.Deselect));
                }
            }
        }
    }

    public bool HasSelected()
    {
        return select.HasSelected();
    }

    public void ClearSelection()
    {
        select.ClearSelection();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
