using Godot;
using System;
using System.Collections.Generic;

public class ResourcePanel : Panel
{
    
    Control _hBox = null;

    PackedScene ResourceLabelScene = null;

    bool Initialized = false;

    [Export]
    public string ScenePath { get; set; } = "res://UI/ResourceLabel.tscn";

    void GetNodes(){
        _hBox = GetNode<Control>("ScrollContainer/HBox");
    }

    public override void _Ready()
    {
        GetNodes();
        ResourceLabelScene = (PackedScene)ResourceLoader.Load(ScenePath);
    }

    public void UpdatePanel(Dictionary<string, int> Resources){
        if(Initialized){
            foreach(Node node in _hBox.GetChildren()){
                if(node is ResourceLabel label){
                    if(Resources.ContainsKey(label.ResourceName.Text)){
                        //label.SetValue(Resources[label.ResourceName.Text].Quantity);
                        label.SetValue(Resources[label.ResourceName.Text]);
                    }else{
                        //GD.Print("Update resource panel "+label.ResourceName.Text);
                    }
                }
            }
        }else{
            foreach(KeyValuePair<string, int> resource in Resources){
                var label = (ResourceLabel)ResourceLabelScene.Instance();
                _hBox.AddChild(label);
                if(Theme != null){
                    label.SetLabelTheme(Theme);
                }else{
                    Theme = new Theme();
                    var font = new DynamicFont();
                    font.Size = 20;
                    font.UseFilter = true;
                    Theme.DefaultFont = font;
                    label.SetLabelTheme(Theme);
                }
                label.ResourceName.Text = resource.Key;
                //label.Value.Text = resource.Value.Quantity.ToString();
                label.Value.Text = resource.Value.ToString();
                Initialized = true; 
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
