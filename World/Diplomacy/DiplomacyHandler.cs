using Godot;
using System;
using System.Collections.Generic;

public partial class DiplomacyHandler : Node
{
    public Player Player { get; set; }

    public Dictionary<Player, DiplomaticRelation> DiplomaticRelations { get; set; }

    public delegate void RaiseEvent(Player player, DiplomaticEvent diplomaticEvent);

    public event RaiseEvent EventOccured;

    public event RaiseEvent EventExpired;

    public void OnDiplomaticEventOccured(Player player, DiplomaticEvent diplomaticEvent)
    {
        
    }

    public void OnDiplomaticEventExpired(Player player, DiplomaticEvent diplomaticEvent)
    {
        DiplomaticRelations[player].RemoveEvent(diplomaticEvent);
    }

    public void RaiseDiplomaticEvent(DiplomaticEvent diplomaticEvent)
    {
        EventOccured?.Invoke(Player, diplomaticEvent);
    }

    public DiplomaticRelation GetDiplomaticRelation(Player player)
    {
        return DiplomaticRelations[player];
    }
}
