using Godot;
using System;
using System.Collections.Generic;

public partial class ListPanel : VBoxContainer
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

    public void GetNodes(){
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
                control.Connect("gui_input", new Callable(node, methodName));
                //control.Connect("gui_input", new Callable(node, methodName), array);
                control.MouseFilter = MouseFilterEnum.Stop;
            }
        }
    }

    public void ConnectToOnGuiInput(Node node, string methodName){
        foreach(Node child in _items.GetChildren()){
            if(child is Label control){
                if(!control.IsConnected("gui_input", new Callable(node, methodName))){
                    Godot.Collections.Array array = new Godot.Collections.Array();
                    //array.Add(control);
                    control.Connect("gui_input", new Callable(node, methodName));
                    
                    //control.Connect("gui_input", new Callable(node, methodName), array);
                    control.MouseFilter = MouseFilterEnum.Stop;
                }else{
                    //GD.Print("Already connected");
                }
            }
        }
    }

    public void ConnectToOnGuiInput(GuiInputEventHandler method){
        foreach(Node child in _items.GetChildren()){
            if(child is Label control){
                    Godot.Collections.Array array = new Godot.Collections.Array();
                    control.GuiInput += method;
                    //control.Connect("gui_input", new Callable(node, methodName), array);
                    control.MouseFilter = MouseFilterEnum.Stop;

            }
        }
    }

    public void ConnectToEvent(PlanetInterface node, string methodName, string eventName){
        foreach(Node child in _items.GetChildren()){
            if(child is BuildingLabel control){
                if(!control.BButton.IsConnected(eventName, new Callable(node, methodName))){
                    //Godot.Collections.Array array = new Godot.Collections.Array();
                    //array.Add(control);
                    //control.BButton.Connect(eventName, new Callable(node, methodName));
                    //var p = (PlanetInterface)node;
                    control.BButton.ButtonUp += () => node._on_BuildingLabelGuiInputEvent(control.RefBuilding, control.BButton.Disabled);
                    //control.BButton.Connect(eventName, new Callable(node, methodName), array);
                    control.MouseFilter = MouseFilterEnum.Stop;
                }else{
                    //GD.Print("Already connected");
                }
            }
        }
    }

    public void DisconnectToOnGuiInput(Node node, string methodName){
        foreach(Node child in _items.GetChildren()){
                    child.Disconnect("gui_input", new Callable(node, methodName));
        }
    }

    public void RemoveListItem(Node item){
        _items.RemoveChild(item);
    }

    public void ClearItems(){
        foreach(Node n in _items.GetChildren()){
            n.Name = "Remove"; // if i need to reuse the name of the node it is better to rename it before using qfree
            n.QueueFree();
        }
    }

    public void HideItems(){
        foreach(Node n in _items.GetChildren()){
            if(n is Control control){
                control.Hide();
            }
        }
    }

    public void ShowItems(){
        foreach(Node n in _items.GetChildren()){
            if(n is Control control){
                control.Show();
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
