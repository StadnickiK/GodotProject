using Godot;
using System;

public interface IEnterCombat : IMapObjectController, INode
{

    public delegate void EnterCombatEventHandler(IEnterCombat attacker, IEnterCombat enemy, Node parent);

    public event EnterCombatEventHandler EnterCombat;

    public UnitController UnitController { get; set; }

    public InputController InputController { get; set; }

    public Vector3 GlobalPosition { get; set; }
}
