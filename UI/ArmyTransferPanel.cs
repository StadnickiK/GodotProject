using Godot;
using System;
using System.Collections.Generic;

public partial class ArmyTransferPanel : VBoxContainer
{
    public ArmyPanel TransferArmy { get; set; }

    public Button ConfrimButton { get; set; }

    public override void _Ready()
    {
        TransferArmy = GetNode<ArmyPanel>("TransferArmy");
        ConfrimButton = GetNode<Button>("Confirm");
    }

    public void UpdateTransferPanel(List<Unit> transferArmy){
        Show();
        TransferArmy.UpdateArmyList(transferArmy, true);
    }
}
