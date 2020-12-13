using Godot;
using System;
using System.Collections.Generic;

public class RightPanel : Panel
{
    
    Label _title = null;
    
    OverviewPanel _overwievPanel = null;

    [Export]
    public string Title { get; set; } = "Title";

    [Export]
    public List<string> Options { get; set; } = new List<string>();

    [Signal]
    public delegate void LookAtObject(Node node);

    void GetNodes(){
        _title = GetNode<Label>("Vertical/Title");
        _overwievPanel = GetNode<OverviewPanel>("OverviewPanel");
    }

    public void UpdateRightPanel(Player player){
        var panelName = "Planets";
        //_overwievPanel.DisconnectToGuiInputEvent(this, panelName, nameof(_on_GuiInputEvent));
        _overwievPanel.ClearPanel(panelName);
        foreach(PhysicsBody body in player.MapObjects){
            if(body is Planet){
                var planet = (Planet)body;
                var label = new Label();
                label.Name = planet.PlanetName;
                label.Text = planet.PlanetName;
                _overwievPanel.AddNodeToPanel(panelName, label);
            }
        }
        _overwievPanel.ConnectToGuiInputEvent(this, panelName, nameof(_on_GuiInputEvent));
    }

    public void _on_GuiInputEvent(Node node){
        GD.Print(node.Name);
        EmitSignal(nameof(LookAtObject), node);
    }

    void InitOverviewPanel(){
        foreach(string s in Options){
            _overwievPanel.AddPanel(s);
            _overwievPanel.ConnectToGuiInputEvent(this, s, nameof(_on_GuiInputEvent));
        }
    }



    public override void _Ready()
    {
        GetNodes();
        _title.Text = Title;
        InitOverviewPanel();
    }

    public void ConnectToLookAt(Node node, string methodName){
        Connect(nameof(LookAtObject), node, methodName);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
