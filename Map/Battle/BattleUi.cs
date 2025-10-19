using Godot;
using System;
using System.Collections.Generic;

public partial class BattleUi : Control
{
    public Button Fight { get; set; }

    Control ConfirmPanel;

    Control TopPanel;

    public ArmyView ArmyView { get; set; }


    public override void _Ready()
    {
        GetNodes();
    }

    private void GetNodes()
    {
        Fight = GetNode<Button>("ConfirmPanel/Fight");
        ConfirmPanel = GetNode<Control>("ConfirmPanel");
        TopPanel = GetNode<Control>("TopPanel");
        ArmyView = GetNode<ArmyView>("ArmyInterface");
        // LeftDeployZone/LCollisionShape3D
    }

    public void UpdateBattleUI(List<Unit> units)
    {
        Show();
        ConfirmPanel.Show();
        ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateArmyList(units, true);
    }

    public void UpdateBattleUI(List<Unit3D> units)
    {
        Show();
        ConfirmPanel.Show();
        ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateArmyList(units, true);
    }

    public void _on_fight_button_up()
    {
        ConfirmPanel.Hide();
    }
}
