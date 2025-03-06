using Godot;
using System;

public partial class BuildingInterface : Panel
{
    Label _title = null;

    RichTextLabel _desc = null;

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

    public void UpdateInterface(Building building){
        if(building != null){
            _title.Text = building.Name;
            _desc.Text = "";
            _desc.AppendText("\n[b][font_size=20]Build Cost[/font_size][/b]\n\n");
            foreach(var resName in building.BuildCost.Keys){
                _desc.Text = resName + " - " + building.BuildCost[resName] + "\n";
            }
            _desc.AppendText("\n[b]Construction time: [/b]" + building.BuildTime + "\n");

            _desc.AppendText("\n[b][font_size=20]Production[/font_size][/b]\n");
            foreach(var resourceName in building.Products.Keys){
                _desc.AppendText(resourceName + " " + building.Products[resourceName] + "\n");
            }
            _desc.AppendText("\n[b][font_size=20]Production cost[/font_size][/b]\n");
            foreach(var resName in building.ProductCost.Keys){
                _desc.AppendText(resName + " " + building.ProductCost[resName] + "\n");
            }
            foreach(var resName in building.ResourceLimits.Keys){
                _desc.AppendText("\n[b]Storage capacity: [/b]" + building.ResourceLimits[resName]+"\n");
            }
        }
    }

    public void UpdateInterface(Unit unit){
        if(unit != null){
            _title.Text = unit.Name;
            _desc.Text = "";
            _desc.AppendText("\n[b][font_size=20]Build Cost[/font_size][/b]\n\n");
            foreach(var resName in unit.BuildCost.Keys){
                _desc.Text = resName + " - " + unit.BuildCost[resName] + "\n";
            }
            _desc.AppendText("\n[b]Construction time: [/b]" + unit.BuildTime + "\n");

            _desc.AppendText("\n[b][font_size=20]Stats: [/font_size][/b]\n");
            foreach(var stat in unit.StatsList){
                _desc.AppendText(stat.Name + " " + stat.CurrentValue + "\n");
            }
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
