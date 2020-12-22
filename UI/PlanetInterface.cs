using Godot;
using System;
using System.Collections.Generic;


public class PlanetInterface : Panel
{


    Button _closeButton = null;

    Header _header = null;

    Planet _planet = null;

    OverviewPanel _overviewPanel = null;

    [Export]
    public string Title { get; set; } = "Title";

    [Export]
    public List<string> Options { get; set; } = new List<string>();

    [Signal]
    public delegate void SelectObjectInOrbit(Planet planet, Node node);

    void GetNodes(){
        _closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
        _header = GetNode<Header>("VBoxContainer/Header");
        _overviewPanel = GetNode<OverviewPanel>("VBoxContainer/OverviewPanel");
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
        _closeButton.Connect("button_up", this, nameof(_on_XButton_button_up));
        InitOverviewPanel();
    }

    void ClearPlanetInterface(){
        foreach(string s in Options){
            _overviewPanel.ClearPanel(s);
        }
    }

    public void UpdatePlanetInterface(Planet planet){
        if(planet != null){
            _planet = planet;
            SetTitle(planet.Name);
            ClearPlanetInterface();
            foreach(PhysicsBody body in planet.Orbit.GetChildren()){
                var label = new Label();
                label.Name = label.Text = body.Name;
                label.SetMeta(label.Name, body);
                var node = (Node)label.GetMeta(label.Name);
                GD.Print("m "+node.Name);
                _overviewPanel.AddNodeToPanel("Orbit", label);
            }
            _overviewPanel.ConnectToGuiInputEvent(this, "Orbit", nameof(_on_LabelGuiInputEvent));
        }
    }

    public void _on_LabelGuiInputEvent(InputEvent input, Node node){
        if(input is InputEventMouseButton button){
            if(button.ButtonIndex == (int)ButtonList.Left){
                GD.Print("p "+node.Name);
                EmitSignal(nameof(SelectObjectInOrbit), _planet, node);
            }
        }
    }

    public void ConnectToSelectObjectInOrbit(Node node, string methodName){
        Connect(nameof(SelectObjectInOrbit), node, methodName);
    }

    void _on_XButton_button_up(){
        Visible = false;
    }

}
