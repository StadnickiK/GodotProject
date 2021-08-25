using Godot;
using System;

public class SmallList : Panel
{

    public VBoxContainer Items = null;

    [Signal]
    public delegate void SelectObject(PhysicsBody body);

    
    void GetNodes(){
        Items = GetNode<VBoxContainer>("Scroll/Items");
    }

    public override void _Ready()
    {
        GetNodes();
    }

    public void AddItem(Node node){
        var button = new Button();
        button.Text = node.Name;
        Items.AddChild(button);
        button.Name = node.Name;
        var arr = new Godot.Collections.Array();
        arr.Add(node);
        button.Connect("button_up", this, nameof(_on_button_up), arr);
    }

    public void RemoveItem(Node node){
        Items.RemoveChild(node);
    }

    public void ClearItems(){
        foreach(Node node in Items.GetChildren()){
            node.QueueFree();
        }
    }

    public void UpdateOrbitInfo(Node orbit){
        ClearItems();
        foreach(Node node in orbit.GetChildren()){
            if(node is Ship ship){
                AddItem(node);
            }
        }
    }

    void _on_button_up(Node node){
        if(node is PhysicsBody body)
            EmitSignal(nameof(SelectObject), body);
    }

}
