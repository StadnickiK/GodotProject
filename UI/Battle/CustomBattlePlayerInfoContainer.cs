using Godot;
using System;

public partial class CustomBattlePlayerInfoContainer : GridContainer
{
    public OptionButton Team { get; set; }

    public OptionButton BattlefieldPosition { get; set; }

    public override void _Ready()
    {
        Team = GetNode<OptionButton>("Team");
        BattlefieldPosition = GetNode<OptionButton>("Position");
    }

}
