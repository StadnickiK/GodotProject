using Godot;
using System;
using System.Diagnostics;

public interface ILeftClickAction
{
  public void LeftClickAction();
}

public interface IRightClickAction
{
  public void RightClickAction();
}

public partial class InputController : Node
{
  public delegate void MoveEventHandler(ISelection movable);

  public delegate void TargetEventHandler(CollisionObject3D movable);

  public event MoveEventHandler SelectUnit;

  public event MoveEventHandler DeselectUnit;

  public event TargetEventHandler SelectTarget;

  public event MoveEventHandler AddUnit;

  public event TargetEventHandler AddTarget;


  [Export]
  public InputMode Mode { get; set; } = InputMode.Select;

  [Export]
  public int DragSpeed { get; set; } = 10;

  bool DoDrag = false;

  bool MultiSelect = false;

  CollisionObject3D rigidBody3DParent;

  IRightClickAction RightClickAction;

  public enum InputMode
  {
    Select,
    Drag
  }

  public void ChangeMode(InputMode mode)
  {
    Mode = mode;
  }


  public override void _Ready()
  {
    try
    {
      rigidBody3DParent = (CollisionObject3D)GetParent();
      rigidBody3DParent.Connect(CollisionObject3D.SignalName.InputEvent, new Callable(this, nameof(_on_input_event)));
      //RightClickAction = GetNode<MoveToTargetAction>("RightClickAction");
      var s = rigidBody3DParent.GetNodeOrNull<Shield>("Shield");
      s?.Connect(CollisionObject3D.SignalName.InputEvent, new Callable(this, nameof(_on_input_event)));
    }
    catch (System.Exception ex)
    {
      throw new Exception("Parent should be RigidBody3D " + ex.Message);
    }
    if(rigidBody3DParent is ISelection selection)
    {
      SelectUnit -= World.Instance.Select;
      SelectUnit += World.Instance.Select;
    }
    DeselectUnit -= WorldCursorControl.Instance._DeselectUnit;
    DeselectUnit += WorldCursorControl.Instance._DeselectUnit;
    AddUnit -= WorldCursorControl.Instance._AddUnit;
    AddUnit += WorldCursorControl.Instance._AddUnit;
    SelectTarget -= WorldCursorControl.Instance._SelectTarget;
    SelectTarget += WorldCursorControl.Instance._SelectTarget;
  }

  public override void _Input(InputEvent inputEvent)
  {
    if (inputEvent is InputEventKey key && inputEvent.IsPressed())
    {
      switch (key.Keycode)
      {
        case Key.Shift:
          MultiSelect = true;
          break;
      }
    }
    else
    {
      MultiSelect = false;
    }
  }


  void _on_input_event(Node camera, InputEvent inputEvent, Vector3 click_position, Vector3 click_normal, int shape_idx)
  {
    if (inputEvent.IsActionPressed("Select_multiple"))
      AddUnitInvoke();
    if (inputEvent is InputEventMouseButton eventMouseButton)
    {
      switch (eventMouseButton.ButtonIndex)
      {
        case MouseButton.Left:
          SelectUnitInvoke();
          DoDrag = inputEvent.IsPressed() && Mode == InputMode.Drag;
          break;
        case MouseButton.Right:
          SelectTargetInvoke();
          break;
      }
    }
  }

  public void SelectUnitInvoke()
  {
    SelectUnit?.Invoke((ISelection)rigidBody3DParent);
  }

  public void DeselectUnitInvoke()
  {
    DeselectUnit?.Invoke((ISelection)rigidBody3DParent);
  }

  public void AddUnitInvoke()
  {
    AddUnit?.Invoke((ISelection)rigidBody3DParent);
  }

  public void SelectTargetInvoke()
  {
    SelectTarget?.Invoke(rigidBody3DParent);
  }
  public void AddTargetInvoke()
  {
    AddTarget?.Invoke(rigidBody3DParent);
  }

  void Drag()
  {
    var transform = rigidBody3DParent.GlobalTransform;
    transform.Origin = GetMouseWorldPosition();
    rigidBody3DParent.GlobalTransform = transform;
  }

  public override void _Process(double delta)
  {
    if (DoDrag)
    {
      Drag();
    }
  }

  public Vector3 GetMouseWorldPosition()
  {
    var ray_length = 1000;
    var mousePos = GetViewport().GetMousePosition();
    var camera3D = GetViewport().GetCamera3D();
    var from = camera3D.ProjectRayOrigin(mousePos);
    var to = from + camera3D.ProjectRayNormal(mousePos) * ray_length;
    Vector3 p = rigidBody3DParent.Position;
    var space_state = rigidBody3DParent.GetWorld3D().DirectSpaceState;
    var state = space_state.IntersectRay(new PhysicsRayQueryParameters3D()
    {
      From = from,
      To = to,
      Exclude = { rigidBody3DParent.GetRid() }
    });
    if (state.ContainsKey("position"))
    {
      p = (Vector3)state["position"];
      p.Y = rigidBody3DParent.Position.Y;
    }
    return p;
  }

}
