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
void GetNodes(){
        _header = GetNode<Header>("VBoxContainer/Header");
        _overviewPanel = GetNode<OverviewPanel>("VBoxContainer/OverviewPanel");
    }

    public void UpdatePanel(PhysicsBody body){
        _header.SetTitle(body.Name);
        foreach(string item in Options)
        {
            _overviewPanel.ClearPanel(item);
        }
        CreateOverviewPanelLabel("Overview", body.Name, body);
    }

    public void _on_LabelGuiInputEvent(InputEvent input, Node node){
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

    void InitOverviewPanel(){
        foreach(string s in Options){
            _overviewPanel.AddPanel(s);
        }
    }

    void _on_Button_Up(){
        Visible = false;
    }

    public override void _Ready()
    {
        GetNodes();
        _header.SetTitle(Title);
        if(_itemPath != null){
            //_itemScene = (PackedScene)ResourceLoader.Load(_itemPath);
        }
        InitOverviewPanel();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
