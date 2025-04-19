using Godot;
using System;

public partial class CameraGimbal : Node3D
{
	bool drag = false;
	[Export]
	public float MovementSpeed { get; set; } = 20;
	[Export] 
	public float RotationSpeed { get; set; } = 0.7f;
	[Export]
	public float ZoomSpeed { get; set; } = 80f;
	[Export]
	public float MinZoom { get; set; } = 10f;
	[Export]
	public float MaxZoom { get; set; } = 70f;

	public Vector3 LimitCenter { get; set; } = Vector3.Zero;

	[Export]
	public int Limit { get; set; } = -1;

	[Export]
	Vector3 PosToLookFrom = new Vector3(0,0,10);



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var camera3D = GetChildren()[0].GetChild<Camera>(0);
		camera3D.ZoomSpeed = ZoomSpeed;
		camera3D.MinZoom = MinZoom;
		camera3D.MaxZoom = MaxZoom;
	}

public override void _Input(InputEvent inputEvent){
	// if(inputEvent is InputEventKey key){
	//     KeyboardAction(key);
	// }
	if(inputEvent is InputEventMouseButton button){
		
		if(button.ButtonIndex == MouseButton.Right){
			if(drag != true){
				drag = true;
			}else{
				drag = false;
			}
		}
	}

	if(inputEvent is InputEventMouseMotion motion && drag){
		//GlobalRotate(new Vector3(0,1,0), Mathf.Deg2Rad(motion.Relative.x*RotationSpeed));
		RotateY(Mathf.DegToRad(motion.Relative.X*RotationSpeed));
	}
}

Vector3 DirToTarget(Vector3 target){
	return (target-GlobalTransform.Origin).Normalized();
}

public void LookAt(Vector3 target){
	LookAtFromPosition(target+PosToLookFrom,target,new Vector3(0,1,0));
	Rotation *= new Vector3(0,1,0);
}

Vector2 InputKeyToVector2(){
	Vector2 input_movement_vector = new Vector2();
	if (Input.IsActionPressed("camera_up"))
		input_movement_vector.Y -= 1;
	if (Input.IsActionPressed("camera_down"))
		input_movement_vector.Y += 1;
	if (Input.IsActionPressed("camera_left"))
		input_movement_vector.X -= 1;
	if (Input.IsActionPressed("camera_right"))
		input_movement_vector.X += 1;

	return input_movement_vector;
}

void KeyboardAction(InputEventKey key){
	Vector2 input_movement_vector = InputKeyToVector2();
	
	Vector3 Direction = new Vector3();
	Direction = GlobalTransform.Basis.Z.Normalized() * input_movement_vector.Y * MovementSpeed;
	Direction += GlobalTransform.Basis.X.Normalized() * input_movement_vector.X * MovementSpeed;

	if(Limit >-1){
		UpdatePositionWithLimit(Direction);
	}else{
		AddGlobalOrigin(Direction);
	}
}

void KeyboardAction(double time){
	Vector2 input_movement_vector = InputKeyToVector2();
	
	Vector3 Direction = new Vector3();
	Direction = GlobalTransform.Basis.Z.Normalized() * input_movement_vector.Y * MovementSpeed * (float)time;
	Direction += GlobalTransform.Basis.X.Normalized() * input_movement_vector.X * MovementSpeed * (float)time;

	if(Limit >-1){
		UpdatePositionWithLimit(Direction);
	}else{
		AddGlobalOrigin(Direction);
	}
}

void UpdatePositionWithLimit(Vector3 Direction){
	Vector3 origin = GlobalTransform.Origin + Direction;
	Vector3 dir = origin - LimitCenter;
	int distance = (int)(dir).Length();
	if(distance < Limit){
		AddGlobalOrigin(Direction);
	}else{
		SetGlobalOrigin((dir.Normalized()*(Limit-1)));
	}
}


void AddGlobalOrigin(Vector3 position){
	var temp = GlobalTransform;
	temp.Origin += position;
	GlobalTransform = temp;
}

void SetGlobalOrigin(Vector3 position){
	var temp = GlobalTransform;
	temp.Origin = position;
	GlobalTransform = temp;
}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(double delta)
 {
	KeyboardAction(delta);   
 }
}
