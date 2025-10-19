using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ResourcePanel : Control
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

    public void UpdatePanel(ResourceManager ResManager){
            foreach(var res in ResManager.Resources){
                if(resLabels.ContainsKey(res.Key)){
                    resLabels[res.Key].Update(res.Value, ResManager.TotalProduction.Upkeep[res.Key]);
                    resLabels[res.Key].TooltipText = "[font_size=24][b]"+ resLabels[res.Key].ResName + "[/b][/font_size]\n\n";
                    resLabels[res.Key].TooltipText += " \n";

                    resLabels[res.Key].TooltipText += "[color="+Positive.ToHtml()+"][b]Production: [/b]+" + ResManager.Production.Upkeep[res.Key] +"[/color]\n";
                    resLabels[res.Key].TooltipText += " \n";

                    resLabels[res.Key].TooltipText += "[color="+Negative.ToHtml()+"][b]Production cost: [/b]-" + ResManager.ProdCost.Upkeep[res.Key] +"[/color]\n";
                    resLabels[res.Key].TooltipText += "[color="+Negative.ToHtml()+"][b]Upkeep: [/b]-" + ResManager.Upkeep.Upkeep[res.Key] +"[/color]\n";
                    var total = ResManager.ProdCost.Upkeep[res.Key] + ResManager.Upkeep.Upkeep[res.Key];
                    resLabels[res.Key].TooltipText += "[color="+Negative.ToHtml()+"][b]Total spending: [/b]-" + total +"[/color]\n";
                    resLabels[res.Key].TooltipText += " \n";

                    var color = ResManager.TotalProduction.Upkeep[res.Key] > 0 ? Positive.ToHtml() : Negative.ToHtml();
                    var sign = ResManager.TotalProduction.Upkeep[res.Key] > 0 ? '+' : ' ';
                    resLabels[res.Key].TooltipText += "[color="+color+"][b]Total: [/b]"+sign + ResManager.TotalProduction.Upkeep[res.Key] +"[/color]\n\n";
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
        label.ResName = resource.ResourceName;
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
