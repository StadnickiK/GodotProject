using Godot;
using System;

public partial class TopLeftPanel : Control
{

	public TechnologyPanel TechPanel { get; set; }

	public Player _Player { get; set; } = null;

	public Node WorldTechnology { get; set; } = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TechPanel = GetNode<TechnologyPanel>("TechnologyPanel");
	}

	void _on_Technology_button_up(){
		//overview.Visible = true;
		//UpdateTechnology();
		TechPanel.Visible = true;
		if(TechPanel._Player == null) TechPanel._Player = _Player;
		TechPanel.UpdatePanel(WorldTechnology);
	}

 public override void _Process(double delta)
 {
	if(_Player != null){
		 if(_Player.Research.HasConstruct){
			 if(TechPanel._Player == null) TechPanel._Player = _Player;
			 TechPanel.UpdateResearch(WorldTechnology, _Player.Research.CurrentConstruction());
		 }
	} 
 }

}
