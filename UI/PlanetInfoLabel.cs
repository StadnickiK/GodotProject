using Godot;
using System;
using System.Collections.Generic;

public partial class PlanetInfoLabel : Control
{
	public GridContainer Container { get; set; }

	Planet planet;

	public List<Label> Labels { get; set; } = new List<Label>();

	[Export]
	public string LabelPath { get; set; } = "";
	public override void _Ready()
	{
		Container = GetNode<GridContainer>("Container");
		foreach (var label in Container.GetChildren()){
			label.QueueFree(); // remove test values
		}
	}

	void AddLabel(Resource resource, int quantity = -1){
		var label = new Label();
		label.Text = resource.IconPlaceholder;
		label.AddThemeFontSizeOverride("font_size", 128);
		if (quantity > -1) label.Text = resource.IconPlaceholder +": "+ quantity;
		Labels.Add(label);
		Container.AddChild(label);
	}

	public void InitResources(Planet planet, List<Resource> resources){
		this.planet = planet;
		foreach(int resID in planet.ResourcesManager.Resources.Keys){
			AddLabel(resources[resID]);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
