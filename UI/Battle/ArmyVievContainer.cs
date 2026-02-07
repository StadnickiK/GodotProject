using Godot;
using System;

public partial class ArmyVievContainer : VBoxContainer
{
    public Header Header { get; set; }

    public ArmyView ArmyView { get; set; }

    public override void _Ready()
    {
        Header = GetNode<Header>("Header");
        ArmyView = GetNode<ArmyView>("ArmyView");
    }

}
