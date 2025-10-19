using Godot;
using System;

public partial class ArmyView : VBoxContainer
{
    private bool showInfoPanel = true;


    public ArmyInterfaceContainer ArmyInterfaceContainer { get; set; }

    public InfoPanel InfoPanel { get; set; }

    [Export]
    public bool ShowInfoPanel { get => showInfoPanel; set { showInfoPanel = value; if(InfoPanel != null) InfoPanel.Visible = value; } }

    public override void _Ready()
    {
        ArmyInterfaceContainer = GetNode<ArmyInterfaceContainer>("ArmyInterfaceContainer");
        InfoPanel = GetNode<InfoPanel>("InfoPanel");
        InfoPanel.Visible = ShowInfoPanel;
    }

}
