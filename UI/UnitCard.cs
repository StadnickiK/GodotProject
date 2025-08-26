using Godot;
using System;
using System.Collections.Generic;

public partial class UnitCard : Control, IUpdateStat
{

	public Unit Unit { get; set; }

	public int Index { get; set; }

	public ProgressBar Health { get; set; }

	public ProgressBar Shield { get; set; }

	public ProgressBar Progress { get; set; }

	public Label Label { get; set; }

	public Button button { get; set; }
	public HashSet<string> StatNames { get; set; } = new HashSet<string>();

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
		if(Unit != null) Unit.StatManager.StatChanged -= UpdateStat;
		Unit = unit;
		unit.StatManager.StatChanged += UpdateStat;
		var health = unit.GetStat(GlobalStatNames.Health);
		Modulate = new Color(1, 1, 1, 1);
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
		Modulate = new Color(1, 1, 1, 0.5f);
		Progress.Show();
		Label.Show();
		Label.Text = unit.CurrentTime + "/" + unit.BuildTime;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void UpdateStat(IStat stat)
    {
		switch (stat.Name)
		{
			case "Shield":
				Shield.Value = stat.CurrentValue;
				break;
			case "Health":
				Health.Value = stat.CurrentValue;
					break;
		}
    }
}
