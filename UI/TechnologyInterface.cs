using Godot;
using System;

public class TechnologyInterface : Panel
{
    
    Button _acceptButton = null;
    Header _header = null;

    Technology _technology = null;

    ListPanel _listPanel = null;

    [Signal]
    public delegate void StartResearch(Technology technology);

    void GetNodes(){
        _header = GetNode<Header>("Header");
        _listPanel = GetNode<ListPanel>("Details");
        _acceptButton = GetNode<Button>("Build");
    }

    public override void _Ready()
    {
        GetNodes();
        _header.ConnectToButtonUp(this, nameof(_on_ButtonUp));
    }

    public void UpdateInterface(Technology technology){
        _technology = null;
        _listPanel.ClearItems();
        if(technology != null){
            _technology = technology;
            _header.SetTitle(technology.Name);
            var label = new Label();
            label.Text = "\nBuild Cost\n";
            _listPanel.AddListItem(label);
            foreach(var resName in technology.BuildCost.Keys){
                label = new Label();
                label.Text = resName + " " + technology.BuildCost[resName];
                _listPanel.AddListItem(label);
            }
            label = new Label();
            label.Text = "\nConstruction time: " + technology.BuildTime;
            _listPanel.AddListItem(label);
        }
    }

    void _on_BuildingInterface_gui_input(InputEvent e){
        if(e is InputEventScreenDrag drag)
            RectPosition += drag.Relative;
    }

    void _on_ButtonUp(){
        Visible = false;
        _listPanel.ClearItems();
    }

    void _on_Build_button_up(){
        if(_technology != null){
            EmitSignal(nameof(StartResearch), _technology);
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
