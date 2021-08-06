using Godot;
using System;

public class BuildingLabel : Button
{

    public ProgressBar Progress { get; set; } = null;

    void GetNodes(){
        Progress = GetNode<ProgressBar>("ProgressBar");
    }
    public override void _Ready()
    {
        GetNodes();        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
