using Godot;
using System;

public partial class BuildingLabel : Control
{

    public ProgressBar Progress { get; set; }

    public Label ProgressLabel { get; set; }

    public Button BButton { get; set; }

    
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
        Progress.Visible = true;
        Progress.MaxValue = building.BuildTime;
        Progress.Value = building.CurrentTime;
        ProgressLabel.Text = building.CurrentTime +"/"+ building.BuildTime;
    }

    public void UpdateProgress(IBuilding building){
        if (building is Building b)
            RefBuilding = b;
        SelfModulate = new Color(1, 1, 1, 0.5f);
        BButton.Text = building.Name;
        Show();
        Progress.Visible = true;
        Progress.MaxValue = building.BuildTime;
        Progress.Value = building.CurrentTime;
        ProgressLabel.Text = building.CurrentTime +"/"+ building.BuildTime;
    }

    public void UpdateBuilding(Building building){
        Show();
        Progress.Visible = false;
        BButton.Text = building.Name;
        SelfModulate = new Color(1, 1, 1, 1);
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
            BButton.Visible = true;
            Progress.Visible = false;
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
