using Godot;
using System;

public class UI : Control
{


    public ResourcePanel ResPanel { get; set; } = null;

    public PlanetInterface PInterface { get; set; } = null;
    
    public RightPanel RPanel { get; set; } = null;
 
    public UnitInfoPanel UInfo { get; set; } = null;

    public SmallList OrbitList { get; set; } = null;

    public CmdPanel CommandPanel { get; set; } = null;

    public TopLeftPanel TopLeft { get; set; } = null;

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

    public UnitTransferPanel UnitTransferP { get; set; }

    void GetNodes(){
        ResPanel = GetNode<ResourcePanel>("ResourcePanel");
        PInterface = GetNode<PlanetInterface>("PlanetInterface");
        RPanel = GetNode<RightPanel>("RightPanel");
        _menu = GetNode<Control>("Menu");
        _battlePanel = GetNode<BattlePanel>("BattlePanel");
        UInfo = GetNode<UnitInfoPanel>("UnitInfoPanel");
        ABox = GetNode<AlertBox>("AlertBox");
        OrbitList = GetNode<SmallList>("OrbitList");
        CommandPanel = GetNode<CmdPanel>("CmdPanel");
        TopLeft = GetNode<TopLeftPanel>("TopLeftPanel");
        UnitTransferP = GetNode<UnitTransferPanel>("UnitTransferPanel");
    }

    public void UpdateUI(Player player){
        if(player.MapObjectsChanged){
            RPanel.UpdateRightPanel(player);
            player.MapObjectsChanged = false;
            // GD.Print("update r panel");
        }
        if(player.ResourcesChanged){
            if(player.ResManager != null){
                ResPanel.UpdatePanel(player.ResManager.Resources);
                player.ResourcesChanged = false;
            }
        }
        HideIfLostFocus(OrbitList);
        HideIfLostFocus(CommandPanel);
    }

    void HideIfLostFocus(Control control){
        if(control.Visible){
            var box = new Rect2(GetGlobalMousePosition(), new Vector2(1,1));
            if(!box.Intersects(control.GetRect()))
                control.Visible = false;
        }
    }

    public override void _Ready()
    {
        GetNodes();
    }

    void _on_OrbitIconFocus(Node Orbit){
        OrbitList.Visible = true;
        var vec2 = GetGlobalMousePosition();
        OrbitList.SetPosition(new Vector2(vec2.x-4, vec2.y-4));
        OrbitList.UpdateOrbitInfo(Orbit);
    }


    //public void ConnectToLookAtObject(Node node, string methodName){
        // to do if RPanel goes private
    //}
}
