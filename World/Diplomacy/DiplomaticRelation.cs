using Godot;
using System;
using System.Collections.Generic;

public class DiplomaticRelation
{
    public enum DiplomaticStatus
    {
        Enemy,
        Neutral,
        DefensiveAlly,
        MilitaryAlly,
        Ally
    }
    public DiplomaticStatus Status { get; set; } = DiplomaticStatus.Enemy;

    public float RelationshipScore { get; set; } = 0;

    public HashSet<DiplomaticEvent> DiplomaticEvents { get; set; } = new HashSet<DiplomaticEvent>();

    public delegate void DiplomaticRelationChangedHandler(DiplomaticRelation diplomaticRelation);

    public event DiplomaticRelationChangedHandler DiplomaticRelationChanged;

    public void AddEvenet(DiplomaticEvent diplomaticEvent)
    {
        DiplomaticEvents.Add(diplomaticEvent);
        RelationshipScore += diplomaticEvent.Score;
        DiplomaticRelationChanged?.Invoke(this);
    }

    public void RemoveEvent(DiplomaticEvent diplomaticEvent)
    {
        DiplomaticEvents.Remove(diplomaticEvent);
        RelationshipScore -= diplomaticEvent.Score;
        DiplomaticRelationChanged?.Invoke(this);
    }
}
