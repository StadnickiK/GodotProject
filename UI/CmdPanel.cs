using Godot;
using System;

public class CmdPanel : Panel
{

    public Button MoveButton { get; set; } = null;

    public Button ConquerButton { get; set; } = null;

    public Button ScanButton { get; set; } = null;

    public Button BombardButton { get; set; } = null;

    [Signal]
    public delegate void ShipCommand(CmdPanelOption option, Planet planet);

    Planet _currentPlanet = null;

    public enum CmdPanelOption
    {
        None,
        MoveTo,
        Conquer,
        Scan,
        Bombard
    }

    public override void _Ready()
    {
        GetNodes();
    }

    void GetNodes(){
        MoveButton = GetNode<Button>("VBoxContainer/MoveButton");
        ConquerButton = GetNode<Button>("VBoxContainer/Conquer");
        ScanButton = GetNode<Button>("VBoxContainer/Scan");
        BombardButton = GetNode<Button>("VBoxContainer/Bombard");
    }

    public void ShowPanel(Planet planet){
        _currentPlanet = planet;
        Visible = true;
        var vec2 = GetGlobalMousePosition();
        SetPosition(new Vector2(vec2.x-4, vec2.y-4));
    }

    void _on_MoveButton_button_up(){
        EmitSignal("ShipCommand", CmdPanelOption.MoveTo, _currentPlanet);
    }

    void _on_Conquer_button_up(){
        EmitSignal("ShipCommand", CmdPanelOption.Conquer, _currentPlanet);
    }

    void _on_Scan_button_up(){
        EmitSignal("ShipCommand", CmdPanelOption.MoveTo, _currentPlanet);
    }

    void _on_Bombard_button_up(){
        EmitSignal("ShipCommand", CmdPanelOption.MoveTo, _currentPlanet);
    }
}
