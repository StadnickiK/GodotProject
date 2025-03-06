using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ResourcePanel : Panel
{
    
    Control _hBox = null;

    PackedScene ResourceLabelScene = null;

    bool Initialized = false;

    [Export]
    public string ScenePath { get; set; } = "res://UI/ResourceLabel.tscn";

    Dictionary<int, ResourceLabel> resLabels = new Dictionary<int, ResourceLabel>();

    void GetNodes(){
        _hBox = GetNode<Control>("ScrollContainer/HBox");
    }

    public override void _Ready()
    {
        GetNodes();
        ResourceLabelScene = (PackedScene)ResourceLoader.Load(ScenePath);
    }

    public void UpdatePanel(Dictionary<int, int> Resources){
        if(Initialized){
            foreach(var resName in Resources.Keys){
                if(resLabels.ContainsKey(resName)){
                    resLabels[resName].SetValue(Resources[resName]);
                }else{
                    CreateResourceLabel(resName, Resources[resName]);
                }
            }
            foreach(Node node in _hBox.GetChildren()){ // horizontalBox
                if(node is ResourceLabel label){
                    // if(Resources.ContainsKey(label.ResourceName.Text)){
                    //     //label.SetValue(Resources[label.ResourceName.Text].Quantity);
                    //     label.SetValue(Resources[label.ResourceName.Text]);
                    // }else{
                    //     //GD.Print("Update resource panel "+label.ResourceName.Text);
                    // }
                }
            }
        }else{
            foreach(KeyValuePair<int, int> resource in Resources){
                CreateResourceLabel(resource.Key, resource.Value);
            }
            Initialized = true; 
        }
    }

    public void InitResourcePanel(Godot.Collections.Array<Node> nodes){
        foreach(var r in nodes){
            if(r is Resource resource){
                CreateResourceLabel(resource);
            }
        }
    }

    ResourceLabel CreateResourceLabel(int resName, int quantity = 0){
        if(resLabels.Keys.Contains(resName)) return null;
         var label = (ResourceLabel)ResourceLabelScene.Instantiate();
        resLabels.Add(resName, label);
        _hBox.AddChild(label);
        if(Theme != null){
            label.SetLabelTheme(Theme);
        }else{
            Theme = new Theme();
            var font = new FontFile();
            font.FixedSize = 20;
            font.Antialiasing = TextServer.FontAntialiasing.Lcd;
            Theme.DefaultFont = font;
            label.SetLabelTheme(Theme);
        }
        label.ResourceName.Text = resName.ToString();
        label.Value.Text = quantity.ToString();
        return label;
    }

    ResourceLabel CreateResourceLabel(Resource resource, int quantity = 0){
        if(resLabels.Keys.Contains(resource.Index)) return null;
        var label = CreateResourceLabel(resource.Index);
        if(resource.IconPlaceholder != null)
            label.ResourceName.Text = resource.IconPlaceholder.ToString();
        return label;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
