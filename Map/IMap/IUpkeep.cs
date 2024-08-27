using Godot;
using System;
using Godot.Collections;

public interface IUpkeep
{
    System.Collections.Generic.Dictionary<int, int> Upkeep { get; set; }
}
