using Godot;
using System;

public class OrbitFleetInterface : Panel
{
    ListPanel _listPanel = null;
    Header _header = null;

    void GetNodes(){
        _header = GetNode<Header>("Header");
        _listPanel = GetNode<ListPanel>("Details");
    }

    public override void _Ready()
    {
        GetNodes();
        _header.ConnectToButtonUp(this, nameof(_on_ButtonUp));
    }
    
    void _on_ButtonUp(){
        Visible = false;
        _listPanel.ClearItems();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
