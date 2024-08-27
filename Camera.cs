using Godot;
using System;

public partial class Camera : Godot.Camera3D
{

	[Export]
	public float MinZoom { get; set; } = 2.0f;
	[Export]
	public float MaxZoom { get; set; } = 50.0f;
	[Export]
	public float ZoomSpeed { get; set; } = 40f;
	[Export]
	public float ZoomDampener { get; set; } = 0.92f;

	public float ZoomDirection { get; set; } = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		float z = (MaxZoom - MinZoom)/2;
		AddOrigin(new Vector3(0,0,z));
	}
	public override void _Input(InputEvent inputEvent){
		if(Input.IsActionPressed("camera_zoom_in"))
			ZoomDirection = -1;
		if(Input.IsActionPressed("camera_zoom_out"))
			ZoomDirection = 1;
	}	

	void CameraZoom(double delta){
		if(Transform.Origin.Z > MinZoom){
			AddOrigin(new Vector3(0,0,ZoomSpeed*ZoomDirection*(float)delta));
			ZoomDirection *= ZoomDampener;
		}	
		if(Transform.Origin.Z < MaxZoom){
			AddOrigin(new Vector3(0,0,ZoomSpeed*ZoomDirection*(float)delta));
			ZoomDirection *= ZoomDampener;
		}
	}


	void AddOrigin(Vector3 position){
		var temp = Transform;
		temp.Origin += position;
		Transform = temp;
	}
	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CameraZoom(delta);
	}
}
