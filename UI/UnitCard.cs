using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class UnitCard : Control, IUpdateStat
{

	public Unit Unit { get; set; }

	IStatManager statManager;

	public int Index { get; set; }

	public ProgressBar Health { get; set; }

	public ProgressBar Shield { get; set; }

	public ProgressBar Progress { get; set; }

	public Label Label { get; set; }

	public Button button { get; set; }
    public Array<string> StatNames { get; set; }
    public System.Collections.Generic.Dictionary<string, IStat> Stats { get; set; }

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

	public void UpdateCard(Unit unit)
	{
		if (Unit != null) Unit.StatManager.StatChanged -= UpdateStat;
		Unit = unit;
		unit.StatManager.StatChanged += UpdateStat;
		var health = unit.GetStat(GlobalStatNames.Health);
		var shield = unit.GetStat(GlobalStatNames.Shield);
		UpdateCard(health, shield);
	}

	public void UpdateCard(Unit3D unit)
	{
		if (statManager != null) statManager.StatManager.StatChanged -= UpdateStat;
		statManager = unit;
		unit.StatManager.StatChanged += UpdateStat;
		var health = statManager.StatManager.GetStat(GlobalStatNames.Health);
		var shield = statManager.StatManager.GetStat(GlobalStatNames.Shield);
		UpdateCard(health, shield);
	}

	void UpdateCard(IStat health, IStat shield = null)
	{
		Modulate = new Color(1, 1, 1, 1);
		Progress.Hide();
		Label.Hide();
		if (health != null)
		{
			UpdateHealth(health);
			//Health._Draw();
		}
		if (shield != null)
		{
			Shield.Show();
			UpdateShield(shield);
			//Shield._Draw();
		}
		else
		{
			Shield.Hide();
		}
	}

	public void UpdateProgress(Unit unit)
	{
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
				UpdateShield(stat);
				break;
			case "Health":
				UpdateHealth(stat);
				break;
		}
	}

	void UpdateShield(IStat stat)
	{
		Shield.MinValue = stat.MinValue;
		Shield.MaxValue = stat.MaxValue;
		Shield.Value = stat.CurrentValue;
	}
	void UpdateHealth(IStat stat)
	{
		if (stat.CurrentValue <= stat.MinValue)
		{
			Hide();
		}else
		{
			Health.MinValue = stat.MinValue;
			Health.MaxValue = stat.MaxValue;
			Health.Value = stat.CurrentValue;
		}
			
	}
}
