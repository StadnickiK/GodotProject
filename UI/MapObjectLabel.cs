using Godot;
using System;

public partial class MapObjectLabel : Control
{   

    public Node3D MapObject { get; set; } = null;

    public Button Button { get; set; }

    [Export]
    public string Title { get => TitleLabel.Text; set => TitleLabel.Text = value; }

    public Label TitleLabel { get => _titleLabel; set => _titleLabel = value; }
    public Label LocationLabel { get => _locationLabel; set => _locationLabel = value; }
    public Label DestinationLabel { get => _destinationLabel; set => _destinationLabel = value; }

    public string Destination { get => DestinationLabel.Text; set => DestinationLabel.Text = value; }

    public string Location { get => LocationLabel.Text; set => LocationLabel.Text = value; }

    Label _titleLabel = null;

    Label _locationLabel = null;

    Label _destinationLabel = null;

    void GetNodes(){
        TitleLabel = GetNode<Label>("VBoxContainer/Title");
        Button = GetNode<Button>("Button");
        LocationLabel = GetNode<Label>("VBoxContainer/HBoxContainer/Location");
        DestinationLabel = GetNode<Label>("VBoxContainer/HBoxContainer/Destination");
    }

    public void UpdateLabel(){
        TitleLabel.Text = Title;
        LocationLabel.Text = Location;
        DestinationLabel.Text = Destination;
    }

    public void UpdateLabel(Node3D node, string Title, string Location, string Destination = ""){
        TitleLabel.Text = Title;
        LocationLabel.Text = Location;
        DestinationLabel.Text = Destination;
        MapObject = node;
    }

    public void UpdateLabel(MapObjectLabel label){
        Title = label.Title;
        Name = label.Name;
        LocationLabel.Text = label.Location;
        DestinationLabel.Text = label.Destination;
        MapObject = label.MapObject;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
        UpdateLabel();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
