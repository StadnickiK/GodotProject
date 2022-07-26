using Godot;
using System;

public class UnitTransferPanel : PanelContainer
{
    
    Node LeftList;

    Node RightList;

    Ship left;

    Ship right;

    Header _header;

    void GetNodes(){
        LeftList = GetNode("VBoxContainer/HBoxContainer/LeftScroll/Left");
        RightList = GetNode("VBoxContainer/HBoxContainer/RightScroll/Right");
        _header = GetNode<Header>("VBoxContainer/Header");
    }

    public override void _Ready()
    {
        GetNodes();
        _header.ConnectToButtonUp(this, nameof(_on_closeButton_Up));
    }

    public void UpdateTransferPanel(Ship a , Ship b){
        left = a;
        right = b;
        UpdateSide(a, LeftList);
        UpdateSide(b, RightList);
    }

    void UpdateSide(Ship ship, Node side){
        ClearPanel(side);
        int i = 0;
        foreach(Unit unit in ship.Units.GetChildren()){
            Button b = new Button();
            b.Text = unit.Name;
            b.Name = "TransferButton " + side.Name + i;
            Godot.Collections.Array array = new Godot.Collections.Array();
            array.Add(unit);
            b.Connect("button_up", this, nameof(_on_UnitButton_Up), array);
            i++;
            side.AddChild(b); 
        }
    }

    void ClearPanel(Node node){
        foreach(Node child in node.GetChildren())
            child.QueueFree();
    }

    void _on_OpenTransferPanel(Ship left, Ship right){
        Visible = true;
        UpdateTransferPanel(left, right);
    }

    void _on_UnitButton_Up(Unit unit){
        var ship = (Ship)unit.GetParent().GetParent();
        if(ship.Units.GetChildren().Count > 1){
            unit.GetParent().RemoveChild(unit);
            if(ship != left){
                left.Units.AddChild(unit);
                UpdateTransferPanel(left, ship);
            }else{
                right.Units.AddChild(unit);
                UpdateTransferPanel(ship, right);
            }
        }
    }

    void _on_closeButton_Up(){
        Visible = false;
    }

    void _on_Merge_button_up(){
        left.Merge(right);
        Visible = false;
    }

    void _on_ConfrimButton_button_up(){
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
