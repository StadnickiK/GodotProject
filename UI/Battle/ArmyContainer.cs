using Godot;
using System;
using System.Collections.Generic;

public partial class ArmyContainer : VBoxContainer
{
	[Export]
	public string ItemScenePath { get; set; } = "res://UI/Unit_Card.tscn";

	PackedScene scene;

	Label ArmyName;

	Label ControllerName;

	public UnitCardFactory UnitCardFactory { get; set; }

	List<UnitCard> unitCards = new List<UnitCard>();

	Node container;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scene = (PackedScene)ResourceLoader.Load(ItemScenePath);
        GetNodes();
	}

	void GetNodes(){
		container = GetNode("ArmyGrid");
		ArmyName = GetNode<Label>("ArmyName");
		ControllerName = GetNode<Label>("ControllerName");
		//GetChildren()[0].GetChildren().Count;
		var p = 0;
		foreach (var item in container.GetChildren())
		{
			if(item is UnitCard card){
				unitCards.Add(card);
				card.Index = p;
				card.Hide();
				p++;
			}
		}
	}

	public void ConnectUnitCards(UI ui){
		foreach(var node in unitCards){
			var b = node.GetChild(0).GetNode<Button>("Button");
			b.MouseEntered += () => ui._on_BuildingLabelGuiInputEvent(node.Unit, b.Disabled);
			b.MouseExited += () => ui._mouseLeftBuildingLabel();
		}
	}

	public void UpdateArmyContainer(IEnterCombat army)
	{
		Show();
		ArmyName.Text = army.Name;
		ControllerName.Text = army.Controller.PlayerName;
		UpdateArmyList(army.UnitController.UnitsList);
	}

	void UpdateArmyList(List<Unit> units)
	{
		if (unitCards.Count < units.Count)
			AddCards(units.Count - unitCards.Count);
		for (int i = 0; i < unitCards.Count; i++)
		{
			if (i < units.Count)
			{
				unitCards[i].Show();
				unitCards[i].UpdateCard(units[i]);
				unitCards[i].button.Disabled = !units[i].HasHitpoints;
			}
			else
			{
				unitCards[i].Hide();
			}
		}
	}

	public void ResetPanel(){

		foreach(var card in unitCards){
			card.button.ButtonPressed = false;
		}
	}

	void AddCards(int count){
		for (int i = 0; i<count;i++){
			unitCards.Add(UnitCardFactory.GetInstance(container));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
