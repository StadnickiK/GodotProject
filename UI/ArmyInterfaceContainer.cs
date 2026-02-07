using Godot;
using System;

public partial class ArmyInterfaceContainer : HBoxContainer
{
    public Button BuildButton { get; set; }

    public ArmyRecruitment ArmyRecruitment { get; set; }

    public ArmyPanel ArmyPanel { get; set; }

    public override void _Ready()
    {
        BuildButton = GetNode<Button>("BuildButton");
        ArmyRecruitment = GetNode<ArmyRecruitment>("ArmyRecruitment");
        ArmyPanel = GetNode<ArmyPanel>("ArmyPanel");
    }
}
