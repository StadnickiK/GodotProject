using Godot;
using System;

public interface IStatChangedNotifier
{
    public event IStat.StatChangedEventHandler StatChanged;
}
