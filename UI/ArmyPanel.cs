using Godot;
using System;
using System.Collections.Generic;

public partial class ArmyPanel : ScrollContainer
{
	[Export]
	public string ItemScenePath { get; set; } = "res://UI/Unit_Card.tscn";

	PackedScene scene;

	List<UnitCard> unitCards = new List<UnitCard>();

	Node container;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scene = (PackedScene)ResourceLoader.Load(ItemScenePath);
        GetNodes();
	}

	void GetNodes(){
		//container = GetNode("BuildingPanel");
		foreach (var item in GetChildren()[0].GetChildren())
		{
			if(item is UnitCard card){
				unitCards.Add(card);
				card.Hide();
				}

		}
	}

	public void ConnectBuildButtons(ArmyInterface planetInterface){
		foreach (var node in container.GetChildren()){
			var b = (Button)node.GetChild(0);
			b.MouseEntered += () => planetInterface._on_build_button_mouse_entered();
		}
	}

	public void ConnectUnitCards(ArmyInterface planetInterface){
		foreach(var node in unitCards){
			var b = node.GetChild(0).GetNode<Button>("Button");
			b.MouseEntered += () => planetInterface._on_BuildingLabelGuiInputEvent(node.Unit, b.Disabled);
			b.MouseExited += () => planetInterface._mouseLeftBuildingLabel();
		}
	}

	public void UpdateArmyList(List<Unit> units){
		if(unitCards.Count < units.Count)
			AddCards(units.Count - unitCards.Count);
		for (int i = 0; i < unitCards.Count; i++){
			if(i < units.Count){
				unitCards[i].Show();
				unitCards[i].UpdateCard(units[i]);
			}else{
				unitCards[i].Hide();
			}
		}
	}

	void AddCards(int count){
		for (int i = 0; i<count;i++){
			unitCards.Add(scene.Instantiate<UnitCard>());
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
