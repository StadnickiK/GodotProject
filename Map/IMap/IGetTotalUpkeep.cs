using Godot;
using System;
using System.Collections.Generic;

public interface IGetTotalUpkeep
{
    void GetTotalUpkeep(Dictionary<string ,int> costs);
}
