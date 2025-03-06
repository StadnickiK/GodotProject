using Godot;
using System;
using System.Collections.Generic;

public partial class ArmyRecruitment : HBoxContainer
{

	public List<UnitCard> unitCards { get; set; } = new List<UnitCard>();

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/UnitCard.cs";
	
	PackedScene scene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNodes();
		var ItemScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
	}

	void GetNodes(){
		foreach(var card in GetChildren()){
			if(card is UnitCard unitCard)
				unitCards.Add(unitCard);
		}
	}

	public void UpdateUnitCards(UnitController unitController){
		if(unitController.UnitsList.Count > unitCards.Count)
			AddLabel(unitController.UnitsList.Count - unitCards.Count);
		for (int i = 0; i < unitCards.Count; i++)
		{	
			if(unitController.UnitsList.Count < i){
				unitCards[i].UpdateProgress(unitController.UnitsList[i]);
			}else{
				unitCards[i].Hide();
			}	
			
		}
	}

	public void AddLabel(int Count){
		for(int i = 0; i < Count; i++){
			var s = scene.Instantiate<UnitCard>();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
