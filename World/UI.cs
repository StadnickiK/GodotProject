using Godot;
using System;

public class UI : Node
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
    
    

    void GetNodes(){
        ResPanel = GetNode<ResourcePanel>("ResourcePanel");
        PInterface = GetNode<PlanetInterface>("PlanetInterface");
        RPanel = GetNode<RightPanel>("RightPanel");
        _menu = GetNode<Control>("Menu");
        _battlePanel = GetNode<BattlePanel>("BattlePanel");
        UInfo = GetNode<UnitInfoPanel>("UnitInfoPanel");
    }

    public void UpdateUI(Player player){
        if(player.MapObjectsChanged){
            RPanel.UpdateRightPanel(player);
            player.MapObjectsChanged = false;
        }
        if(player.ResourcesChanged){
            if(player.Resources != null){
                ResPanel.UpdatePanel(player.Resources);
                //player.ResourcesChanged = false;
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
