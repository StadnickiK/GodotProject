using Godot;
using System;

public partial class BuildingLabel : Control
{

    public ProgressBar Progress { get; set; }

    public Label ProgressLabel { get; set; }

    public Button BButton { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    [Export]
    public bool ReversProgress { get; set; } = false;
    
    public Building RefBuilding { get; set; }

    public Unit RefUnit { get; set; }

    bool IsProgressing = false;
    

    void GetNodes(){
        BButton = GetNode<Button>("Button");
        Progress = GetNode<ProgressBar>("ProgressBar");
        ProgressLabel = GetNode<Label>("ProgressBar/Label");
    }
    public override void _Ready()
    {
        GetNodes();        
    }

    public void InitProgress(){
        Progress = GetNode<ProgressBar>("ProgressBar");
        Progress.Visible = true;
    }

    /// <summary>
    /// Initialize progress bar with Building
    /// </summary>
    /// <param name="building"></param> <summary>
    /// 
    /// </summary>
    /// <param name="building"></param>
    public void UpdateProgress(Building building){
        Show();
        RefBuilding = building;
        BButton.Text = building.Name;
        UpdateProgressBar(building);
    }

    public void UpdateProgress(IConstruct building){
        if (building is Building b)
            RefBuilding = b;
        BButton.Modulate = new Color(1, 1, 1, 0.5f);
        BButton.Text = building.ConstructName;
        Show();
        UpdateProgressBar(building);
    }

    void UpdateProgressBar(IConstruct building){
        Progress.Visible = true;
        Progress.MaxValue = building.BuildTime;
        if(ReversProgress){
            Progress.Value = building.BuildTime - building.CurrentTime; // reverse build time, so it shows time remaining
            ProgressLabel.Text = Progress.Value.ToString();
        }else{
            Progress.Value = building.CurrentTime;
            ProgressLabel.Text = Progress.Value +"/"+ building.BuildTime;
        }
    }

    public void UpdateBuilding(Building building){
        Show();
        Progress.Visible = false;
        BButton.Text = building.Name;
        BButton.Modulate = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// Update progress after initialization
    /// </summary>
    /// <param name="progress"></param> <summary>
    /// 
    /// </summary>
    /// <param name="progress"></param>
    public void UpdateProgress(int progress){
        Progress.Value = progress;
    }

    void _on_Button_button_up(){
        BButton.Visible = false;
    }

    void _on_BuildingLabel_mouse_entered(){
        BButton.Visible = true;
    }

    void _on_BuildingLabel_mouse_exited(){
        BButton.Visible = false;
    }

    void _on_Progress_value_changed(float value){
        if(value >= Progress.MaxValue){
            Progress.Visible = false;
            BButton.Visible = true;  
        }
        if(value < Progress.MaxValue){
            Progress.Visible = true;
            BButton.Visible = false;
        }
    }



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
