using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class BuildingPanel : ScrollContainer
{
	List<BuildingLabel> buildingLabels = new List<BuildingLabel>();

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/BuildingLabel.tscn";
	
	public BuildButton BuildButton { get; set; }

	PackedScene scene;

	public Vector2 LabelSize { get; set; } = new Vector2(100,100);

	Node container;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        container = GetNode("BuildingPanel");
		BuildButton = container.GetNode<BuildButton>("BuildButton");
	}

	public void ConnectBuildButtons(PlanetInterface planetInterface){
		foreach (var node in container.GetChildren()){
			var b = (Button)node.GetChild(0);
			
			b.MouseEntered += () => planetInterface._on_build_button_mouse_entered();
		}
	}

	public void InitAllBuildings(List<Building> buildings, PlanetInterface planetInterface){ // todo: Separate construction and building list into separate nodes for better organization
		var ItemScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
		foreach(var building in buildings){
				var label = (BuildingLabel)ItemScene.Instantiate();
				label.Size = LabelSize;
				label.RefBuilding = building;
				label.MouseEntered += () => planetInterface._on_BuildingLabelGuiInputEvent(label.RefBuilding, label.BButton.Disabled);
				label.MouseExited += () => planetInterface._mouseLeftBuildingLabel();
				container.AddChild(label);
				buildingLabels.Add(label);
				label.Hide();	
		}
	}

	public void UpdatePlanetBuildings(BuildingManager buildingManager){
		var count = buildingManager.Buildings.Count + buildingManager.Constructions.ConstructionList.Count;
		if(count > buildingLabels.Count)
			AddBuildingLabel(count - buildingLabels.Count);
		int j = 0;
		for(int i = 0; i < count; i++){
			if(i < buildingManager.Constructions.ConstructionList.Count){
				buildingLabels[i].UpdateProgress(buildingManager.Constructions.ConstructionList[i]);
			}
			if(i > buildingManager.Constructions.ConstructionList.Count || buildingManager.Constructions.ConstructionList.Count == 0){
				buildingLabels[i].UpdateBuilding(buildingManager.Buildings[j]);
				j++;
			}
			if(i > count)
				buildingLabels[i].Hide();
		}
	}

	public void	HidePlanetBuildings(){
		foreach(var building in buildingLabels){
			building.Hide();
		}
	}

	public void AddBuildingLabel(int Count){
		for(int i = 0; i < Count; i++){
			var s = scene.Instantiate<BuildingLabel>();
			s.SelfModulate = new Color(1, 1, 1, 0.5f);
			s.InitProgress();
			buildingLabels.Add(s);
			container.AddChild(s);
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
