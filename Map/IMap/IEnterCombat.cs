using Godot;
using System;

public interface IEnterCombatBase : IMapObjectController, INode
{
    public UnitController UnitController { get; set; }

    public Vector3 GlobalPosition { get; set; }
}

public interface IEnterCombat : IEnterCombatBase
{

    public delegate void EnterCombatEventHandler(IEnterCombat attacker, IEnterCombat enemy, Node parent);

    public event EnterCombatEventHandler EnterCombat;

    //public UnitController UnitController { get; set; }

    public InputController InputController { get; set; }    
}
