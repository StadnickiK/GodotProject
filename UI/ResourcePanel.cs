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

    [Export]
    public Color Positive { get; set; } = new Color("green");

    [Export]
    public Color Negative { get; set; } = new Color("red");

    Dictionary<int, ResourceLabel> resLabels = new Dictionary<int, ResourceLabel>();

    void GetNodes(){
        _hBox = GetNode<Control>("ScrollContainer/HBox");
    }

    public override void _Ready()
    {
        GetNodes();
        ResourceLabelScene = (PackedScene)ResourceLoader.Load(ScenePath);
    }

    public void UpdatePanel(ResourceManager resourceManager, UpkeepComponent upkeepComponent){
            foreach(var res in resourceManager.Resources){
                if(resLabels.ContainsKey(res.Key)){
                    resLabels[res.Key].Update(res.Value, resourceManager.Production.Upkeep[res.Key]);
                    resLabels[res.Key].TooltipText = "[b]"+ resLabels[res.Key].ResourceName + "[/b]\n\n";
                    resLabels[res.Key].TooltipText += "[color="+Positive.ToHtml()+"][b]Production: [/b]" + resourceManager.Production.Upkeep[res.Key] +"[/color]\n\n";

                    resLabels[res.Key].TooltipText += "[color="+Negative.ToHtml()+"][b]Production cost: [/b]" + resourceManager.ProdCost.Upkeep[res.Key] +"[/color]\n";
                    resLabels[res.Key].TooltipText += "[color="+Negative.ToHtml()+"][b]Upkeep: [/b]" + upkeepComponent.Upkeep[res.Key] +"[/color]\n\n";
                    
                    var color = resourceManager.Production.Upkeep[res.Key] > 0 ? Positive.ToHtml() : Negative.ToHtml();
                    resLabels[res.Key].TooltipText += "[color="+color+"][b]Total: [/b]" + resourceManager.Production.Upkeep[res.Key] +"[/color]\n\n";
                }else{
                    //CreateResourceLabel(resName, Resources[resName]);
                }
            }
    }

    public void InitResourcePanel(List<Resource> resources){
        foreach(var r in resources){
            CreateResourceLabel(r);  
        }
    }

    ResourceLabel CreateResourceLabel(Resource resource, int quantity = 0){
        if(resLabels.Keys.Contains(resource.Index)) return null;
        var label = CreateResourceLabel(resource.Index);
        if(resource.IconPlaceholder != null)
            label.ResourceName.Text = resource.IconPlaceholder.ToString();
        return label;
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

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
