using Godot;
using System;
using System.Collections.Generic;

public partial class Populations : Node, IRequirements
{

    [Export]
    public int TotalQuantity { get; set; } = 1000000;

    public Random Rand { get; set; } = null;


    [Export]
     Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> ExportRequirements { get; set; } = new Godot.Collections.Dictionary<int, Godot.Collections.Array<int>>();

     public System.Collections.Generic.Dictionary<int, List<int>> Requirements { get; set; } = new System.Collections.Generic.Dictionary<int, List<int>>();

    public override void _Ready()
    {
        // SetProcess(false);
        InitTotalQuantity();
    }

    public Population GetPopulation(string name){
        return GetNode<Population>(name);
    }

    public void InitTotalQuantity(){
        TotalQuantity = 0;
        foreach(Node node in GetChildren()){
            if(node is Population population){
                TotalQuantity += population.Quantity;
            }
        }
    }

    public void Decimate(float decimator){
        foreach(Node node in GetChildren()){
            if(node is Population population){
                population.Quantity -= (int)((float)population.Quantity*decimator);
            }
        }
    }

}
