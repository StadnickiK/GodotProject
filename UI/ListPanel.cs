using Godot;
using System;
using System.Collections.Generic;

public class ListPanel : VBoxContainer
{
    [Export]
    public string Title { get; set; } = "Title";

    Label _titleLabel = null;

    Node _container = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
        Name = Title;
        _titleLabel.Text = Title;
    }

    void GetNodes(){
        _titleLabel = GetNode<Label>("Title");
        _container = GetNode("ItemList/VContainer");
    }

    public void AddListItem(Node item){
        _container.AddChild(item);
    }

    public void RemoveListItem(Node item){
        _container.RemoveChild(item);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
