using Godot;
using System;

public partial class UI : Control
{


    public ResourcePanel ResourcePanel { get; set; } = null;

    public PlanetInterface PInterface { get; set; } = null;

    public BuildingInterface BuildingInterface { get; set; }

    public RightPanel RightPanel { get; set; } = null;

    public SmallList OrbitList { get; set; } = null;

    public CmdPanel CommandPanel { get; set; } = null;

    public TopLeftPanel TopLeft { get; set; } = null;

    public RichTextLabel ResLabelTooltip { get; set; }

    public UnitCardFactory UnitCardFactory { get; set; }

    public EndTurnPanel EndTurnPanel { get; set; }

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

    void GetNodes()
    {
        ResourcePanel = GetNode<ResourcePanel>("ResourcePanel");
        PInterface = GetNode<PlanetInterface>("PlanetInterface");
        RightPanel = GetNode<RightPanel>("RightPanel");
        _menu = GetNode<Control>("Menu");
        _battlePanel = GetNode<BattlePanel>("BattlePanel");
        ABox = GetNode<AlertBox>("AlertBox");
        OrbitList = GetNode<SmallList>("OrbitList");
        CommandPanel = GetNode<CmdPanel>("CmdPanel");
        TopLeft = GetNode<TopLeftPanel>("TopLeftPanel");
        BuildingInterface = GetNode<BuildingInterface>("BuildingInterface");
        UnitCardFactory = GetNode<UnitCardFactory>("UnitCardFactory");
        EndTurnPanel = GetNode<EndTurnPanel>("EndTurnPanel");
    }

    public void UpdateResourcePanel(Player player)
    {
        ResourcePanel.UpdatePanel(player.ResManager);
    }

    public void UpdateUI(Player player)
    {
        RightPanel.UpdateRightPanel(player);
        // GD.Print("update r panel");
        if (player.ResManager != null)
        {
            UpdateResourcePanel(player);
        }        
        HideIfLostFocus(OrbitList);
        HideIfLostFocus(CommandPanel);
    }

    void HideIfLostFocus(Control control)
    {
        if (control.Visible)
        {
            var box = new Rect2(GetGlobalMousePosition(), new Vector2(1, 1));
            if (!box.Intersects(control.GetRect()))
                control.Visible = false;
        }
    }

    public override void _Ready()
    {
        GetNodes();
        UnitCardFactory.UI = this;
        BattlePan.ConnectContainers(this);
    }

    public void UpdateBattleUI()
    {
        TopLeft.Hide();
        RightPanel.Hide();
        BattlePan.Hide();
        ResourcePanel.Hide();
    }

    void _on_OrbitIconFocus(Node Orbit)
    {
        OrbitList.Visible = true;
        var vec2 = GetGlobalMousePosition();
        OrbitList.SetPosition(new Vector2(vec2.X - 4, vec2.Y - 4));
        OrbitList.UpdateOrbitInfo(Orbit);
    }

	public void _on_BuildingLabelGuiInputEvent(Unit unit, bool tooExpensive){
		BuildingInterface.Visible = true;
        MoveControlToMousePosition(BuildingInterface);
		BuildingInterface.UpdateInterface(unit, tooExpensive);
	}
    
    public void _on_BuildingLabelGuiInputEvent(Unit unit){
		BuildingInterface.Visible = true;
        MoveControlToMousePosition(BuildingInterface);
		BuildingInterface.UpdateInterface(unit.UnitName, unit);
	}

    void MoveControlToMousePosition(Control control)
    {
        var pos = GetGlobalMousePosition();
		var x = pos.X-(control.Size.X/2);
		var y = pos.Y-control.Size.Y-20;
        control.Position = new Vector2(x, y);
    }

	public void _mouseLeftBuildingLabel()
    {
        BuildingInterface.Visible = false;
    }	

    //public void ConnectToLookAtObject(Node node, string methodName){
    // to do if RightPanel goes private
    //}
}
