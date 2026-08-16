 using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Squad3D : Node3D, IUnit3D
{
	public Player Controller { get; set; }
	public VisibilityConroller VisibilityConroller { get; set; }
	public IMovableState MovableState { get; set; }
	public OrderQueue OrderQueue { get; set; }
	public Vector3 Velocity { get; set; }
	public Vector3 AngularVelocity { get; set; }

	public SelectionCircleControler Selection { get; set; }
	public StatManager StatManager { get; set; }

	public HealthBar3D HealthBar { get; set; }
	public InputController InputController { get; set; }

	public VelocityController VelocityController { get; set; }

	Node INode.GetAsNode { get { return this; } }

	List<Unit3D> Unit3Ds = new List<Unit3D>();

	List<Unit3D> ReversedUnit3Ds = new List<Unit3D>();

	HashSet<ISelection> SelectedUnits = new HashSet<ISelection>();

	public event Node3DPool.SaveNodeEventHandler SaveNode;
    public event DrawingLines3DController.SpawnOrderMarkersHandler SpawnMarkers;
    public event DrawingLines3DController.SpawnOrderMarkersHandler ClearMarkers;

    public float FormationWidth { get; set; } = 5;

	public float FormationDepth { get; set; } = 5;

	public Vector3 Size { get; set; } = Vector3.Zero;

	public Vector3 MaxSize { get; set; } = Vector3.Zero;
	public ProjectileFactory ProjectileFactory { get; set; }
	public int CardIndex { get; set; }

	// public CollisionShape3D CollisionShape3D { get; set; }

	public List<Vector3> Formation { get; set; } = new List<Vector3>();
    public MeshInstance3D MeshInstance3D { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void ChangeVision(VisibilityConroller.VisibilityStruct visibilityStruct, int playerID)
	{
		foreach (var item in Unit3Ds)
		{
			item.ChangeVision(visibilityStruct, playerID);
		}
	}

	public override void _Ready()
	{
		base._Ready();
		//Selection = GetNode<SelectionCircleControler>("SelectionCircleControler");
		StatManager = GetNode<StatManager>("StatManager");
		HealthBar = GetNode<HealthBar3D>("HealthBar");
		//CollisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
		//InputController = GetNode<InputController>("InputController");
	}

	public void ClearTargets()
	{
		foreach (var item in Unit3Ds)
		{
			item.ClearTargets();
		}
	}

	public void MoveToTarget(OrderQueue.Target target)
	{
		foreach (var item in Unit3Ds)
		{
			item.MoveToTarget(target);
		}
	}
	public int ReturnIndex()
	{
		return GetIndex();
	}

	public bool ReturnVisible()
	{
		return Visible;
	}

	public void SetVisibility(bool visible)
	{
		Visible = visible;
	}

	public void UpdateVelocity(Vector3 linearVelocity, Vector3 angularVelocity)
	{
		foreach (var item in Unit3Ds)
		{
			item.UpdateVelocity(linearVelocity, angularVelocity);
		}
	}

	public void UpdateModel(Unit unit)
	{
		foreach (var item in Unit3Ds)
		{
			item.UpdateModel(unit);
			Size += item.GetUnitSize();
		}
		MoveToPosition(GlobalPosition);
		// UpdateShape();
	}

	void UpdateShape()
	{
		var shape = new CylinderShape3D();
		shape.Height = Size.Y;
		shape.Radius = Size.X > Size.Z ? Size.X : Size.Z;
		// CollisionShape3D.Shape = shape;
	}

	public void LoadUnit(IStatManager unit)
	{
		StatManager.CloneStats(unit);
		var squadHealth = StatManager.GetStat(GlobalStatNames.Health);
		squadHealth.CurrentValue = squadHealth.MaxValue = 0;
		
		
		foreach (var item in Unit3Ds)
		{
			var Health = item.StatManager.GetStat(GlobalStatNames.Health);
			squadHealth.CurrentValue += Health.CurrentValue;
			squadHealth.MaxValue += Health.MaxValue;
			item.LoadUnit(unit);
		}
		StatManager.ConnectStatListeners(StatManager);
		StatManager.UpdateListeners();
	}

	public void InvokeSaveNode()
	{
		foreach (var item in Unit3Ds)
		{
			item.InvokeSaveNode();
		}
		QueueFree();
	}

	public void BeforeSave() {}

	public void Initialize(UnitFactory unitFactory, Node parent, Vector3 position, Player controller, Unit unit)
	{
		var squad = (Squad)unit;
		Controller = controller;
		LoadUnit(unit);
		// unitFactory.ConnectSignals(this);
		for (int i = 0; i < squad.ModelCount; i++)
		{
			var Unit3D = (Unit3D)unitFactory.CreateUnit(this, position, controller);
			unitFactory.DrawingLines3D.ConnectUnit3D(Unit3D);
			Unit3D.ProjectileFactory = unitFactory.ProjectileFactory;
			// Unit3D.LoadUnit(squad.Units[i]);
			// Unit3D.UpdateModel(squad.Units[i]);
			Unit3D.Initialize(this, position,controller,squad.Units[i]);
			Unit3Ds.Add(Unit3D);
			Unit3D.InputController.ConnectWCC(unitFactory.WorldCursorControl);
			Size += Unit3D.GetUnitSize();
			UpdateMaxSize(Unit3D.GetUnitSize());
			ConnectToSquad(Unit3D);
			//Unit3D.InputController.ConnectToSquad(this);	
			if(InputController == null)
			{
				InputController = Unit3D.InputController;
				OrderQueue = Unit3D.OrderQueue;
			}
		}
		ReversedUnit3Ds = Unit3Ds.AsEnumerable().Reverse().ToList();
		//Formation = WedgeFormation(position, Unit3Ds.Count, MaxSize.X/2 + FormationWidth, MaxSize.Z/2 + FormationDepth);
	}

	void UpdateMaxSize(Vector3 size)
	{
        if (size.X > MaxSize.X)
            MaxSize = new Vector3(size.X,MaxSize.Y,MaxSize.Z);
        if (size.Y > MaxSize.Y)
            MaxSize = new Vector3(MaxSize.X,size.Y,MaxSize.Z);
        if (size.Z > MaxSize.Z)
            MaxSize = new Vector3(MaxSize.X,MaxSize.Y,size.Z);
	}

	public void ConnectToSquad(Unit3D RigidBody3DParent)
	{
      RigidBody3DParent.Connect(CollisionObject3D.SignalName.InputEvent, new Callable(this, nameof(_on_input_event)));
	  //RigidBody3DParent.InputEvent += _on_input_event;
      //RightClickAction = GetNode<MoveToTargetAction>("RightClickAction");
      var s = RigidBody3DParent.GetNodeOrNull<Shield>("Shield");
      s?.Connect(CollisionObject3D.SignalName.InputEvent, new Callable(this, nameof(_on_input_event)));
	}

	void _on_input_event(Node camera, InputEvent inputEvent, Vector3 click_position, Vector3 click_normal, int shape_idx)
	{
		if (inputEvent is InputEventMouseButton eventMouseButton)
			switch (eventMouseButton.ButtonIndex)
			{
				case MouseButton.Left:
					Game.Instance.Select(this);
				break;
			}
	}

	public void AddSquad()
	{
		foreach (var item in Unit3Ds)
			item.InputController.AddUnitInvoke();
	}

	public void AddSquad(ISelection selection)
	{
		foreach (var item in Unit3Ds)
		{
			if(selection != item && !SelectedUnits.Contains(selection))
			{
				item.InputController.AddUnitInvoke();
				SelectedUnits.Add(selection);
			}
				
		}
	}

	public void DeselectSquad(ISelection selection)
	{
		foreach (var item in Unit3Ds)
		{
			if(selection != item && SelectedUnits.Contains(selection))
			{
				item.InputController.DeselectUnitInvoke();
				SelectedUnits.Remove(selection);
			}
		}
	}

	public void SelectTargetSquad(INode3D selection)
	{
		foreach (var item in Unit3Ds)
		{
			if(selection != item)
				item.InputController.AddUnitInvoke();
		}
	}


	public void Select()
	{
		foreach (var item in Unit3Ds)
		{
			item.Select();
		}
	}

	public void Deselect()
	{
		foreach (var item in Unit3Ds)
		{
			item.Deselect();
		}
	}

	public Vector3 GetUnitSize()
	{
		return Size;
	}

	public void ChangeMovableState(IMovableState movableState)
	{
		foreach (var item in Unit3Ds)
		{
			item.MovableState = movableState;
		}
	}

    public void UpdatePosition(Vector3 position, float yRotation = 0)
    {
        //var squadDir = (targetPosition - GlobalPosition).Normalized();
		//GlobalPosition = position;
		for (int i = 0; i < Unit3Ds.Count; i++)
		{
			Vector3 formationOffset = CalculateFormationOffset(i);
			Vector3 rotatedOffset = formationOffset.Rotated(Vector3.Up, yRotation);
			Vector3 finalDestination = position + rotatedOffset; //* squadDir

			Unit3Ds[i].UpdatePosition(finalDestination, yRotation);
		}
    }

	// public void MoveToPosition(Vector3 targetPosition)
	// {
	// 	// var angle = Unit3Ds[0].GlobalPosition.SignedAngleTo(targetPosition, Vector3.Up);
	// 	var dir = (targetPosition - Unit3Ds[0].GlobalPosition).Normalized();
	// 	// var angle = VelocityController.GetAngleYToTarget(Unit3Ds[0].Transform, targetPosition);
	// 	float angle = Mathf.Atan2(dir.X,-dir.Z);
	// 	Basis rotation = Basis.FromEuler(new Vector3(0,angle,0));
		
	// 	for (int i = 0; i < Unit3Ds.Count; i++)
	// 	{
	// 		Vector3 formationOffset = CalculateFormationOffset(i);
	// 		var rotatedOffset = formationOffset * rotation;
	// 		Vector3 finalDestination = targetPosition + rotatedOffset; //* squadDir

	// 		Unit3Ds[i].MoveToPosition(finalDestination);
	// 	}
	// }

	public void MoveToPosition(Vector3 targetPosition)
	{
		var dir = (targetPosition - Unit3Ds[0].GlobalPosition).Normalized();
		var wedge = WedgeFormation(targetPosition, Unit3Ds.Count, dir);
		var assignedSlots = AssignUnitsToSlots(ReversedUnit3Ds, wedge);
		foreach (var item in assignedSlots)
		{
			item.Key.MoveToPosition(item.Value);
		}
	}

	Vector3 CalculateFormationOffset(int i)	//1
	{
			int col = i%2;		// 1
			int offsetMultiplier = i / 2;	// 1
			offsetMultiplier += col == 1 ? 1 : 0; // 2
			int sign = -2 * col + 1; // -1

			var size = Unit3Ds[i].GetUnitSize();
			float spacing = FormationWidth + (size.X * 0.8f);

			float xOffset = sign * offsetMultiplier * spacing;
			float zOffset = offsetMultiplier * spacing;; // Units line up behind the target

			Vector3 formationOffset = new Vector3(xOffset, 0, zOffset);
			return formationOffset;
	}

	Vector3 CalculateFormationOffset(Vector3 size, int i)	//1
	{
			int col = i%2;		// 1
			int offsetMultiplier = i / 2;	// 1
			offsetMultiplier += col == 1 ? 1 : 0; // 2
			int sign = -2 * col + 1; // -1

			float spacing = FormationWidth + (size.X * 0.8f);

			float xOffset = sign * offsetMultiplier * spacing;
			float zOffset = offsetMultiplier * spacing; // Units line up behind the target

			Vector3 formationOffset = new Vector3(xOffset, 0, zOffset);
			return formationOffset;
	}

	List<Vector3> WedgeFormation(Vector3 position, int unitCount, Vector3 direction)
	{
		var positions = new List<Vector3>
        {
            position
        };
		float angle = Mathf.Atan2(direction.X,-direction.Z);
		Basis rotation = Basis.FromEuler(new Vector3(0,angle,0));
		for (int i = 1; i < unitCount; i ++)
		{
			var formationOffset = CalculateFormationOffset(MaxSize, i);
			var rotatedOffset = formationOffset * rotation;
			Vector3 finalDestination = position + rotatedOffset;
			positions.Add(finalDestination);
		}
		return positions;
	}

	public static Dictionary<Unit3D, Vector3> AssignUnitsToSlots(
    List<Unit3D> units,
    List<Vector3> slots)
	{
		var availableSlots = new List<Vector3>(slots);
		var assignments = new Dictionary<Unit3D, Vector3>();

		foreach (var unit in units)
		{
			float bestDistance = float.MaxValue;
			int bestIndex = -1;

			for (int i = 0; i < availableSlots.Count; i++)
			{
				float d = unit.GlobalPosition.DistanceSquaredTo(availableSlots[i]);

				if (d < bestDistance)
				{
					bestDistance = d;
					bestIndex = i;
				}
			}

			assignments[unit] = availableSlots[bestIndex];
			availableSlots.RemoveAt(bestIndex);
		}
		return assignments;
	}

	Vector3 RotatedOffset(Vector3 right, Vector3 forward, int i)
	{
		Vector3 formationOffset = CalculateFormationOffset(i);
		var rotatedOffset = right * -formationOffset.X + forward * -formationOffset.Z;
		return rotatedOffset;
	}

    public Aabb GetAabb()
    {
        return new Aabb(Position, Size);
    }
}
