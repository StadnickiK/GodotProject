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

    SpaceBattle _battle = null;

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
        _battle = battle;
        ClearInterface();
        UpdateOverview(battle);
        UpdateAttacker(battle);
        UpdateDefender(battle);
    }

    void UpdateOverview(SpaceBattle battle){
        var progressBar = new ProgressBar();
        progressBar.Value = (double)battle.Attacker.Power.CurrentValue/((double)battle.Attacker.Power.CurrentValue+(double)battle.Defender.Power.CurrentValue)*100;
        progressBar.PercentVisible = true;
        _overviewPanel.AddNodeToPanel("Overview", progressBar);
        for(int i =0; i<battle.Comabatants.Count; i +=2){
            var label = new Label();
            label.Text = battle.Comabatants[i].Name + " vs " + battle.Comabatants[i+1].Name;
            _overviewPanel.AddNodeToPanel("Overview", label);
        }
    }

    void UpdateDefender(SpaceBattle battle){
        var label = new Label(); 
        label.Text = "Name / HP / Attack / Defence \n";
        _overviewPanel.AddNodeToPanel("Defender", label);
        foreach(Unit unit in battle.Defender.Units){
            label = new Label(); 
            label.Text = unit.Name +" " + unit.Stats["HitPoints"].CurrentValue +" "+ unit.Stats["Attack"].CurrentValue + " " + unit.Stats["Defence"].CurrentValue;
            _overviewPanel.AddNodeToPanel("Defender", label);
        }
    }

    void UpdateAttacker(SpaceBattle battle){
        var label = new Label(); 
        label.Text = "Name / HP / Attack / Defence \n";
        _overviewPanel.AddNodeToPanel("Attacker", label);
        foreach(Unit unit in battle.Attacker.Units){
            label = new Label(); 
            label.Text = unit.Name +" " + unit.Stats["HitPoints"].CurrentValue +" "+ unit.Stats["Attack"].CurrentValue + " " + unit.Stats["Defence"].CurrentValue;
            _overviewPanel.AddNodeToPanel("Attacker", label);
        }
    }

    void _on_XButton_button_up(){
        Visible = false;
    }

    public override void _Process(float delta){
        if(Visible){
            if(_battle != null){
                if(_battle.PowerChanged)
                    UpdatePanel(_battle);
            }
        }
    }

}
