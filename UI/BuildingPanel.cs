using Godot;
using Godot.Collections;
using System;

public partial class BuildingPanel : GridContainer
{

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/BuildingLabel.tscn";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        
	}

	public void InitAllBuildings(Array<Node> nodes, PlanetInterface planetInterface){ // todo: Separate construction and building list into separate nodes for better organization
		var ItemScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
		foreach(Node node in nodes){
			if(node is Building building){
				var label = (BuildingLabel)ItemScene.Instantiate();
				label.RefBuilding = building;
				label.Name = building.Name;
				if(label.BButton != null){
					label.BButton.Text = building.Name;
				}else{
					label.BButton = label.GetNode<Button>("Button");
					label.BButton.Text = building.Name;
				}
				label.MouseEntered += () => planetInterface._on_BuildingLabelGuiInputEvent(building);
				label.MouseExited += () => planetInterface._mouseLeftBuildingLabel();
				AddChild(label);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
