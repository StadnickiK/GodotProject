using Godot;
using System;

public partial class CenterBattlePanel : Panel
{

    public Button Fight { get; set; }

    public Button AutoFight { get; set; }

    public Button Retreat { get; set; }

    public Button EndFight { get; set; }

    public Button RepeatFight { get; set; }

    public Label BattleName { get; set; }

    public ProgressBar BattleOdds { get; set; }

    public Label Attacker { get; set; }

    public Label Defender { get; set; }

    Control PreButtons;

    Control PostButtons;

    public override void _Ready()
    {
        GetNodes();

    }

    void GetNodes()
    {
        Fight = GetNode<Button>("VBoxContainer/PreButtons/Fight");
        AutoFight = GetNode<Button>("VBoxContainer/PreButtons/Auto");
        Retreat = GetNode<Button>("VBoxContainer/PreButtons/Retreat");
        EndFight = GetNode<Button>("VBoxContainer/PostButtons/EndFight");
        RepeatFight = GetNode<Button>("VBoxContainer/PostButtons/RepeatFight");
        PreButtons = GetNode<Control>("VBoxContainer/PreButtons");
        PostButtons = GetNode<Control>("VBoxContainer/PostButtons");
        Attacker = GetNode<Label>("VBoxContainer/Labels/Attacker");
        Defender = GetNode<Label>("VBoxContainer/Labels/Defender");
        BattleOdds = GetNode<ProgressBar>("VBoxContainer/BattleOdds");
        Defender = GetNode<Label>("VBoxContainer/BattleName");
    }

    void ShowPreButtons()
    {
        PreButtons.Visible = true;
        PostButtons.Visible = false;
    }

    void ShowPostButtons()
    {
        PreButtons.Visible = false;
        PostButtons.Visible = true;
    }

    public void Update(Ship attacker, Ship defender, int odds = 50)
    {
        ShowPreButtons();
        BattleOdds.Value = odds;
        Attacker.Text = attacker.Name;
        Defender.Text = defender.Name;
    }

    public void UpdatePostBattle(Ship attacker, Ship defender, int odds = 50)
    {
        ShowPostButtons();
        BattleOdds.Value = odds;
    }
}
