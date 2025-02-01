using Godot;
using System;

public partial class BuialdingPanel : ListPanel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	void InitAllBuildings(Array<Node> nodes){ // todo: Separate construction and building list into separate nodes for better organization
		var construction = planet.BuildingsManager.CurrentConstruction();
		foreach(Node node in nodes){
			if(node is Building building)
				if(CheckBuildingResources(planet, building)){
					if(_planet.BuildingsManager.Buildings.Find(x => x.Name == building.Name) == null){
						var label = (BuildingLabel)ItemScene.Instantiate();
						label.RefBuilding = building;
						label.Name = building.Name;
						if(label.BButton != null){
							label.BButton.Text = building.Name;
						}else{
							label.BButton = label.GetNode<Button>("Button");
							label.BButton.Text = building.Name;
						}
						AddChild(label);
						// if(label.Progress != null && construction.Count > 0){                             // ProgressBar was null, bcs label.getnodes method is executed when label enters the tree, so it has to be done after AddNodeToPanel
						// 	var currentBuilding = construction.Find(x => x.Name == building.Name);
						// 	if(currentBuilding != null){
						// 		label.Progress.Value = currentBuilding.CurrentTime;
						// 		label.Progress.MaxValue = currentBuilding.BuildTime;
						// 	}
						// }
					}
				}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
