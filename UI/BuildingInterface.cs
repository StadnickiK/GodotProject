using Godot;
using System;

public class BuildingInterface : Panel
{
    
    Button _acceptButton = null;
    Header _header = null;

    Building _building = null;

    Unit _unit = null;

    Ship _ship = null; 

    ListPanel _listPanel = null;

    [Signal]
    public delegate void StartConstruction(Node building);

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

    public void UpdateInterface(Building building, Planet planet){
        _building = null;
        _unit = null;
        _listPanel.ClearItems();
        if(building != null){
            _building = building;
            _header.SetTitle(building.Name);
            var label = new Label();
            label.Text = "\nBuild Cost\n";
            _listPanel.AddListItem(label);
            foreach(var resName in building.BuildCost.Keys){
                label = new Label();
                label.Text = resName + " " + building.BuildCost[resName];
                _listPanel.AddListItem(label);
            }
            label = new Label();
            label.Text = "\nConstruction time: "+building.BuildTime;
            _listPanel.AddListItem(label);
            label = new Label();
            label.Text = "\nProduction\n";
            _listPanel.AddListItem(label);
            foreach(var resourceName in building.Products.Keys){
                label = new Label();
                label.Text = resourceName + " " + building.Products[resourceName];
                _listPanel.AddListItem(label);
            }
            label = new Label();
            label.Text = "\nProduction cost\n";
            _listPanel.AddListItem(label);
            foreach(string resName in building.ProductCost.Keys){
                label = new Label();
                label.Text = resName + " " + building.ProductCost[resName];
                _listPanel.AddListItem(label);
            }
            foreach(var resName in building.ResourceLimits.Keys){
                label = new Label();
                label.Text = "\nStorage capacity: "+building.ResourceLimits[resName]+"\n";
                _listPanel.AddListItem(label);
            }
        }
    }

    public void UpdateInterface(Unit unit, Planet planet){
        _building = null;
        _unit = null;
        _listPanel.ClearItems();
        if(unit != null){
            _unit = unit;
            _header.SetTitle(unit.Name);
            var label = new Label();
            label.Text = "\nBuild Cost\n";
            _listPanel.AddListItem(label);
            foreach(var resName in unit.BuildCost.Keys){
                label = new Label();
                label.Text = resName + " " + unit.BuildCost[resName];
                _listPanel.AddListItem(label);
            }
            label = new Label();
            label.Text = "\nConstruction time: "+unit.BuildTime;
            _listPanel.AddListItem(label);
            label = new Label();
            label.Text = "\nStatistics:\n";
            _listPanel.AddListItem(label);
            foreach(var node in unit.Stats.GetChildren()){
                if(node is BaseStat stat){
                    label = new Label();
                    label.Text = stat.Name + ": " + stat.BaseValue;
                    _listPanel.AddListItem(label);
                }
            }
        }
    }


    public void UpdateInterface(){
        _listPanel.ClearItems();
        for(int i = 0; i < 5; i++){
            Unit unit = new Unit(5+i*8,2+i*2);
            unit.Name = "Unit "+i;
            Resource resource = new Resource();
            resource.Name = "resource "+i;
            resource.Quantity = 20 + i * 1;
            unit.BuildCost.Add(resource.Name, resource.Quantity);
            var label = new Label(); 
            label.Text = unit.Name +" " + unit.Stats.GetNode<BaseStat>("HitPoints").CurrentValue +" "+ unit.Stats.GetNode<BaseStat>("Attack").CurrentValue + " " + unit.Stats.GetNode<BaseStat>("Defence").CurrentValue 
            +"\n Cost: "+ resource.Name +": "+resource.Quantity;
            _listPanel.AddListItem(label, this, nameof(_on_gui_input), unit);
        }
    }

    void _on_gui_input(InputEvent input, Node node){
        if(input is InputEventMouseButton button && node is Unit unit){
            if(button.ButtonIndex == (int)ButtonList.Left){
                _unit = unit;
            }
        }
    }

    void _on_BuildingInterface_gui_input(InputEvent e){
        if(e is InputEventScreenDrag drag)
            RectPosition += drag.Relative;
    }

    public void ConnecToStartConstruction(Node node, string methodName){
        Connect(nameof(StartConstruction), node, methodName);
    }

    void _on_ButtonUp(){
        Visible = false;
        _listPanel.ClearItems();
    }

    void _on_Build_button_up(){
        if(_building != null){
            EmitSignal(nameof(StartConstruction), _building);
        }
        if(_unit != null){
            EmitSignal(nameof(StartConstruction), _unit);
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
