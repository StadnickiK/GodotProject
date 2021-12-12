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

    Dictionary<string, ResourceLabel> resLabels = new Dictionary<string, ResourceLabel>();

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
            foreach(string resName in Resources.Keys){
                if(resLabels.ContainsKey(resName)){
                    resLabels[resName].SetValue(Resources[resName]);
                }else{
                    CreateResourceLabel(resName, Resources[resName]);
                }
            }
            foreach(Node node in _hBox.GetChildren()){ // horizontalBox
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
                CreateResourceLabel(resource.Key, resource.Value);
            }
            Initialized = true; 
        }
    }

    ResourceLabel CreateResourceLabel(string resName, int quantity = 0){
         var label = (ResourceLabel)ResourceLabelScene.Instance();
        resLabels.Add(resName, label);
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
        label.ResourceName.Text = resName;
        label.Value.Text = quantity.ToString();
        return label;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
