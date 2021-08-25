using Godot;
using System;

public class BuildingLabel : Control
{

    public ProgressBar Progress { get; set; }

    bool IsProgressing = false;
    
    public Button BButton { get; set; }

    void GetNodes(){
        BButton = GetNode<Button>("Button");
        Progress = GetNode<ProgressBar>("Progress");
    }
    public override void _Ready()
    {
        GetNodes();        
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
