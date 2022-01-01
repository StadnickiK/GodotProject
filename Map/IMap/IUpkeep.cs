using Godot;
using System;
using System.Collections.Generic;

public interface IUpkeep
{
    Dictionary<string, int> Upkeep { get; set; }
}
