using Godot;
using System;

public partial class InputController : Node
{
    [Signal]
    public delegate void SelectUnitEventHandler(RigidBody3D unit);

    [Signal]
    public delegate void SelectTargetEventHandler(RigidBody3D target);

    RigidBody3D rigidBody3DParent;

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

    void _on_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
      if(inputEvent is InputEventMouseButton eventMouseButton){
        switch(eventMouseButton.ButtonIndex){
          case MouseButton.Left:
            EmitSignal(SignalName.SelectUnit, rigidBody3DParent);
            break;
          case MouseButton.Right:
            EmitSignal(SignalName.SelectTarget, rigidBody3DParent);
            break;
        }
      } 
    }
}
