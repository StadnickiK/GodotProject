using Godot;
using System;

public class BuildingInterface : Panel
{
    
    Button _acceptButton = null;
    Header _header = null;

    Building _building = null;

    ListPanel _listPanel = null;

    [Signal]
    public delegate void StartConstruction(Building building);

    void GetNodes(){
        foreach(Node node in GetChildren()){
            GD.Print(node.Name);
        }
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
        _listPanel.ClearItems();
        if(building != null){
            _building = building;
            _header.SetTitle(building.Name);
            var label = new Label();
            label.Text = "\nBuild Cost\n";
            _listPanel.AddListItem(label);
            foreach(Resource resource in building.BuildCost){
                label = new Label();
                label.Text = resource.Name + " " + resource.Quantity;
                _listPanel.AddListItem(label);
            }
            label = new Label();
            label.Text = "\nConstruction time: "+building.BuildTime;
            _listPanel.AddListItem(label);
            label = new Label();
            label.Text = "\nProduction\n";
            _listPanel.AddListItem(label);
            foreach(Resource resource in building.Products){
                label = new Label();
                label.Text = resource.Name + " " + resource.Quantity;
                _listPanel.AddListItem(label);
            }
            label = new Label();
            label.Text = "\nProduction cost\n";
            _listPanel.AddListItem(label);
            foreach(Resource resource in building.ProductCost){
                label = new Label();
                label.Text = resource.Name + " " + resource.Quantity;
                _listPanel.AddListItem(label);
            }
        }else{
            //GD.Print("null");
        }
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
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
