using System.Collections.Generic;
using Godot;

public interface IUpdateStat
{
    public HashSet<string> StatNames { get; set; }
    public void UpdateStat(IStat stat);
}
