using Godot;
using System;

public partial class InputEventController : Node
{

    public delegate void SelectCollisionObject3DEventHandler(CollisionObject3D collisionObject3D);

    public event SelectCollisionObject3DEventHandler SelectMapObject;

    public event SelectCollisionObject3DEventHandler SelectTarger;

    RigidBody3D CollisionObject3D;

    public override void _Ready()
    {
        var parent = GetParent();
        if (parent is RigidBody3D collisionObject3D)
        {
            CollisionObject3D = collisionObject3D;
            CollisionObject3D.InputEvent += _on_input_event;
        }
    }

    void _on_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, long shape_idx){
      if(inputEvent is InputEventMouseButton eventMouseButton){
        switch(eventMouseButton.ButtonIndex){
          case MouseButton.Left:
            SelectMapObject?.Invoke(CollisionObject3D);
            break;
          case MouseButton.Right:
            SelectTarger?.Invoke(CollisionObject3D);
            break;
        }
      } 
    }
}
