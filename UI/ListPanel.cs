using Godot;
using System;
using System.Collections.Generic;

public class ListPanel : VBoxContainer
{
    [Export]
    public string Title { get; set; } = "Title";

    Label _titleLabel = null;

    Node _items = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
        Name = Title;
        _titleLabel.Text = Title;
    }

    void GetNodes(){
        _titleLabel = GetNode<Label>("Header/Title");
        _items = GetNode("ItemList/Items");
    }

    public void SetTitle(string title){
        _titleLabel.Text = title;
    }

    public Label GetTitle(){
        return _titleLabel;
    }

    public Node GetHeader(){
        return GetNode("Header");
    }

    public Node GetFoot(){
        return GetNode("Foot");
    }

    public void AddListItem(Node item){
        if(!_items.GetChildren().Contains(item)){
            _items.AddChild(item);
        }
    }

    public void AddListItem(Node item, Node node, string methodName, Unit unit){
        if(!_items.GetChildren().Contains(item)){
            _items.AddChild(item);
            if(item is Control control){
                Godot.Collections.Array array = new Godot.Collections.Array();
                array.Add(unit);
                control.Connect("gui_input", node, methodName, array);
                control.MouseFilter = MouseFilterEnum.Stop;
            }
        }
    }

    public void ConnectToOnGuiInput(Node node, string methodName){
        foreach(Node child in _items.GetChildren()){
            if(child is Label control){
                if(!control.IsConnected("gui_input", node, methodName)){
                    Godot.Collections.Array array = new Godot.Collections.Array();
                    array.Add(control);
                    control.Connect("gui_input", node, methodName, array);
                    control.MouseFilter = MouseFilterEnum.Stop;
                }else{
                    //GD.Print("Already connected");
                }
            }
        }
    }

    public void ConnectToEvent(Node node, string methodName, string eventName){
        foreach(Node child in _items.GetChildren()){
            if(child is Label control){
                if(!control.IsConnected(eventName, node, methodName)){
                    Godot.Collections.Array array = new Godot.Collections.Array();
                    array.Add(control);
                    control.Connect(eventName, node, methodName, array);
                    control.MouseFilter = MouseFilterEnum.Stop;
                }else{
                    //GD.Print("Already connected");
                }
            }
        }
    }

    public void DisconnectToOnGuiInput(Node node, string methodName){
        foreach(Node child in _items.GetChildren()){
                    child.Disconnect("gui_input", node, methodName);
        }
    }

    public void RemoveListItem(Node item){
        _items.RemoveChild(item);
    }

    public void ClearItems(){
        foreach(Node n in _items.GetChildren()){
            n.QueueFree();
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
