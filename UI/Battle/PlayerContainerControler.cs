using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerContainerControler : VBoxContainer
{
    public Button AddPlayerButton { get; set; }

    public Control PlayerContainer { get; set; }

    public HashSet<CustomBattlePlayerContainer> CustomBattlePlayerContainers { get; set; } = new HashSet<CustomBattlePlayerContainer>();

    public HashSet<Combatant> Combatants { get; set; } = new HashSet<Combatant>();

    CustomBattlePlayerContainer EditedContainer;
    public BuildingInterface BuildingInterface { get; set; }

    public BuildMenu BuildMenu { get; set; }

    public Vector3 MaxUnitSize { get; set; } = Vector3.Zero;

    public float MaxVisionRange { get; set; } = 0;

    [Export]
    public string CustomBattlePlayerContainerPath { get; set; } = ScenePaths.Instance.CustomBattlePlayerContainerScene;

    public delegate void UnitSizeChangedEventHandler(Vector3 size);

    public event  UnitSizeChangedEventHandler UnitSizeChanged;

    public delegate void MaxVisionRangeChangedEventHandler(float range);

    public event  MaxVisionRangeChangedEventHandler MaxVisionRangeChanged;

    PackedScene packedScene;
 
    public override void _Ready()
    {
        PlayerContainer = GetNode<Control>("ScrollContainer/PlayerContainer");
        AddPlayerButton = GetNode<Button>("TeamControlls/AddPlayerButton");
        BuildMenu = GetNode<BuildMenu>("BuildMenuScroll");
        AddPlayerButton.ButtonUp += OnAddPlayer;
        foreach (var item in PlayerContainer.GetChildren())
        {
            if(item is CustomBattlePlayerContainer armyView)
            {
                armyView.Header.HButton.Hide();
                AddCustomBattlePlayerContainer(armyView);
                Combatants.Add(armyView.Combatant);
            }
        }
        packedScene = ResourceLoader.Load<PackedScene>(CustomBattlePlayerContainerPath);
    }

    public void OnAddPlayer()
    {
        var armyView = (CustomBattlePlayerContainer)packedScene.Instantiate();
        AddCustomBattlePlayerContainer(armyView);
        Combatants.Add(armyView.Combatant);
    }

    void AddCustomBattlePlayerContainer(CustomBattlePlayerContainer armyView)
    {
        CustomBattlePlayerContainers.Add(armyView);
        if(armyView.GetParent() == null) PlayerContainer.AddChild(armyView);
        armyView.SetTitle("Player " + CustomBattlePlayerContainers.Count);
        armyView.Index = CustomBattlePlayerContainers.Count;
        armyView.Header.HButton.ButtonUp += () => { OnRemovePlayer(armyView); };
        armyView.BuildButton.MouseEntered += () => _on_build_button_mouse_entered(armyView);
        armyView.BuildButton.MouseEntered += _on_mouse_exited;
        if(BuildingInterface!= null) armyView.ConnectLabels(BuildingInterface);
    }

    public void OnRemovePlayer(CustomBattlePlayerContainer container)
    {
        CustomBattlePlayerContainers.Remove(container);
        Combatants.Remove(container.Combatant);
        container.QueueFree();
    }

    public void _on_build_button_mouse_entered(CustomBattlePlayerContainer source)
	{
		var pos = GetGlobalMousePosition();
		var x = pos.X - (BuildMenu.Size.X / 2);
		var y = pos.Y - BuildMenu.Size.Y + 20;
		BuildMenu.Position = new Vector2(x, y);
		//_planet.BuildingManager.UpdateCanPayBuildings(_planet.Controller.ResManager);
		//BuildMenu.UpdateBuildMenu(_planet.BuildingManager);
        EditedContainer = source;
		BuildMenu.Show();
	}

    void _on_mouse_exited(){
		//GD.Print(GetRect() + " " + GetGlobalMousePosition());
		if(!GetRect().HasPoint(GetGlobalMousePosition())){
			BuildMenu.Hide();
            EditedContainer = null;
		}
	}

    public void Initialize(Data data)
    {
        BuildMenu.InitAllUnits(data.Units);
        foreach (var label in BuildMenu.buildingLabels)
        {
            label.BButton.ButtonUp += () => EditedContainer._on_AddUnit(label.RefUnit.Duplicate());
            label.BButton.ButtonUp += () => _on_AddUnit(label.RefUnit);
                // label.MouseEntered += () => CustomBatttleMenu.PlayerContainerControler.ArmyView.ArmyInterfaceContainer.ArmyRecruitment.UpdateRecruitment(_planet.RecruitmentComponent, IsLocalPlayer);
				// label.MouseExited += _on_mouse_exited;
				// label.MouseExited += uI._mouseLeftBuildingLabel;
        }
    }

    void _on_AddUnit(Unit unit)
    {
        var s = MaxUnitSize;
        var size = unit.ModelData.MeshInstance3D.Mesh.GetAabb().Size;
            if (size.X > MaxUnitSize.X)
                s.X = size.X;
            if (size.Y > MaxUnitSize.Y)
                s.Y = size.Y;
            if (size.Z > MaxUnitSize.Z)
                s.Z = size.Z;
        
        MaxVisionRange = Mathf.Max(MaxVisionRange, unit.StatManager.GetStat(GlobalStatNames.VisionRange).CurrentValue);
        
        MaxVisionRangeChanged?.Invoke(MaxVisionRange);
        UnitSizeChanged?.Invoke(s);
    }

    public void InitializeBuildingInterface(BuildingInterface buildingInterface)
    {
        BuildingInterface = buildingInterface;
        foreach (var item in CustomBattlePlayerContainers)
        {
            item.ConnectLabels(buildingInterface);
        }
    }
}
