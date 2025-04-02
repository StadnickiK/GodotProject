using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BuildMenu : ScrollContainer
{

	Node container;

	public List<BuildingLabel> buildingLabels = new List<BuildingLabel>();

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/BuildingLabel.tscn";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		container = GetNode("BuildMenu");
		//ConnectBuildButtons();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void ConnectBuildButtons(){
		foreach (var node in container.GetChildren()){
			var b = (Button)node.GetChild(0);
			b.MouseExited += () => _on_mouse_exited();
		}
	}

	public void InitAllBuildings(List<Building> buildings, PlanetInterface planetInterface){ // todo: Separate construction and building list into separate nodes for better organization
		var ItemScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
		foreach(var building in buildings){
				var label = (BuildingLabel)ItemScene.Instantiate();
				label.RefBuilding = building;
				label.Size = new Vector2(100, 100);
				label.Name = building.Name;
				if(label.BButton != null){
					label.BButton.Text = building.Name;
				}else{
					label.BButton = label.GetNode<Button>("Button");
					label.BButton.Text = building.Name;
				}
				label.MouseEntered += () => planetInterface._on_BuildingLabelGuiInputEvent(label.RefBuilding, label.BButton.Disabled);
				label.MouseExited += () => planetInterface._mouseLeftBuildingLabel();
				label.MouseExited += () => _on_mouse_exited();
				container.AddChild(label);
				buildingLabels.Add(label);
		}
	}

	public void InitAllUnits(Array<Node> nodes, PlanetInterface planetInterface){ // todo: Separate construction and building list into separate nodes for better organization
		var ItemScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
		foreach(Node node in nodes){
			if(node is Unit unit){
				var label = (BuildingLabel)ItemScene.Instantiate();
				label.RefUnit = unit;
				label.Size = new Vector2(100, 100);
				label.Name = unit.Name;
				if(label.BButton != null){
					label.BButton.Text = unit.Name;
				}else{
					label.BButton = label.GetNode<Button>("Button");
					label.BButton.Text = unit.Name;
				}
				label.MouseEntered += () => planetInterface._on_BuildingLabelGuiInputEvent(unit);
				label.MouseExited += () => _on_mouse_exited();
				container.AddChild(label);
				buildingLabels.Add(label);
			}
		}
	}

	public void InitAllUnits(Array<Node> nodes, ArmyInterface planetInterface){ // todo: Separate construction and building list into separate nodes for better organization
		var ItemScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
		foreach(Node node in nodes){
			if(node is Unit unit){
				var label = (BuildingLabel)ItemScene.Instantiate();
				label.RefUnit = unit;
				label.Size = new Vector2(100, 100);
				label.Name = unit.Name;
				if(label.BButton != null){
					label.BButton.Text = unit.Name;
				}else{
					label.BButton = label.GetNode<Button>("Button");
					label.BButton.Text = unit.Name;
				}
				label.MouseEntered += () => planetInterface._on_BuildingLabelGuiInputEvent(unit);
				label.MouseExited += () => _on_mouse_exited();
				container.AddChild(label);
				buildingLabels.Add(label);
			}
		}
	}

	public void UpdateBuildMenu(BuildingManager buildingManager){

		var newOrder = new List<int>();
		newOrder.AddRange(buildingManager.CanPay);
		newOrder.AddRange(buildingManager.CantPay);
		var labels = new List<BuildingLabel>();
		foreach(var label in buildingLabels)
			if(buildingManager.AvaiableBuildings.Contains(label.RefBuilding)){
				label.Show();
				labels.Add(label);
				if(buildingManager.CanPay.Contains(label.RefBuilding.Index)){
					label.BButton.Disabled = false;
				}else{
					label.BButton.Disabled = true;
				}
			}else{
				label.Hide();
			}
		
		SortBuildLabels(labels, newOrder);
	}

	void SortBuildLabels(List<BuildingLabel> labels, List<int> newOrder){
		labels.Sort((a, b) => newOrder.IndexOf(a.RefBuilding.Index).CompareTo(newOrder.IndexOf(b.RefBuilding.Index)));
		for(int i = 0; i < labels.Count; i++){
			container.MoveChild(labels[i], i);
		}
	}

	public void HideBuilding(Building building){
		buildingLabels.FirstOrDefault(x => x.RefBuilding == building).Hide();
	}

	void _on_mouse_entered(){
		if(!Visible)
			Show();
	}

	void _on_mouse_exited(){
		//GD.Print(GetRect() + " " + GetGlobalMousePosition());
		if(!GetRect().HasPoint(GetGlobalMousePosition())){
			Hide();
		}
	}

}
