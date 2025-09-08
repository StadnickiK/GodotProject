using Godot;
using System;
using System.Collections.Generic;

public partial class ArmyPanel : ScrollContainer
{
	public delegate void UpdateUnitsToTransferEventHandler(List<Unit> UnitsToTransfer);

	public event UpdateUnitsToTransferEventHandler UpdateUnitsToTransfer;

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/Unit_Card.tscn";

	public delegate void NewCardEventHandler(UnitCard card);

	public event NewCardEventHandler NewCard;

	PackedScene scene;

	List<UnitCard> unitCards = new List<UnitCard>();

	public List<Unit> UnitsToTransfer { get; set; } = new List<Unit>();

	public UI UI { get; set; }
    public List<UnitCard> UnitCards { get => unitCards; private set => unitCards = value; }

    Node container;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scene = (PackedScene)ResourceLoader.Load(ItemScenePath);
        GetNodes();
	}

	void GetNodes(){
		container = GetNode("Grid");
		//GetChildren()[0].GetChildren().Count;
		var p = 0;
		foreach (var item in container.GetChildren())
		{
			if(item is UnitCard card){
				UnitCards.Add(card);
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
		if(UnitCards[pos].button.ButtonPressed){
			UnitsToTransfer.Add(UnitCards[pos].Unit);
		}else{
			UnitsToTransfer.Remove(UnitCards[pos].Unit);
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
		foreach(var card in UnitCards){
			ConnectUnitCard(card);
		}
	}

	void ConnectUnitCard(UnitCard card)
	{
		var b = card.GetChild(0).GetNode<Button>("Button");
		b.MouseEntered += () => UI._on_BuildingLabelGuiInputEvent(card.Unit);
		b.MouseExited += () => UI._mouseLeftBuildingLabel(); // simplifying breaks this
	}

	public void UpdateArmyList(List<Unit> units, bool IsController)
	{
		while(UnitCards.Count < units.Count)
			AddCards();
		for (int i = 0; i < UnitCards.Count; i++)
		{
			if (i < units.Count)
			{
				UnitCards[i].Show();
				UnitCards[i].UpdateCard(units[i]);
				UnitCards[i].button.Disabled = !IsController;
			}
			else
			{
				UnitCards[i].Hide();
			}
		}
	}
	
	public void UpdateArmyList(List<Unit3D> units, bool IsController)
	{
		while(UnitCards.Count < units.Count)
			AddCards();
		for (int i = 0; i < UnitCards.Count; i++)
		{
			if (i < units.Count)
			{
				UnitCards[i].Show();
				UnitCards[i].UpdateCard(units[i]);
				UnitCards[i].button.Disabled = !IsController;
			}
			else
			{
				UnitCards[i].Hide();
			}
		}
	}

	public void ResetPanel()
	{

		foreach (var card in UnitCards)
		{
			card.button.ButtonPressed = false;
		}
		UnitsToTransfer.Clear();
		UpdateUnitsToTransfer?.Invoke(UnitsToTransfer);
	}

	void AddCards(int count = 1){
		for (int i = 0; i < count; i++)
		{
			var card = scene.Instantiate<UnitCard>();
			ConnectUnitCard(card);
			UnitCards.Add(card);
			container.AddChild(card);
			NewCard?.Invoke(card);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
