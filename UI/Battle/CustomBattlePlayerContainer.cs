using Godot;
using System;

public partial class CustomBattlePlayerContainer : VBoxContainer
{

    [Export]
    public bool XButtonVisible { get; set; } = true;

    [Export]
    public string Title { get; set; }

    [Export]
    public int Team { get; set; }

    [Export]
    public bool IsLocal { get; set; }

    public ArmyView ArmyView { get; set; }

    public CustomBattlePlayerInfoContainer PlayerInfo { get; set; }

    public Header Header { get; set; }

    public Button BuildButton { get; set; }

    public Combatant Combatant { get; set; }

    public int Index { get; set; }

    public override void _Ready()
    {
        ArmyView = GetNode<ArmyView>("HBoxContainer/ArmyView");
        Combatant = GetNode<Combatant>("Combatant");
        BuildButton = ArmyView.ArmyInterfaceContainer.BuildButton;
        Header = GetNode<Header>("Header");
        PlayerInfo = GetNode<CustomBattlePlayerInfoContainer>("HBoxContainer/CustomBattlePlayerInfoContainer");
        if(Title != null && Title?.Length != 0)
            Header.SetTitle(Title);
        Header.HButton.Visible = XButtonVisible;
        ConnectSignals();
        PlayerInfo.Team.Selected = Combatant.Team = Team;
        Combatant.Controller.IsLocal = IsLocal;
    }

    void ConnectSignals()
    {
        PlayerInfo.Team.ItemSelected += _on_TeamChanged;
        PlayerInfo.BattlefieldPosition.ItemSelected += _on_PositionChanged;
        foreach (var item in ArmyView.ArmyInterfaceContainer.ArmyPanel.UnitCards)
        {
            item.button.ButtonUp += () => _on_RemoveUnit(item.Unit);
        }
    }

    public void SetTitle(string title)
    {
        Header.SetTitle(title);
    }

    public void _on_AddUnit(Unit unit)
    {
        Combatant.UnitController.AddUnit(unit);
        ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateArmyList(Combatant.UnitController.UnitsList, true);
    }

    public void _on_RemoveUnit(Unit unit)
    {
        Combatant.UnitController.RemoveUnit(unit);
        ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateArmyList(Combatant.UnitController.UnitsList, true);
    }

    public void ConnectLabels(BuildingInterface buildingInterface)
    {
        foreach (var item in ArmyView.ArmyInterfaceContainer.ArmyPanel.UnitCards)
        {
            item.button.MouseEntered += () => buildingInterface._on_ShowUnitInfo(item.Unit, false);
            item.button.MouseExited += buildingInterface.Hide;
        }
    }

    void _on_TeamChanged(long value)
    {
        Combatant.Team = (int)value;
    }

    void _on_PositionChanged(long value)
    {
        Combatant.CombatantPosition = (int)value;
    }
}
