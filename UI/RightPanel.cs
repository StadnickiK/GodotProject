using Godot;
using System;
using System.Collections.Generic;

public class RightPanel : Panel
{
    
    Label _title = null;

    PackedScene _itemScene = null;
    
    OverviewPanel _overwievPanel = null;

    [Export]
    public string Title { get; set; } = "Title";

    [Export]
    string _itemPath = "res://UI/MapObjectLabel.tscn";

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
            if(body is Planet planet){
                CreateOverviewPanelLabel(panelName, planet.PlanetName, planet);
            }else if(body is Ship ship){
                CreateOverviewPanelLabel("Fleets", ship.Name, ship);
            }
        }
        _overwievPanel.ConnectToGuiInputEvent(this, panelName, nameof(_on_LabelGuiInputEvent));
    }

    public void _on_LabelGuiInputEvent(InputEvent input, Node node){
        EmitSignal(nameof(LookAtObject), node);
    }

    void CreateOverviewPanelLabel(string panelName, string name, Node mapObject){
        Node node = null;
        if(_itemScene != null){
            var label = (MapObjectLabel)_itemScene.Instance();
            label.Title = name;
            label.Name = name;
            label.MapObject = mapObject;
            node = label;
        }else{
            var label = new Label();
            label.Name = name;
            label.Text = name;
            node = label;
        }
        _overwievPanel.AddNodeToPanel(panelName, node);
    }

    void InitOverviewPanel(){
        foreach(string s in Options){
            _overwievPanel.AddPanel(s);
            //_overwievPanel.ConnectToGuiInputEvent(this, s, nameof(_on_GuiInputEvent));
            //_overwievPanel.ConnectToGuiInputEvent(this, s, nameof(_on_LabelGuiInputEvent));
        }
    }

    public override void _Ready()
    {
        GetNodes();
        _title.Text = Title;
        if(_itemPath != null){
            _itemScene = (PackedScene)ResourceLoader.Load(_itemPath);
        }
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
