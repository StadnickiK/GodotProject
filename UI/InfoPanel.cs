using Godot;
using System;
using System.Collections.Generic;

public partial class InfoPanel : HBoxContainer
{
    public Label Controller { get; set; }

    public Label Upkeep { get; set; }

    // public Dictionary<int, Resource> Resources { get; set; }

    public override void _Ready()
	{
		Controller = GetNodeOrNull<Label>("Controller");
        Upkeep = GetNodeOrNull<Label>("Upkeep");
	}
}
