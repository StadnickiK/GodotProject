using Godot;
using System;
using System.Collections.Generic;

public class StarSystem : StaticBody, IEnterMapObject, IExitMapObject
{

	[Export]
	int Size = 10;

	[Export]
	int Wealth = 5;

	[Signal]
	public delegate void ViewStarSystem(StarSystem system);

	[Signal]
	public delegate void ViewGalaxy(StarSystem system);

	[Signal]
	public delegate void SelectTarget(StarSystem target);

private int _radius;
public int Radius
{
	get { return _radius; }
	set { _radius = value; }
}


	MeshInstance _size = null;

	Spatial _mask = null;   

	public String SystemName { get; set; }

	public Text3 SystemName3D { get; set; } = null;

	public int SystemID { get; set; }
	// Node storing objects in the star system

	private Spatial _starSysObjects;
	public Spatial StarSysObjects
	{
		get { return _starSysObjects; }
		set { _starSysObjects = value; }
	}

	CollisionShape Placeholder = null;
	Button XButton = null;

	private List<Planet> _planets = new List<Planet>();
	public List<Planet> Planets
	{
		get { return _planets; }
	}

	public Star SystemStar { get; set; } = null;

	public Random Rand { get; set; } = new Random();

	public void GetNodes(){
		_starSysObjects = GetNode<Spatial>("StarSysObjects");
		SystemName3D = GetNode<Text3>("Placeholder/Text3");
		Placeholder = GetNode<CollisionShape>("Placeholder");
		XButton = GetNode<Button>("XButton");
		_size = GetNode<MeshInstance>("StarSysObjects/Diameter");
		_mask = GetNode<Spatial>("Placeholder/Mask");
	}

	void GenerateMesh(){
		var mesh = (MeshInstance)SystemStar.Mesh.Duplicate();
		mesh.Scale = new Vector3(4,4,4);
		_mask.AddChild(mesh);
		foreach(Planet planet in _planets){
			mesh = (MeshInstance)planet.Mesh.Duplicate();
			var transform = mesh.Transform;
			transform.origin = planet.Transform.origin;
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

	public void OpenStarSystem(){
		StarSysObjects.Visible = true;
		XButton.Visible = true;
		Placeholder.Visible = false;
		EmitSignal(nameof(ViewStarSystem), this);
	}

	public void AddMapObject(PhysicsBody body){
		if(body.GetParent() != StarSysObjects)
			StarSysObjects.AddChild(body);
	}

	void _on_StarSystem_input_event(Node camera, InputEvent e,Vector3 click_position,Vector3 click_normal, int shape_idx){
		if(e is InputEventMouseButton mouseButton){
			if(!mouseButton.Pressed && mouseButton.ButtonIndex == (int)ButtonList.Left){
				OpenStarSystem();
			}else if(!mouseButton.Pressed && mouseButton.ButtonIndex == (int)ButtonList.Right){
				EmitSignal(nameof(SelectTarget), (PhysicsBody)this);
			}
		}
	}

	void _on_XButton_button_up(){
		CloseSystem();
		EmitSignal(nameof(ViewGalaxy), this);
	}

	public void CloseSystem(){
		StarSysObjects.Visible = false;
		XButton.Visible = false;
		Placeholder.Visible = true;
	}

	protected void _ConnectSignal(){
		WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/Game/World/WorldCursorControl");
		WCC.ConnectToSelectTarget(this);
	}

	public void EnterMapObject(Node node, Vector3 aproachVec, PhysicsDirectBodyState state){
		if(node != null){
			if(node.GetParent() == GetParent())
				if(node is Ship ship){
					ship.GetParent().RemoveChild(ship);
					_starSysObjects.AddChild(ship);
					ship.NextTarget();
					var trans = state.Transform;
					trans.origin = Radius*0.9f*(-aproachVec)+GlobalTransform.origin;
					state.Transform = trans;
					ship.MapObject = this;
				}
        }
	}

	public void ExitMapObject(Node node, Vector3 aproachVec, PhysicsDirectBodyState state){
		if(node is Ship ship)
			if(ship.MapObject == this){
				if(ship.Transform.origin.Length()>Radius){
					//StarSysObjects.RemoveChild(ship);
					ship.GetParent().RemoveChild(ship);
					GetParent().AddChild(ship);
					var trans = state.Transform;
					trans.origin = Transform.origin;
					state.Transform = trans;
					ship.NextTarget();
					ship.Visible = false;
				}
			}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		_size.Scale = new Vector3(_radius,1,_radius);
		_ConnectSignal();
		GenerateMesh();
	}

}
