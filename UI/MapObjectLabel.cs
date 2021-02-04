using Godot;
using System;

public class MapObjectLabel : VBoxContainer
{   

    public Node MapObject { get; set; } = null;

    [Export]
    public string Title { get; set; } = "Title";

    public string Location { get; set; } = "Location";

    public string Destination { get; set; } = "Destination";
    public Label TitleLabel { get => _titleLabel; set => _titleLabel = value; }
    public Label LocationLabel { get => _locationLabel; set => _locationLabel = value; }
    public Label DestinationLabel { get => _destinationLabel; set => _destinationLabel = value; }

    Label _titleLabel = null;

    Label _locationLabel = null;

    Label _destinationLabel = null;

    void GetNodes(){
        TitleLabel = GetNode<Label>("Title");
        LocationLabel = GetNode<Label>("HBoxContainer/Location");
        DestinationLabel = GetNode<Label>("HBoxContainer/Destination");
    }

    public void UpdateLabel(){
        TitleLabel.Text = Title;
        LocationLabel.Text = Location;
        DestinationLabel.Text = Destination;
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
