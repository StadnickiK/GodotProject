using Godot;
using System;
using System.Diagnostics;

public partial class InputController : Node
{
  [Signal]
  public delegate void SelectUnitEventHandler(RigidBody3D unit);

  [Signal]
  public delegate void SelectTargetEventHandler(RigidBody3D target);

  [Export]
  public InputMode Mode { get; set; } = InputMode.Select;

  [Export]
  public int DragSpeed { get; set; } = 10;

  bool DoDrag = false;

  RigidBody3D rigidBody3DParent;

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
      rigidBody3DParent = (RigidBody3D)GetParent();
      rigidBody3DParent.Connect(RigidBody3D.SignalName.InputEvent, new Callable(this, nameof(_on_input_event)));
    }
    catch (System.Exception ex)
    {
      throw new Exception("Parent should be RigidBody3D " + ex.Message);
    }
  }

  void _on_input_event(Node camera, InputEvent inputEvent, Vector3 click_position, Vector3 click_normal, int shape_idx)
  {
    if (inputEvent is InputEventMouseButton eventMouseButton)
    {
      switch (eventMouseButton.ButtonIndex)
      {
        case MouseButton.Left:
          EmitSignal(SignalName.SelectUnit, rigidBody3DParent);
          DoDrag = inputEvent.IsPressed();
          break;
        case MouseButton.Right:
          EmitSignal(SignalName.SelectTarget, rigidBody3DParent);
          break;
      }
    }
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

  public Vector3 GetMouseWorldPosition(){
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
