using Godot;
using System;
using Godot.Collections;

public partial class RightPanel : Panel
{
    
    Label _title = null;

    PackedScene _itemScene = null;
    
    OverviewPanel _overviewPanel = null;

    [Export]
    public string Title { get; set; } = "Title";

    [Export]
    string _itemPath = "res://UI/MapObjectLabel.tscn";

    [Export]
    public Array<string> Options { get; set; } = new Array<string>();

    [Signal]
    public delegate void LookAtObjectEventHandler(Node node);

    void GetNodes(){
        _title = GetNode<Label>("Vertical/Title");
        _overviewPanel = GetNode<OverviewPanel>("OverviewPanel");
    }

    public void UpdateRightPanel(Player player){
        var panelName = "Planets";
        //_overviewPanel.DisconnectToGuiInputEvent(this, panelName, nameof(_on_GuiInputEvent));
        _overviewPanel.ClearPanel(panelName);
        _overviewPanel.ClearPanel("Fleets");
        foreach(CollisionObject3D body in player.MapObjects){
            if(body != null){
                if(body is Planet planet){
                    CreateOverviewPanelLabel(panelName, planet.Name, planet);
                }else if(body is Ship ship){
                    CreateOverviewPanelLabel("Fleets", ship.Name, ship);
                }
            }else{
                player.RemoveMapObject(body);
            }
        }
        _overviewPanel.ConnectToGuiInputEvent(this, panelName, nameof(_on_LabelGuiInputEvent));
        _overviewPanel.ConnectToGuiInputEvent(this, "Fleets", nameof(_on_LabelGuiInputEvent));
    }

    public void _on_LabelGuiInputEvent(InputEvent input, Node node){
        if(input is InputEventMouseButton button){
            if(button.ButtonIndex == MouseButton.Left){
                if(node != null)
                    EmitSignal(nameof(LookAtObject), node);
            }
        }
    }

    void CreateOverviewPanelLabel(string panelName, string name, Node mapObject){
        Node node = null;
        if(_itemScene != null){
            var label = (MapObjectLabel)_itemScene.Instantiate();
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

    public override void _Ready()
    {
        GetNodes();
        _title.Text = Title;
        if(_itemPath != null){
            //_itemScene = (PackedScene)ResourceLoader.Load(_itemPath);
        }
        InitOverviewPanel();
    }

    public void ConnectToLookAt(Node node, string methodName){
        Connect(nameof(LookAtObject), new Callable(node, methodName));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
