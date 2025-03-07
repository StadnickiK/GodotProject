using Godot;
using System;
using System.Collections.Generic;

public partial class UpkeepComponent : Node
{

    public Dictionary<int, int> Upkeep { get; set; } = new Dictionary<int, int>();

    [Export]
    public bool CanGoNegative { get; set; } = false;

    public bool UpkeepChanged { get; set; } = false;

    public void UpdateUpkeep(List<Building> buildings){
        foreach(var building in buildings)
            UpdateUpkeep(building);
    }

    public void UpdateUpkeep(Dictionary<int, int > upkeep){
        foreach(var res in upkeep)
            UpdateUpkeep(res.Key, res.Value);
    }

    public void UpdateUpkeep(int resName , int quantity){
        if(Upkeep.ContainsKey(resName)){
            Upkeep[resName] += quantity;
        }else
        {
            Upkeep.Add(resName, quantity);
        }
    }

    public void UpdateUpkeep(IUpkeep upkeep){
        foreach(var pair in upkeep.Upkeep){
            UpdateUpkeep(pair.Key, pair.Value);
        }
    }

    public void RemoveUpkeep(IUpkeep upkeep){
        foreach(var pair in upkeep.Upkeep){
            RemoveUpkeep(pair.Key, pair.Value);
        }
    }

    public void RemoveUpkeep(Dictionary<int, int > upkeep){
        foreach(var pair in upkeep){
            RemoveUpkeep(pair.Key, pair.Value);
        }
    }

    public void RemoveUpkeep(int resName , int quantity){
        if(Upkeep.ContainsKey(resName)){
            if(CanGoNegative){
                Upkeep[resName] -= quantity;
            }else
                if(Upkeep[resName] - quantity > 0){
                    Upkeep[resName] -= quantity;
                }else
                {
                    Upkeep[resName] = 0;
                }
        }else
        {
            Upkeep.Add(resName, 0);
        }
    }
}
