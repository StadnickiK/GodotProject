using Godot;
using System;

public interface ITargetSpottedListener
{
    public void OnTargetSpotted(ITargetable targetable);

    public void OnTargetLost(ITargetable targetable);
}
