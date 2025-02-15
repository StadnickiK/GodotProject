using Godot;
using System;
using System.Collections.Generic;

public partial class StarSystem : Area3D //, IEnterMapObject, IExitMapObject
{

	[Export]
	int Size = 10;

	[Export]
	int Wealth = 5;

	[Signal]
	public delegate void ViewStarSystemEventHandler(StarSystem system);

	[Signal]
	public delegate void ViewGalaxyEventHandler(StarSystem system);

	[Signal]
	public delegate void SelectTargetEventHandler(StarSystem target);

	private int _radius;
	public int Radius
	{
		get { return _radius; }
		set { _radius = value; }
	}


	MeshInstance3D _size = null;

	Node3D _mask = null;   

	public String SystemName { get; set; }

	public Label3D MapObjectName3 { get; set; } = null;

	public int SystemID { get; set; }
	// Node storing objects in the star system

	private Node3D _starSysObjects;
	public Node3D StarSysObjects
	{
		get { return _starSysObjects; }
		set { _starSysObjects = value; }
	}

	CollisionShape3D Placeholder = null;
	Button XButton = null;

	private List<Planet> _planets = new List<Planet>();
	public List<Planet> Planets
	{
		get { return _planets; }
	}

	public Star SystemStar { get; set; } = null;

	public Random Rand { get; set; } = new Random();

	public void GetNodes(){
		_starSysObjects = GetNode<Node3D>("StarSysObjects");
		MapObjectName3 = GetNode<Label3D>("Placeholder/Text3");
		Placeholder = GetNode<CollisionShape3D>("Placeholder");
		XButton = GetNode<Button>("XButton");
		_size = GetNode<MeshInstance3D>("StarSysObjects/Diameter");
		_mask = GetNode<Node3D>("Placeholder/Mask");
	}

	void GenerateMesh(){
		var mesh = (MeshInstance3D)SystemStar.Mesh.Duplicate();
		mesh.Scale = new Vector3(4,4,4);
		_mask.AddChild(mesh);
		foreach(Planet planet in _planets){
			mesh = (MeshInstance3D)planet.Mesh.Duplicate();
			var transform = mesh.Transform;
			transform.Origin = planet.Transform.Origin;
			mesh.Transform = transform;
			mesh.Scale = new Vector3(2,2,2);
			_mask.AddChild(mesh);
		}
		_mask.Scale = new Vector3(0.05f, 0.05f, 0.05f);
	}
	
	public enum Type
	{
		Capitol,
		Core,
		Strategic,
		Colony
	}

	// public void OpenStarSystem(){
	// 	StarSysObjects.Visible = true;
	// 	XButton.Visible = true;
	// 	Placeholder.Visible = false;
	// 	foreach(var node in StarSysObjects.GetChildren()){
	// 		if(node is Ship ship){
	// 			if(ship.IsLocal && !ship.Visible)
	// 				ship.Visible = true;
	// 		}
	// 	}
	// 	EmitSignal(nameof(ViewStarSystemEventHandler), this);
	// }

	public void AddMapObject(PhysicsBody3D body){
		if(body.GetParent() != StarSysObjects)
			StarSysObjects.AddChild(body);
	}

	// void _on_StarSystem_input_event(Node camera, InputEvent e,Vector3 click_position,Vector3 click_normal, int shape_idx){
	// 	if(e is InputEventMouseButton mouseButton){
	// 		if(!mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left){
	// 			OpenStarSystem();
	// 		}else if(!mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Right){
	// 			EmitSignal(nameof(SelectTargetEventHandler), (CollisionObject3D)this);
	// 		}
	// 	}
	// }

	// void _on_XButton_button_up(){
	// 	CloseSystem();
	// 	EmitSignal(nameof(ViewGalaxyEventHandler), this);
	// }

	// public void CloseSystem(){
	// 	StarSysObjects.Visible = false;
	// 	XButton.Visible = false;
	// 	Placeholder.Visible = true;
	// }

	protected void _ConnectSignal(){
		WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/Game/World/WorldCursorControl");
		WCC.ConnectToSelectTarget(this);
	}

	// public void EnterMapObject(Node node, Vector3 aproachVec, PhysicsDirectBodyState3D state){
	// 	if(node != null){
	// 		if(node.GetParent() == GetParent())
	// 			if(node is Ship ship){
	// 				ship.GetParent().RemoveChild(ship);
	// 				_starSysObjects.AddChild(ship);
	// 				ship.NextTarget();
	// 				var trans = state.Transform;
	// 				trans.Origin = Radius*0.9f*(-aproachVec)+GlobalTransform.Origin;
	// 				state.Transform = trans;
	// 				ship.MapObject = this;
	// 			}
    //     }
	// }

	// public void ExitMapObject(Node node, Vector3 aproachVec, PhysicsDirectBodyState3D state){
	// 	if(node is Ship ship)
	// 		if(ship.MapObject == this){
	// 			if(ship.Transform.Origin.Length()>Radius){
	// 				//StarSysObjects.RemoveChild(ship);
	// 				ship.GetParent().RemoveChild(ship);
	// 				GetParent().AddChild(ship);
	// 				var trans = state.Transform;
	// 				trans.Origin = Transform.Origin;
	// 				state.Transform = trans;
	// 				ship.NextTarget();
	// 				ship.Visible = false;
	// 			}
	// 		}
	// }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		// SetProcess(false);
		_size.Scale = new Vector3(_radius,1,_radius);
		_ConnectSignal();
		GenerateMesh();
	}

}
