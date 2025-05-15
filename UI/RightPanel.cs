using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;

public partial class RightPanel : Control
{
    
    Label _title = null;

    PackedScene _itemScene = null;

    [Export]
    public string Title { get; set; } = "Title";

    [Export]
    string ItemScenePath = "res://UI/MapObjectLabel.tscn";

    public List<MapObjectLabel> ShipLabels { get; set; } = new List<MapObjectLabel>();

    public List<MapObjectLabel> PlanetLabels { get; set; } = new List<MapObjectLabel>();

    Node Ships;

    Node Planets;

    public CameraGimbal WorldCamera { get; set; }

    [Signal]
    public delegate void LookAtObjectEventHandler(Node node);

    void GetNodes()
    {
        _title = GetNode<Label>("Vertical/Title");
        Ships = GetNode("Vertical/TabContainer/Ships/Ships");
        Planets = GetNode("Vertical/TabContainer/Planets/Planets");
        
        // _overviewPanel = GetNode<OverviewPanel>("OverviewPanel");
    }

    public void UpdateRightPanel(Player player)
    {
        // var panelName = "Planets";
        //_overviewPanel.DisconnectToGuiInputEvent(this, panelName, nameof(_on_GuiInputEvent));
        // _overviewPanel.ClearPanel(panelName);
        // _overviewPanel.ClearPanel("Fleets");        
        UpdatePlanets(player);
        UpdateShips(player);
        // _overviewPanel.ConnectToGuiInputEvent(this, panelName, nameof(_on_LabelGuiInputEvent));
        // _overviewPanel.ConnectToGuiInputEvent(this, "Fleets", nameof(_on_LabelGuiInputEvent));
    }

    public void UpdatePlanets(Player player){
        if(player.Planets.Count > PlanetLabels.Count)
            AddLabel(Planets, PlanetLabels, player.Planets.Count-PlanetLabels.Count);
        
        for (int i = 0; i < PlanetLabels.Count; i++){
            if(i < player.Planets.Count){
                if(player.Planets[i] != null){
                    if(player.Planets[i] is Planet planet){
                        PlanetLabels[i].Show();
                        PlanetLabels[i].UpdateLabel(planet, planet.Name, "");
                    }
                }
            }else{
                PlanetLabels[i].Hide();
            }
        }
    }

    public void UpdateShips(Player player){
        if(player.Ships.Count > ShipLabels.Count)
            AddLabel(Ships, ShipLabels, player.Ships.Count-ShipLabels.Count);
        for (int i = 0; i < ShipLabels.Count; i++){
            if(i < player.Ships.Count){
                if(player.Ships[i] != null){
                    if(player.Ships[i] is Ship ship){
                        ShipLabels[i].Show();
                        ShipLabels[i].UpdateLabel(ship, ship.Name, "");
                    }
                }
            }else{
                ShipLabels[i].Hide();
            }
        }
    }

    MapObjectLabel GetLabel(Node3D node){
        var label = (MapObjectLabel)_itemScene.Instantiate();
        label.Title = node.Name;
        label.Name = node.Name;
        label.MapObject = node;
        return label;
    }

    void AddLabel(Node parent, List<MapObjectLabel> mapObjectLabels, int count = 1){
        for (int i = 0; i < count; i++)
        {
            var label = (MapObjectLabel)_itemScene.Instantiate();
            parent.AddChild(label);
            mapObjectLabels.Add(label);
            label.Button.ButtonUp += () => WorldCamera.LookAt(label.MapObject.Position);
        }
    }

    public void _on_LabelGuiInputEvent(InputEvent input, Node node){
        if(input is InputEventMouseButton button){
            if(button.ButtonIndex == MouseButton.Left){
                if(node != null)
                    EmitSignal(nameof(LookAtObject), node);
            }
        }
    }

    void InitPanel(){
        foreach (var node in Planets.GetChildren())
        {
            if (node is MapObjectLabel label)
            {
                PlanetLabels.Add(label);
                label.Button.ButtonUp += () => WorldCamera.LookAt(label.MapObject.Position);
            }
        }
        foreach (var node in Ships.GetChildren())
        {
            if (node is MapObjectLabel label)
            {
                ShipLabels.Add(label);
                label.Button.ButtonUp += () => WorldCamera.LookAt(label.MapObject.Position);
            }
        }
    }

    public override void _Ready()
    {
        GetNodes();
        _title.Text = Title;
        if(ItemScenePath != null){
            _itemScene = (PackedScene)ResourceLoader.Load(ItemScenePath); 
        }
        InitPanel();
    }

    // public void ConnectToLookAt(Node node, string methodName){
    //     Connect(nameof(LookAtObject), new Callable(node, methodName));
    // }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
