using Godot;
using System;
using System.Collections.Generic;

public partial class ArmyRecruitment : HBoxContainer
{

	public List<UnitCard> unitCards { get; set; } = new List<UnitCard>();

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/Unit_Card.tscn";
	
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

	public void UpdateRecruitment(RecruitmentComponent recruitmentComponent, List<Unit> units, bool IsController){
		if(recruitmentComponent.CurrentlyRecruitedUnits.Count > unitCards.Count)
			AddLabel(recruitmentComponent.CurrentlyRecruitedUnits.Count - unitCards.Count);
		for (int i = 0; i < unitCards.Count; i++)
		{	
			if(recruitmentComponent.CurrentlyRecruitedUnits.Count > i){
				unitCards[i].UpdateProgress(units[recruitmentComponent.CurrentlyRecruitedUnits[i]]);
				unitCards[i].button.Disabled = !IsController;
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
