using Godot;
using System;
using System.Collections.Generic;

public class UnitInfoPanel : Panel
{
    Header _header = null;

    PackedScene _itemScene = null;
    
    OverviewPanel _overviewPanel = null;

    [Export]
    public string Title { get; set; } = "Title";

    [Export]
    string _itemPath = "res://UI/MapObjectLabel.tscn";

    [Export]
    public List<string> Options { get; set; } = new List<string>();

    [Signal]
    public delegate void ChangeStance(Node node, string stance);

    Node Selected = null;

void GetNodes(){
        _header = GetNode<Header>("VBoxContainer/Header");
        _header.HButton.Visible = false;
        _overviewPanel = GetNode<OverviewPanel>("VBoxContainer/OverviewPanel");
    }

    void ClearOverviewPanel(){
        foreach(string item in Options)
        {
            _overviewPanel.ClearPanel(item);
        }
    }

    public void UpdatePanel(PhysicsBody body){
        if(body != null){
            Selected = body;
            _header.SetTitle(body.Name);
            ClearOverviewPanel();
            if(body is Ship ship){
                //CreateStancePanel(ship);
                UpdateOverview(ship);
                UpdateDetails(ship);
            }
        }else{
            ClearOverviewPanel();
            Selected = null;
            Visible = false;
        }
    }

    void UpdateDetails(Ship ship){
        if(ship != null){
            var label = new Label(); 
            label.Text = "Name / HP / Attack / Defence \n";
            _overviewPanel.AddNodeToPanel("Details", label);
            foreach(Unit unit in ship.Units){
                label = new Label(); 
                // label.Text = unit.Name +" " + unit.Stats["HitPoints"].CurrentValue +" "+ unit.Stats["Attack"].CurrentValue + " " + unit.Stats["Defence"].CurrentValue;
                _overviewPanel.AddNodeToPanel("Details", label);
            }
        }
    }

    void UpdateOverview(Ship ship){
        var label = new Label();
        label.Text = "Controller: ";     
        if(ship.Controller != null){
            label.Text += "Player "+ship.Controller.PlayerID;
        }
        _overviewPanel.AddNodeToPanel("Overview", label); 
        foreach(BaseStat stat in ship.Stats){
                label = new Label();
                label.Text = stat.StatName + " " + stat.BaseValue;
                _overviewPanel.AddNodeToPanel("Overview", label); 
        }
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
        _overviewPanel.AddNodeToPanel(panelName, node);
    }

    void CreateStancePanel(Ship ship){
        if(ship.GetParent().Name == "Orbit"){
            var button = new Label();
            button.Name = "Colonize";
            button.Text = "Colonize";
            _overviewPanel.AddNodeToPanel("Stance", button);
            button = new Label();
            button.Name = "Conquer";
            button.Text = "Conquer";
            _overviewPanel.AddNodeToPanel("Stance", button);
            _overviewPanel.ConnectToGuiInputEvent(this, "Stance", nameof(_on_Stance_GuiInputEvent));
        }
    }

    void _on_Stance_GuiInputEvent(InputEvent input, Node node){
        if(input is InputEventMouseButton button){
            if(button.ButtonIndex == (int)ButtonList.Left){
                if(node is Label label){
                    EmitSignal(nameof(ChangeStance), Selected, label.Text);
                }
            }
        }
    }

    void InitOverviewPanel(){
        foreach(string s in Options){
            _overviewPanel.AddPanel(s);
        }
    }

    void _on_Button_Up(){
        Visible = false;
    }

    public void ConnectToChangeStance(Node node, string methodName){
        Connect(nameof(ChangeStance), node, methodName);
    }

    public override void _Ready()
    {
        GetNodes();
        _header.SetTitle(Title);
        if(_itemPath != null){
            //_itemScene = (PackedScene)ResourceLoader.Load(_itemPath);
        }
        InitOverviewPanel();
        _header.ConnectToButtonUp(this, nameof(_on_Button_Up));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
