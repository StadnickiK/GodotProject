using Godot;
using System;


public class TechnologyPanel : Panel
{
    OverviewPanel overview;
    Header _header;

    Technology _technology = null;

    public Player _Player { get; set; }

    TechnologyInterface TechInterface { get; set; }

    void GetNodes(){
        overview = GetNode<OverviewPanel>("OverviewPanel");
        _header = GetNode<Header>("Header");
        TechInterface = GetNode<TechnologyInterface>("TechnologyInterface");
    }
    public override void _Ready()
    {
        GetNodes();
        _header.ConnectToButtonUp(this, nameof(_on_Close_Button_Up));
        if(TechInterface != null){
            TechInterface.Connect(nameof(TechnologyInterface.StartResearch), this, nameof(TechnologyPanel._on_StartResearch_button_up));
        }
    }

    public void UpdatePanel(Node WorldTechnology){
        UpdatePosition();
        UpdateResearch(WorldTechnology);
        UpdateOwnedTechnology();
    }

    public void UpdateResearch(Node WorldTechnology){
        overview.ClearPanel("Research");
        foreach(Node node in WorldTechnology.GetChildren()){
            if(node is Technology technology){
                if(!_Player.Technologies.Contains(technology)){
                    overview.AddNodeToPanel("Research", CreateTechButton(technology));
                }
            }
        }
    }

    public void UpdateResearch(Node WorldTechnology, System.Collections.Generic.List<IBuilding> technologies){  
        overview.ClearPanel("Research");
        foreach(var technology in technologies){
            var label = new Label();
            label.Text = technology.Name;
            label.Name = technology.Name;
            overview.AddNodeToPanel("Research", label);
            var progress = new ProgressBar();
            progress.MaxValue = technology.BuildTime;
            progress.Value = technology.CurrentTime;
            overview.AddNodeToPanel("Research", progress);
        }
        foreach(Node node in WorldTechnology.GetChildren()){
            if(node is Technology tech && !technologies.Contains((IBuilding)node)){
                if(!_Player.Technologies.Contains(tech)){
                    overview.AddNodeToPanel("Research", CreateTechButton(tech));
                }
            }
        }
    }

    void UpdatePosition(){
        var size = new Vector2(GetViewport().Size);
        size /= 2;
        size.x -= (RectSize.x/2);
        size.y -= (RectSize.y/2);
        RectPosition = size;
    }

    void UpdateOwnedTechnology(){
        overview.ClearPanel("Owned");
        foreach(Technology technology in _Player.Technologies){
            overview.AddNodeToPanel("Owned", CreateTechButton(technology));
        }
    }

    Button CreateTechButton(Technology technology){
        var button = new Button();
        button.Text = technology.Name;
        button.Name = technology.Name;
        Godot.Collections.Array array = new Godot.Collections.Array();
        array.Add(technology);
        button.Connect("button_up",this, nameof(_on_Tech_Button_Up), array);
        return button;
    }

    void _on_Close_Button_Up(){
        Visible = false;
    }

    void _on_Tech_Button_Up(Technology technology){
        TechInterface.Visible = true;
        TechInterface.UpdateInterface(technology);
    }

    void _on_StartResearch_button_up(Technology technology){
        _Player.Research.ConstructBuilding(technology);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {

//  }
}   
