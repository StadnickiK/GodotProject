using Godot;
using System;

public partial class KinematicCube : CharacterBody3D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    KinematicCube self = null;
    Select selectNode;

  //public TargetManager targetManager { get; set; } = new TargetManager();

	private void LookFollow(PhysicsDirectBodyState3D state, Transform3D currentTransform, Vector3 targetPosition)
	{
		var upDir = new Vector3(0, 1, 0);
		var curDir = currentTransform.Basis * (new Vector3(0, 0, 1));
		var targetDir = (targetPosition - currentTransform.Origin).Normalized();
		var rotationAngle = Mathf.Acos(curDir.X) - Mathf.Acos(targetDir.X);
		state.AngularVelocity = upDir * (rotationAngle / state.Step);
	}
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
      self = (KinematicCube)GetParent().GetChild(GetIndex());
      selectNode = GetNode<Select>("/root/World/WorldCursorControl/Select");
    }

  public void _on_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
      if(inputEvent is InputEventMouseButton eventMouseButton){
        switch(eventMouseButton.ButtonIndex){
          case MouseButton.Left:
            GD.Print("LeftClick");
            //selectNode.SelectUnit((KinematicCube)self);
            //s.SelectedUnits.Add((KinematicCube)self);
            break;
          case MouseButton.Right:
            GD.Print("RightClick");
            //selectNode.AddTarget(self);
            break;
        }
      } 
  }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
public override void _Process(double delta)
  {
        var targetPosition = GetNode<RigidBody3D>("/root/World/Target").GlobalTransform.Origin;
        var ktargetPosition = GetNode<CharacterBody3D>("/root/World/KTarget").GlobalTransform.Origin;
        var upDir = new Vector3(0, 1, 0);
      	//var curDir = GlobalTransform.basis.Xform(new Vector3(0, 0, 1));
		    //var targetDir = GlobalTransform.origin.DirectionTo(targetPosition);//GlobalTransform.origin.AngleTo(targetPosition);
        //var angle = Math.Sqrt(Math.Tan(targetDir.x/targetDir.z))*100;
        //RotationDegrees = new Vector3(0,(float)angle,0);
        //Translation = targetDir;   // translation = position
        //RotationDegrees = new Vector3(0,Transform.origin.AngleTo(targetPosition),0);
        //Transform.SetLookAt(curDir, ktargetPosition, upDir);
        //GD.Print(targetPosition);
        var r = Rotation;
        r.X = 0;
        Rotation = r;
        //GD.Print(Transform.origin);
        /*
        targetDir.x = 0;
        targetDir.z = 0;
        targetDir.y *= -2;
        Rotation = targetDir;
        //TranslateObjectLocal(targetDir);
        /*
        var t = Transform3D;
        t.origin += targetDir;
        Transform3D = t;
        */
  }
}   
