using Godot;
using System;

public interface IEnterCombat : IMapObjectController
{

    public StringName Name { get; set; }

    public delegate void EnterCombatEventHandler(IEnterCombat attacker, IEnterCombat enemy, Node parent);

    public event EnterCombatEventHandler EnterCombat;

    public UnitController UnitController { get; set; }
}
