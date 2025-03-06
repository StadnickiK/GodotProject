using Godot;
using System;
using System.Collections.Generic;

public partial class PlanetInterfaceBuildQueue : ScrollContainer
{

	HBoxContainer container;

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/BuildingLabel.tscn";

	PackedScene scene;

	List<BuildingLabel> buildingLabels = new List<BuildingLabel>(); 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		container = GetNode<HBoxContainer>("HBoxContainer");
		foreach (var l in container.GetChildren()){
			if(l is BuildingLabel label){
				buildingLabels.Add(label);
				label.InitProgress();	
			}
		}

		scene = (PackedScene)ResourceLoader.Load(ItemScenePath);
	}

	public void UpdateBuildQueue(List<Building> buildings){
		Show();
		if(buildingLabels.Count < buildings.Count)
			AddBuildingLabel(buildings.Count - buildingLabels.Count);
		for(int i = 0; i < buildingLabels.Count; i++){
			if(i < buildings.Count){
				buildingLabels[i].Show();
				buildingLabels[i].UpdateProgress(buildings[i]);
			}else{
				buildingLabels[i].Hide();
			}
		}
	}

	public void UpdateBuildQueue(){
		for(int i = 0; i < buildingLabels.Count; i++){
			buildingLabels[i].Hide();
		}
		Hide();
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
