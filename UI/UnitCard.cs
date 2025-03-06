using Godot;
using System;

public partial class UnitCard : Control
{

	public Unit Unit { get; set; }

	public ProgressBar Health { get; set; }

	public ProgressBar Shield { get; set; }

	public ProgressBar Progress { get; set; }

	public Label Label { get; set; }

	public Button button { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var c = GetChildren()[0];
		Health = c.GetNode<ProgressBar>("Health");
		Shield = c.GetNode<ProgressBar>("Shield");
		button = c.GetNode<Button>("Button");
		Label = GetNode<Label>("Label");
		Progress = GetNode<ProgressBar>("ProgressBar");
	}

	public void UpdateCard(Unit unit){
		Unit = unit;
		var health = unit.GetStat("Health");
		SelfModulate = new Color(1, 1, 1, 1);
		Progress.Hide();
		Label.Hide();
		if(health != null){
			Health.Value = health.CurrentValue;
			Health.MinValue = health.MinValue;
			Health.MaxValue = health.MaxValue;
		}
		
	}

	public void UpdateProgress(Unit unit){
		Show();
		UpdateCard(unit);
		SelfModulate = new Color(1, 1, 1, 0.5f);
		Progress.Show();
		Label.Show();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
