using Godot;
using System;

public partial class BuildingInterface : Panel
{
    Label _title = null;

    RichTextLabel _desc = null;

    [Export]
    public Color Negative { get; set; } = new Color("red");
    // [Signal]
    // public delegate void StartConstructionEventHandler(Node building);

    void GetNodes(){
        _title = GetNode<Label>("TitleLabel");
        _desc = GetNode<RichTextLabel>("Description");
    }

    public override void _Ready()
    {
        GetNodes();
    }

    public void UpdateInterface(Technology technology){
        // _building = null;
        // _unit = null;
        // _technology = null;
        // _listPanel.ClearItems();
        // if(technology != null){
        //     _technology = technology;
        //     _header.SetTitle(technology.Name);
        //     var label = new Label();
        //     label.Text = "\nBuild Cost\n";
        //     _listPanel.AddListItem(label);
        //     foreach(var resName in technology.BuildCost.Keys){
        //         label = new Label();
        //         label.Text = resName + " " + technology.BuildCost[resName];
        //         _listPanel.AddListItem(label);
        //     }
        //     label = new Label();
        //     label.Text = "\nConstruction time: " + technology.BuildTime;
        //     _listPanel.AddListItem(label);
        // }
    }

    public void UpdateInterface(Building building, bool tooExpensive){
        if(building != null){
            _title.Text = building.Name;
            _desc.Text = "";
            BuildCost(building);
            TooExpensive(tooExpensive);
            ConstructionTime(building.BuildTime);

            if(building.ExportProducts.Count > 0){
                _desc.AppendText("\n[b][font_size=20]Production[/font_size][/b]\n");
                foreach(var resourceName in building.ExportProducts.Keys){
                    _desc.AppendText(resourceName + " " + building.ExportProducts[resourceName] + "\n");
                }
            }
            if(building.ExportProductCost.Count > 0){
                _desc.AppendText("\n[b][font_size=20]Production cost[/font_size][/b]\n");
                foreach(var resName in building.ExportProductCost.Keys){
                    _desc.AppendText(resName + " " + building.ExportProductCost[resName] + "\n");
                }
            }
            if(building.ExportResourceLimits.Count > 0){
                _desc.AppendText("\n[b]Storage capacity: [/b]");
                foreach(var resName in building.ExportResourceLimits.Keys){
                    _desc.AppendText( resName + " "+ building.ExportResourceLimits[resName]+"\n");
                }
            }
            
        }
    }

    public void UpdateInterface(Unit unit, bool tooExpensive){
        if (unit != null)
        {
            _title.Text = unit.UnitName;
            _desc.Text = "";
            BuildCost(unit);
            TooExpensive(tooExpensive);
            ConstructionTime(unit.BuildTime);

            _desc.AppendText("\n[b][font_size=20]Stats: [/font_size][/b]\n");
            foreach (var stat in unit.StatsList)
            {
                _desc.AppendText(stat.Name + " " + stat.CurrentValue + "\n");
            }
            _desc.AppendText(unit.StatManager.PrintStats());
        }
    }

    public void TooExpensive(bool TooExpensive){
        if(TooExpensive) _desc.AppendText("[b][color="+Negative.ToHtml()+"][font_size=18]Not enough resources[/font_size][/color][/b]\n\n");
    }

    public void ConstructionTime(int time){
        _desc.AppendText("\n[b]Construction time: [/b]" + time + "\n");
    }

    public void BuildCost(Construct construct){
            _desc.AppendText("\n[b][font_size=20]Build Cost[/font_size][/b]\n\n");
            foreach(var resName in construct.ExportBuildCost.Keys){
                _desc.Text = resName + " - " + construct.ExportBuildCost[resName] + "\n";
            }
    }


    public void UpdateInterface(){
    }

    // void _on_gui_input(InputEvent input, Node node){
    //     if(input is InputEventMouseButton button && node is Unit unit){
    //         if(button.ButtonIndex == MouseButton.Left){
    //             _unit = unit;
    //         }
    //     }
    // }

    // void _on_BuildingInterface_gui_input(InputEvent e){
    //     if(e is InputEventScreenDrag drag)
    //         Position += drag.Relative;
    // }

    // public void ConnecToStartConstruction(Node node, string methodName){
    //     //var er = Connect(nameof(StartConstructionEventHandler), new Callable(node, methodName));
    //     var er = Connect("StartConstruction", new Callable(node, methodName));
    //     GD.Print(er.ToString());
    // }

    // void _on_ButtonUp(){
    //     Visible = false;
    //     _listPanel.ClearItems();
    // }

    // void _on_Build_button_up(){
    //     if(_building != null){
    //         EmitSignal(nameof(SignalName.StartConstruction), _building);
    //     }
    //     if(_unit != null){
    //         EmitSignal(nameof(SignalName.StartConstruction), _unit);
    //     }
    // }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
