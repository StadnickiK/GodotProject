using Godot;
using System;

public partial class ScenePaths
{
    private ScenePaths() { }
    private static ScenePaths _instance;
    public static ScenePaths Instance
    {
        get
        {
            if (_instance == null) _instance = new ScenePaths();
            return _instance;
        }
        private set { _instance = value; }
    }

    public string ManualBattleScene { get; set; } = "res://Map/Battle/manual_battle_scene.tscn";

    public string WorldScene { get; set; } = "res://World/World.tscn";

    public string MainMenuScene { get; set; } = "res://Menu/MainMenu.tscn";

    public string CampaignScene { get; set; } = "res://World/campaign_scene.tscn";
}
