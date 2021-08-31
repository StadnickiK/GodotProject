using Godot;
using System;

public class CmdPanel : Panel
{

    public Button MoveButton { get; set; } = null;

    public Button ConquerButton { get; set; } = null;

    public Button ScanButton { get; set; } = null;

    public Button BombardButton { get; set; } = null;


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

    public void ShowPanel(){
        Visible = true;
        var vec2 = GetGlobalMousePosition();
        SetPosition(new Vector2(vec2.x-4, vec2.y-4));
    }
}
