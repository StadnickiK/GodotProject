using Godot;
using System;

public class UI : Spatial
{


    public ResourcePanel ResPanel { get; set; } = null;

    public PlanetInterface PInterface { get; set; } = null;
    
    public RightPanel RPanel { get; set; } = null;
 
    public UnitInfoPanel UInfo { get; set; } = null;

    private Control _menu = null;
    public Control WorldMenu
    {
        get { return _menu; }
    }

    private BattlePanel _battlePanel;
    public BattlePanel BattlePan
    {
        get { return _battlePanel; }
    }
    
    public AlertBox ABox { get; set; } = null;

    void GetNodes(){
        ResPanel = GetNode<ResourcePanel>("ResourcePanel");
        PInterface = GetNode<PlanetInterface>("PlanetInterface");
        RPanel = GetNode<RightPanel>("RightPanel");
        _menu = GetNode<Control>("Menu");
        _battlePanel = GetNode<BattlePanel>("BattlePanel");
        UInfo = GetNode<UnitInfoPanel>("UnitInfoPanel");
        ABox = GetNode<AlertBox>("AlertBox");
    }

    public void UpdateUI(Player player){
        if(player.MapObjectsChanged){
            RPanel.UpdateRightPanel(player);
            player.MapObjectsChanged = false;
            GD.Print("update r panel");
        }
        if(player.ResourcesChanged){
            if(player.ResManager != null){
                ResPanel.UpdatePanel(player.ResManager.Resources);
                player.ResourcesChanged = false;
            }
        }
    }

    public override void _Ready()
    {
        GetNodes();
    }

    //public void ConnectToLookAtObject(Node node, string methodName){
        // to do if RPanel goes private
    //}
}
