using System.Collections.Generic;
using Godot;

public interface IUpdateStat
{
    public Godot.Collections.Array<string> StatNames { get; set; }
    public void UpdateStat(IStat stat);
}
