using Godot;
using System;
using System.Collections.Generic;

public partial class ArmyPanel : ScrollContainer
{
	public delegate void UpdateUnitsToTransferEventHandler(List<Unit> UnitsToTransfer);

	public event UpdateUnitsToTransferEventHandler UpdateUnitsToTransfer;

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/Unit_Card.tscn";

	PackedScene scene;

	List<UnitCard> unitCards = new List<UnitCard>();

	public List<Unit> UnitsToTransfer { get; set; } = new List<Unit>();

	Node container;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scene = (PackedScene)ResourceLoader.Load(ItemScenePath);
        GetNodes();
	}

	void GetNodes(){
		//container = GetNode("BuildingPanel");
		//GetChildren()[0].GetChildren().Count;
		var p = 0;
		foreach (var item in GetChildren()[0].GetChildren())
		{
			if(item is UnitCard card){
				unitCards.Add(card);
				card.Index = p;
				card.button.ToggleMode = true;
				card.button.ButtonUp += () => _on_SelectUnit(card.Index);
				card.Hide();
				p++;
			}
		}
	}

	public void _on_SelectUnit(int pos){
		//unitCards[pos].button.Disabled = true;
		if(unitCards[pos].button.ButtonPressed){
			UnitsToTransfer.Add(unitCards[pos].Unit);
		}else{
			UnitsToTransfer.Remove(unitCards[pos].Unit);
		}
		UpdateUnitsToTransfer?.Invoke(UnitsToTransfer);
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

	public void UpdateArmyList(List<Unit> units, bool IsController){
		if(unitCards.Count < units.Count)
			AddCards(units.Count - unitCards.Count);
		for (int i = 0; i < unitCards.Count; i++){
			if(i < units.Count){
				unitCards[i].Show();
				unitCards[i].UpdateCard(units[i]);
				unitCards[i].button.Disabled = !IsController;
			}else{
				unitCards[i].Hide();
			}
		}
	}

	public void ResetPanel(){

		foreach(var card in unitCards){
			card.button.ButtonPressed = false;
		}
		UnitsToTransfer.Clear();
		UpdateUnitsToTransfer?.Invoke(UnitsToTransfer);
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
