using Godot;
using System;


public interface ICombatant : IEnterCombatBase, INode
{
    public int Team { get; set; }

    public int CombatantPosition { get; set; }
}

public partial class Combatant : Node3D, ICombatant
{
    // public int Team { get { return Controller.TeamID; } set { Controller.TeamID = value; } }

    public int Team { get; set; } = -1;

    public int CombatantPosition { get; set; }

    public UnitController UnitController { get; set; }

    public Player Controller { get; set; }

    public Combatant(){}

    public Node GetAsNode { get {return this;} }

    public Combatant(Player player)
    {
        Controller = player;
    }

    public override void _Ready()
    {
        base._Ready();
        UnitController = GetNodeOrNull<UnitController>("UnitController");
        if(UnitController == null)
        {
            UnitController = new UnitController();
            AddChild(UnitController);
        }
        var children = GetChildren();
        Controller = GetNode<Player>("Player");
        // if(Controller == null)
        // {
        //     UnitController = new Player();
        //     AddChild(UnitController);
        // }
    }

}
