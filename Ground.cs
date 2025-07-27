using Godot;
using System;

public partial class Ground : Area3D
{

	[Export]
	public Vector3 Size { get; set; } = new Vector3(100, 1, 100);

	//CollisionShape3D _collisionShape;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var _collisionShape = GetNode<CollisionShape3D>("CollShape");
		var s = new BoxShape3D();
		s.Size = Size;
		_collisionShape.Shape = s;
		var m = new BoxMesh(){Size = Size};
		GetNode<MeshInstance3D>("Mesh").Mesh = m;

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
}
