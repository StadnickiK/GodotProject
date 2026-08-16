using Godot;
using System;
using System.Text;

public partial class DebugContainer : Control
{
	public CameraDebug CameraDebug { get; set; }

	public PerformanceDebug PerformanceDebug { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CameraDebug = GetNode<CameraDebug>("GridContainer/CameraDebug");
		PerformanceDebug = GetNode<PerformanceDebug>("GridContainer/PerformanceDebug");
	}

	public void Initialize(IWorld world)
	{
		CameraDebug.Initialize(world);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
