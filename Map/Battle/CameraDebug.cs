using Godot;
using System;
using System.Linq;
using System.Text;

public partial class CameraDebug : RichTextLabel
{
	public CameraGimbal CameraGimbal { get; set; }

	public WorldCursorControl WorldCursorControl { get; set; }

	StringBuilder text = new StringBuilder();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// public override void _Input(InputEvent inputEvent){
    //     if(inputEvent is InputEventMouseMotion motion){
    //         //GlobalRotate(new Vector3(0,1,0), Mathf.Deg2Rad(motion.Relative.x*RotationSpeed));
    //         CursorLastPosition = motion.Relative;
    //     }
    // }

	void Update()
	{
		text.Clear();
		text.AppendLine("Camera Info");
		text.AppendLine("Position ");
		text.AppendLine(CameraGimbal.Position.ToString());
		text.AppendLine("Rotation Y");
		text.AppendLine(CameraGimbal.Rotation.Y + " " + CameraGimbal.RotationDegrees.Y);
		text.AppendLine("Rotation X");
		text.AppendLine(CameraGimbal.InnerGimbal.Rotation.X + " " + CameraGimbal.InnerGimbal.RotationDegrees.X);
		text.AppendLine("Zoom ");
		text.AppendLine(CameraGimbal.Camera.Position.ToString());
		text.AppendLine("Mouse position ");
		if(WorldCursorControl != null) text.AppendLine(WorldCursorControl.MousePositionRounded.ToString());
		Text = text.ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(CameraGimbal != null)
			Update();
	}

    internal void Initialize(IWorld world)
    {
        CameraGimbal = world.Camera3D;
		WorldCursorControl = world.WCC;
    }

}
