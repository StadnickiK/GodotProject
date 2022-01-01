using Godot;
using System;

public class TopLeftPanel : Control
{

    TechnologyInterface TechInterface { get; set; }

    public Player _Player { get; set; } = null;

    OverviewPanel overview;

    public Node WorldTechnology { get; set; } = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TechInterface = GetNode<TechnologyInterface>("TechnologyPanel");
        overview = GetNode<OverviewPanel>("Technologies");
        if(TechInterface != null){
            TechInterface.Connect(nameof(TechnologyInterface.StartResearch), this, nameof(_on_StartResearch_button_up));
        }
    }

    void _on_Technology_button_up(){
        overview.Visible = true;
        UpdateTechnology();
    }

    void _on_StartResearch_button_up(Technology technology){
        _Player.Research.ConstructBuilding(technology);
    }

    void UpdateTechnology(){
        foreach(Node node in WorldTechnology.GetChildren()){
            if(node is Technology technology){
                if(!_Player.Technologies.Contains(technology)){
                    overview.AddNodeToPanel("Research", CreateTechButton(technology));
                }
            }
        }
    }

    Button CreateTechButton(Technology technology){
        var button = new Button();
        button.Text = technology.Name;
        button.Name = technology.Name;
        Godot.Collections.Array array = new Godot.Collections.Array();
        array.Add(technology);
        button.Connect("button_up",this, nameof(_on_Button_Up), array);
        return button;
    }

    void _on_Button_Up(Technology technology){
        TechInterface.Visible = true;
        TechInterface.UpdateInterface(technology);
    }
}