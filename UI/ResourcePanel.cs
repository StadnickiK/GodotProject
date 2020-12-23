using Godot;
using System;
using System.Collections.Generic;

public class ResourcePanel : Panel
{
    
    Control _hBox = null;

    PackedScene ResourceLabelScene = null;

    [Export]
    public string ScenePath { get; set; } = "res://UI/ResourcePanel.tscn";

    void GetNodes(){
        _hBox = GetNode<Control>("ScrollContainer/HBox");
    }

    [Export]
    public List<Resource> Resources { get; set; } = new List<Resource>();

    public override void _Ready()
    {
        GetNodes();
        if(ScenePath != null){
            ResourceLabelScene = (PackedScene)ResourceLoader.Load(ScenePath);
        }
        InitPanel();
    }

    void InitPanel(){
        foreach(Resource resource in Resources){
            var label = (ResourceLabel)ResourceLabelScene.Instance();
            label.Name = resource.Name;
            label.SetResourceName(resource.Name);
            label.SetValue(resource.Quantity);
            _hBox.AddChild(label);
        }
    }

    public void UpdatePanel(){
        for(int i = 0; i<Resources.Count; i++){
            var label = (ResourceLabel)_hBox.GetChild(i);
            label.Name = Resources[i].Name;
            label.SetResourceName(Resources[i].Name);
            label.SetValue(Resources[i].Quantity);
            _hBox.AddChild(label);
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
