using Godot;
using System;
using System.Collections.Generic;

public class OverviewPanel : VBoxContainer
{

    List<Button> buttons = new List<Button>();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){
        GetNodes();
    }

    void GetNodes(){
        var options = GetNode("Options");
        foreach(Node n in options.GetChildren()){
            if(n is Button){
                var b = (Button)n;
                buttons.Add(b);
                Godot.Collections.Array array = new Godot.Collections.Array();
                array.Add(b);
                b.Connect("button_up",this, nameof(_on_Button_Up), array);

            }
        }
    }

    public void _on_Button_Up(Node node){
        if(node is Button){
            var button = (Button)node;
            foreach(Button b in buttons){
                if(b.Disabled){
                    b.Disabled = false;
                    GetNode<CanvasItem>(b.Name).Visible = false;
                }
            }
            button.Disabled = true;
            GetNode<CanvasItem>(button.Name).Visible = true;
        }
        GD.Print(node.Name);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
