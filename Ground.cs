using Godot;
using System;

public partial class Ground : Area3D
{

	[Export]
	public Vector3 Size { get; set; } = new Vector3(100, 1, 100);

	public CollisionShape3D CollisionShape3D { get; set; }

	public MeshInstance3D MeshInstance3D { get; set; }


	//CollisionShape3D _collisionShape;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CollisionShape3D = GetNode<CollisionShape3D>("CollShape");
		MeshInstance3D = GetNode<MeshInstance3D>("Mesh");
		UpdateGround(Size);
	}

	public void ConnectToInputEvent(Callable callable)
    {
        if (!IsConnected(SignalName.InputEvent, callable))
            Connect(SignalName.InputEvent, callable);
    }

    public void DisconnectInputEvent(Callable callable)
    {
        if (IsConnected(SignalName.InputEvent, callable))
            Disconnect(SignalName.InputEvent, callable);
    }

	public void UpdateGround(float Radius)
	{
		UpdateGround(new Vector3(Radius, 0, Radius));
	}

	public void UpdateGround(Vector3 size)
	{
		var s = new BoxShape3D();
		s.Size = size;
		CollisionShape3D.Shape = s;
		var m = new BoxMesh(){Size = size};
		MeshInstance3D.Mesh = m;
	}
}
