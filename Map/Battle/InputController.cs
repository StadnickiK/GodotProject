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

  public event MoveEventHandler SelectUnit;

  public event MoveEventHandler DeselectUnit;

  public event MoveEventHandler AddUnit;

  public delegate void TargetEventHandler(INode3D movable);

  public event TargetEventHandler SelectTarget;

  public event TargetEventHandler AddTarget;

  public delegate void DragEventHandler();

  public event DragEventHandler Drag;

  [Export]
  public InputMode Mode { get; set; } = InputMode.Select;

  [Export]
  public int DragSpeed { get; set; } = 10;

  bool MultiSelect = false;

  bool inputEventConnected = false;

  public bool Selected { get; set; } = false;

  public ISelection RigidBody3DParent;

  CollisionObject3D collisionObject;

  public enum InputMode
  {
    Select,
    Drag
  }

  public void ChangeMode(InputMode mode)
  {
    Mode = mode;
  }

  public void Initialize(CollisionObject3D collisionObject3D, ISelection parent, Shield shield = null)
  {
      RigidBody3DParent = parent;
      collisionObject = collisionObject3D;
    if(!inputEventConnected)
    {
      inputEventConnected = true;
      var inputEventCallable = new Callable(this, nameof(_on_input_event));
      collisionObject3D.Connect(CollisionObject3D.SignalName.InputEvent, inputEventCallable);
    }
      //RightClickAction = GetNode<MoveToTargetAction>("RightClickAction");
      shield?.Connect(CollisionObject3D.SignalName.InputEvent, new Callable(this, nameof(_on_input_event)));
      SelectUnit -= Game.Instance.Select;
      SelectUnit += Game.Instance.Select;
  }

  public void ConnectWCC(WorldCursorControl Instance)
  {
    DeselectUnit -= Instance._DeselectUnit;
    DeselectUnit += Instance._DeselectUnit;
    AddUnit -= Instance._AddUnit;
    AddUnit += Instance._AddUnit;
    SelectTarget -= Instance._SelectTarget;
    SelectTarget += Instance._SelectTarget; 
  }

  public void ConnectToSquad(Squad3D Instance)
	{
		DeselectUnit -= Instance.DeselectSquad;
		DeselectUnit += Instance.DeselectSquad;
		AddUnit -= Instance.AddSquad;
		AddUnit += Instance.AddSquad;
		SelectTarget -= Instance.SelectTargetSquad;
		SelectTarget += Instance.SelectTargetSquad; 
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
    if (inputEvent is InputEventMouseButton eventMouseButton) //&& inputEvent.IsReleased()
    {
      switch (eventMouseButton.ButtonIndex)
      {
        case MouseButton.Left:
          if(inputEvent.IsReleased())
            SelectUnitInvoke();
          if (inputEvent.IsPressed())
          {
            if(!Selected) SelectUnitInvoke();
            if(Mode == InputMode.Drag) Drag?.Invoke();
          }
          // else if(inputEvent.IsPressed())
          //   Drag?.Invoke(RigidBody3DParent.Position);
          break;
        case MouseButton.Right:
          SelectTargetInvoke();
          break;
      }
    }
  }

  public void SelectUnitInvoke()
  {
    SelectUnit?.Invoke((ISelection)RigidBody3DParent);
  }

  public void DeselectUnitInvoke()
  {
    DeselectUnit?.Invoke((ISelection)RigidBody3DParent);
  }

  public void AddUnitInvoke()
  {
    AddUnit?.Invoke((ISelection)RigidBody3DParent);
  }

  public void SelectTargetInvoke()
  {
    SelectTarget?.Invoke(RigidBody3DParent);
  }
  public void AddTargetInvoke()
  {
    AddTarget?.Invoke(RigidBody3DParent);
  }

  public Vector3 GetMouseWorldPosition()
  {
    var ray_length = 1000;
    var mousePos = GetViewport().GetMousePosition();
    var camera3D = GetViewport().GetCamera3D();
    var from = camera3D.ProjectRayOrigin(mousePos);
    var to = from + camera3D.ProjectRayNormal(mousePos) * ray_length;
    Vector3 p = RigidBody3DParent.Position;
    var space_state = collisionObject.GetWorld3D().DirectSpaceState;
    var state = space_state.IntersectRay(new PhysicsRayQueryParameters3D()
    {
      From = from,
      To = to,
      Exclude = { collisionObject.GetRid() }
    });
    if (state.ContainsKey("position"))
    {
      p = (Vector3)state["position"];
      p.Y = RigidBody3DParent.Position.Y;
    }
    return p;
  }

}
