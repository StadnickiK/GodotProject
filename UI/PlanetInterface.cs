using Godot;
using System;
using System.Collections.Generic;


public class PlanetInterface : Panel
{


    Button _closeButton = null;

    Header _header = null;

    Planet _planet = null;

    OverviewPanel _overviewPanel = null;

    BuildingInterface _buildingInterface = null;

    [Export]
    public string Title { get; set; } = "Title";

    [Export]
    public List<string> Options { get; set; } = new List<string>();

    [Export]
    public string ItemScenePath { get; set; } = "res://UI/BuildingLabel.tscn";

    PackedScene ItemScene = null;

    BuildingLabel _selectedBuilding = null;

    [Signal]
    public delegate void SelectObjectInOrbit(Planet planet, Node node);

    void GetNodes(){
        _closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
        _header = GetNode<Header>("VBoxContainer/Header");
        _overviewPanel = GetNode<OverviewPanel>("VBoxContainer/OverviewPanel");
        _buildingInterface = GetNode<BuildingInterface>("BuildingInterface");
    }

    void ConnectSignals(){
        _closeButton.Connect("button_up", this, nameof(_on_XButton_button_up));
        _buildingInterface.ConnecToStartConstruction(this, nameof(_on_StartConstruction));
    }

    public void SetTitle(string title){
        _header.SetTitle(title);
    }

    void InitOverviewPanel(){
        foreach(string s in Options){
            _overviewPanel.AddPanel(s);
        }
    }

    public override void _Ready()
    {
        GetNodes();
        ConnectSignals();
        InitOverviewPanel();
        ItemScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
    }

    void ClearPlanetInterface(){
        foreach(string s in Options){
            _overviewPanel.ClearPanel(s);
        }
    }

    public void UpdatePlanetInterface(Planet planet, List<Building> allBuildings){
        if(planet != null){
            _planet = planet;
            SetTitle(planet.Name);
            ClearPlanetInterface();
            if(planet.Vision){
                UpdateOverview(planet);
                foreach(PhysicsBody body in planet.Orbit.GetChildren()){
                    var label = new Label();
                    label.Name = label.Text = body.Name;
                    label.SetMeta(label.Name, body);
                    var node = (Node)label.GetMeta(label.Name);
                    //GD.Print("m "+node.Name);
                    _overviewPanel.AddNodeToPanel("Orbit", label);
                }
                foreach(Building building in planet.Buildings){
                    var label = (BuildingLabel)ItemScene.Instance();
                    label.SetMeta(building.Name, building);
                    label.Name = building.Name;
                    label.Text = building.Name;
                    _overviewPanel.AddNodeToPanel("Buildings", label);
                    if(label.Progress != null){                             // ProgressBar was null, bcs label.getnodes method is executed when label enters the tree, so it has to be done after AddNodeToPanel
                        label.Progress.Value = building.CurrentTime;
                        label.Progress.MaxValue = building.BuildTime;
                        //GD.Print(label.Progress.MaxValue);
                    }
                }
                UpdateAllBuildings(allBuildings);
                _overviewPanel.ConnectToGuiInputEvent(this, "Orbit", nameof(_on_LabelGuiInputEvent));
                _overviewPanel.ConnectToGuiInputEvent(this, "Buildings", nameof(_on_BuildingLabelGuiInputEvent));
            }
        }
    }

    void UpdateOverview(Planet planet){
        var label = new Label();
        if(planet.PlanetOwner != null){
            label.Name = planet.PlanetOwner.Name;
        }
        label.Text = "Planet controller: " + planet.PlanetOwner.Name;
        _overviewPanel.AddNodeToPanel("Overview", label);
    }

    void UpdateAllBuildings(List<Building> buildings){
        foreach(Building building in buildings){
            if(!_planet.Buildings.Contains(building)){
                var label = (BuildingLabel)ItemScene.Instance();
                label.SetMeta(building.Name, building);
                label.Name = building.Name;
                label.Text = building.Name;
                _overviewPanel.AddNodeToPanel("Buildings", label);
                if(label.Progress != null){                             // ProgressBar was null, bcs label.getnodes method is executed when label enters the tree, so it has to be done after AddNodeToPanel
                    label.Progress.Value = building.CurrentTime;
                    label.Progress.MaxValue = building.BuildTime;
                }
            }
        }
    }


    public void _on_LabelGuiInputEvent(InputEvent input, Node node){
        if(input is InputEventMouseButton button){
            if(button.ButtonIndex == (int)ButtonList.Left){
                //GD.Print("p "+node.Name);
                EmitSignal(nameof(SelectObjectInOrbit), _planet, node);
            }
        }
    }

    public void _on_BuildingLabelGuiInputEvent(InputEvent input, Node node){
        if(input is InputEventMouseButton button){
            if(button.ButtonIndex == (int)ButtonList.Left){
                //GD.Print("p "+node.Name);
                if(node is BuildingLabel label){
                    var building = (Building)label.GetMeta(label.Text);
                    if(building != null && _planet != null){
                        _buildingInterface.Visible = true;
                        _buildingInterface.UpdateInterface(building, _planet);
                        _selectedBuilding = label;
                    }
                }
            }
        }
    }

    void _on_XButton_button_up(){
        Visible = false;
        _buildingInterface.Visible = false;
    }

    void _on_StartConstruction(Building building){
        if(building != null && _planet != null){
            _planet.ConstructBuilding(building);
        }
    }

    public void ConnectToSelectObjectInOrbit(Node node, string methodName){
        Connect(nameof(SelectObjectInOrbit), node, methodName);
    }

    public override void _Process(float delta){
        if(Visible){
            if(_planet != null && _selectedBuilding != null){
                if(_planet.Construction != null){
                    _selectedBuilding.Progress.Value = _planet.Construction.CurrentTime;
                }
            }
        }
    }
}
