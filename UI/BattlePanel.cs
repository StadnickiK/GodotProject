using Godot;
using System;
using System.Collections.Generic;

public class BattlePanel : Panel
{
    
    Button _closeButton = null;

    Header _header = null;

    OverviewPanel _overviewPanel = null;

    [Export]
    public string Title { get; set; } = "Space battle";
    
    [Export]
    public List<string> Options { get; set; } = new List<string>();

        void GetNodes(){
        _closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
        _header = GetNode<Header>("VBoxContainer/Header");
        _overviewPanel = GetNode<OverviewPanel>("VBoxContainer/OverviewPanel");
    }

    public override void _Ready()
    {
        GetNodes();
        _closeButton.Connect("button_up", this, nameof(_on_XButton_button_up));
        InitOverviewPanel();
    }

    void InitOverviewPanel(){
        foreach(string s in Options){
            _overviewPanel.AddPanel(s);
        }
    }

    void ClearInterface(){
        foreach(string s in Options){
            _overviewPanel.ClearPanel(s);
        }
    }

    public void UpdatePanel(SpaceBattle battle){
        ClearInterface();
        var progressBar = new ProgressBar();
        progressBar.Value = 69;
        progressBar.PercentVisible = true;
        _overviewPanel.AddNodeToPanel("Overview", progressBar);
        for(int i =0; i<battle.Comabatants.Count; i +=2){
            var label = new Label();
            label.Text = battle.Comabatants[i].Name + " vs " + battle.Comabatants[i+1].Name;
            _overviewPanel.AddNodeToPanel("Overview", label);
        }
    }

    void _on_XButton_button_up(){
        Visible = false;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
