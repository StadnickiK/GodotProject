using Godot;
using System;
using Godot.Collections;

public partial class BattlePanel : CanvasLayer
{

    [Export]
    public string Title { get; set; } = "Space battle";

    SpaceBattle _battle = null;

    BattleArmyContainerHandler Left;

    BattleArmyContainerHandler Right;

    public CenterBattlePanel Center { get; set; }

    void GetNodes()
    {
        Left = GetNode<BattleArmyContainerHandler>("Left/ArmyPanel/BattleArmyContainerHandler");
        Right = GetNode<BattleArmyContainerHandler>("Right/ArmyPanel/BattleArmyContainerHandler");
        Center = GetNode<CenterBattlePanel>("CenterBattlePanel");
    }
    public void ConnectContainers(UI ui)
    {
        Left.ConnectUnitCards(ui);
        Right.ConnectUnitCards(ui);
    }

    public override void _Ready()
    {
        GetNodes();
    }

    public void UpdatePanel(SpaceBattle battle)
    {
        if (_battle == null)
        {
            _battle = battle;
            Center.AutoFight.ButtonUp += _battle.AutoFight;
            Center.EndFight.ButtonUp += _battle.AcceptResult;
            _battle.BattleFinished += UpdatePanelPostBattle;
        }
        Center.Update(battle.Attackers[0], battle.Defenders[0]);
        Left.UpdateArmyContainers(battle.Attackers);
        Right.UpdateArmyContainers(battle.Defenders);
    }

    public void UpdatePanelPostBattle(SpaceBattle battle)
    {
        Center.UpdatePostBattle(battle.Attackers[0], battle.Defenders[0]);
        Left.UpdateArmyContainers(battle.Attackers);
        Right.UpdateArmyContainers(battle.Defenders);
    }

}
