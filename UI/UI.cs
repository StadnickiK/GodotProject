using Godot;
using System;

public partial class UI : Control
{


    public ResourcePanel ResPanel { get; set; } = null;

    public PlanetInterface PInterface { get; set; } = null;

    public BuildingInterface BuildingInterface { get; set; }

    public RightPanel RPanel { get; set; } = null;

    public ArmyInterface ArmyInterfce { get; set; } = null;

    public SmallList OrbitList { get; set; } = null;

    public CmdPanel CommandPanel { get; set; } = null;

    public TopLeftPanel TopLeft { get; set; } = null;

    public RichTextLabel ResLabelTooltip { get; set; }

    public UnitCardFactory UnitCardFactory { get; set; }

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
        ResPanel = GetNode<ResourcePanel>("ResourcePanel");
        PInterface = GetNode<PlanetInterface>("PlanetInterface");
        RPanel = GetNode<RightPanel>("RightPanel");
        _menu = GetNode<Control>("Menu");
        _battlePanel = GetNode<BattlePanel>("BattlePanel");
        ArmyInterfce = GetNode<ArmyInterface>("ArmyInterface");
        ABox = GetNode<AlertBox>("AlertBox");
        OrbitList = GetNode<SmallList>("OrbitList");
        CommandPanel = GetNode<CmdPanel>("CmdPanel");
        TopLeft = GetNode<TopLeftPanel>("TopLeftPanel");
        BuildingInterface = GetNode<BuildingInterface>("BuildingInterface");
        UnitCardFactory = GetNode<UnitCardFactory>("UnitCardFactory");
    }

    public void UpdateResPanel(Player player)
    {
        ResPanel.UpdatePanel(player);
    }

    public void UpdateUI(Player player)
    {
        RPanel.UpdateRightPanel(player);
        // GD.Print("update r panel");

        if (player.ResourcesChanged)
        {
            if (player.ResManager != null)
            {
                UpdateResPanel(player);
                player.ResourcesChanged = false;
            }
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
        ArmyInterfce.ArmyPanel.UI = this;
        UnitCardFactory.UI = this;
        BattlePan.ConnectContainers(this);
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
        var pos = GetGlobalMousePosition();
		var x = pos.X-(BuildingInterface.Size.X/2);
		var y = pos.Y-BuildingInterface.Size.Y+20;
        BuildingInterface.Position = new Vector2(x, y);
		BuildingInterface.UpdateInterface(unit, tooExpensive);
	}

	public void _mouseLeftBuildingLabel(){
		BuildingInterface.Visible = false;
	}	

    //public void ConnectToLookAtObject(Node node, string methodName){
    // to do if RPanel goes private
    //}
}
