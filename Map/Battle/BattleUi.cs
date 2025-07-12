using Godot;
using System;

public partial class BattleUi : Control
{
    public Button Fight { get; set; }

    Control ConfirmPanel;

    Control TopPanel;

    public ArmyInterface ArmyInterface { get; set; }


    public override void _Ready()
    {
        GetNodes();
    }

    private void GetNodes()
    {
        Fight = GetNode<Button>("ConfirmPanel/Fight");
        ConfirmPanel = GetNode<Control>("ConfirmPanel");
        TopPanel = GetNode<Control>("TopPanel");
        ArmyInterface = GetNode<ArmyInterface>("ArmyInterface");
        // LeftDeployZone/LCollisionShape3D
    }

    public void UpdateBattleUI(SpaceBattle SpaceBattle)
    {
        Show();
        ArmyInterface.UpdateArmyPanel(SpaceBattle.GetLocalUnits());
    }
}
