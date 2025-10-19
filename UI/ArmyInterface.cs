using Godot;
using System;
using Godot.Collections;
using System.Linq;
using System.Collections.Generic;

public partial class ArmyInterface : Control
{
	[Signal]
    public delegate void DeselectEventHandler();

	public delegate void TransferUnitsEventHandler(List<Unit> hostIDS, List<Unit> targetIDS);

	public event TransferUnitsEventHandler TransferUnits;

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/BuildingLabel.tscn";

	Button _closeButton = null;

	Button _buildButton = null;

	Header _header = null;

	IPlanetInterface _mapArmy = null;

	IEnterCombat transferArmy;

	public Data _data { get; set; }

	ArmyPanel _armyPanel = null;

	ArmyTransferPanel _armyTransferPanel;

	ArmyRecruitment _armyRecruitment= null;

	InfoPanel info;

	BuildMenu _buildMenu;

	public ArmyPanel ArmyPanel { get => _armyPanel; }

	//BuildingInterface _buildingInterface = null;

	[Export]
	public string Title
	{
		get { return _header.Title; }
		set { _header.Title = value; }
	}


	public int LocalPlayerID { get; set; }
		

    // BuildingLabel _selectedBuilding = null;

	[Signal]
	public delegate void SelectObjectInOrbitEventHandler(Planet planet, Node node);

	void GetNodes(){
		var ArmyView = GetNode<ArmyView>("VBoxContainer/ArmyView");
		_closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
		_buildButton = ArmyView.ArmyInterfaceContainer.BuildButton;
		_header = GetNode<Header>("VBoxContainer/Header");
		//_buildingInterface = GetNode<BuildingInterface>("UnitInterface");
		_buildMenu = GetNode<BuildMenu>("BuildMenuScroll");
		
		_armyPanel = ArmyView.ArmyInterfaceContainer.ArmyPanel;
		_armyTransferPanel = GetNode<ArmyTransferPanel>("ArmyTransferPanel");
		_armyRecruitment = ArmyView.ArmyInterfaceContainer.ArmyRecruitment;
		info = ArmyView.InfoPanel;
	}

	void UpdateInfo(IPlanetInterface ship, System.Collections.Generic.List<Resource> resources){
		info.Controller.Text = "Owned by " + ship.Controller.Name;
		var upkeep = "\tUpkeep:";
		foreach (var item in ship.UnitController.UpkeepComponent.Upkeep)
		{
			upkeep += " " + item.Value+" " + resources[item.Key].IconPlaceholder;
		}
		info.Upkeep.Text = upkeep;
	}

	void ConnectSignals(){
		_closeButton.Connect("button_up", new Callable(this, nameof(_on_XButton_button_up)));
		ArmyPanel.ConnectUnitCards(this);
		_armyTransferPanel.ConfrimButton.ButtonUp += _on_TransferArmy;
		//_buildings.ConnectBuildButtons(this);
		// _buildMenu.ConnectBuildButtons(this);
	}

	public void SetTitle(string title){
		_header.SetTitle(title);
	}

	public override void _Ready()
	{
		GetNodes();
		ConnectSignals();
	}

	public void UpdateArmyPanel(IPlanetInterface ship){
		_armyTransferPanel.Hide();
		if (ship != null)
		{
			var visibility = ship.VisibilityConroller.GetVisibility(LocalPlayerID);
			Visible = true;
			_mapArmy = ship;
			SetTitle(ship.Name);
			UpdateInfo(ship, _data.Resources);
			ArmyPanel.UpdateArmyList(ship.UnitController.UnitsList, ship.Controller.PlayerID == LocalPlayerID);
			_armyRecruitment.UpdateRecruitment(ship.RecruitmentComponent, ship.Controller.PlayerID == LocalPlayerID);
			ship.RecruitmentComponent.UpdateCurrentlyRecruitedUnitsEvent += UpdateRecruitment;
			ArmyPanel.UpdateUnitsToTransfer += ship.UnitController._on_UpdateUnitsToTransfer;
			UpdateBuildButton();
			_mapArmy.RecruitmentComponent.UpdateAvaialableUnitsEvent += UpdateBuildButton;
			_mapArmy.RecruitmentComponent.RemoveAvaialableUnitsEvent += UpdateBuildButton;
			// if(ship.Vision){

			// }
		}
	}

	public void UpdateArmyPanel(List<Unit> units)
	{
		UpdateArmyPanel();	
		ArmyPanel.UpdateArmyList(units, true);	
	}

	public void UpdateArmyPanel(List<Unit3D> units)
	{
		UpdateArmyPanel();
		ArmyPanel.UpdateArmyList(units, true);		
	}
	void UpdateArmyPanel()
	{
		_armyTransferPanel.Hide();
		Show();
		_header.Hide();
		_armyRecruitment.Hide();
		_buildButton.Hide();
	}

	public void UpdateArmyPanel(IPlanetInterface ship, IEnterCombat target)
	{
		UpdateArmyPanel(ship);
		transferArmy = target;
		_armyTransferPanel.UpdateTransferPanel(target.UnitController.UnitsList);
	}

	void UpdateRecruitment(RecruitmentComponent recruitmentComponent){
		_armyRecruitment.UpdateRecruitment(recruitmentComponent, _mapArmy.Controller.PlayerID == LocalPlayerID);
	}

	public void DeselectArmy()
	{
		if (_mapArmy != null)
		{
			_mapArmy.RecruitmentComponent.UpdateCurrentlyRecruitedUnitsEvent -= UpdateRecruitment;
			ArmyPanel.UpdateUnitsToTransfer -= _mapArmy.UnitController._on_UpdateUnitsToTransfer;
			_mapArmy.RecruitmentComponent.UpdateAvaialableUnitsEvent -= UpdateBuildButton;
			_mapArmy.RecruitmentComponent.RemoveAvaialableUnitsEvent -= UpdateBuildButton;
		}
		ArmyPanel.ResetPanel();
		Hide();
	}

	void _on_TransferArmy(){
		_mapArmy.UnitController.TransferUnits(transferArmy.UnitController, _armyTransferPanel.TransferArmy.UnitsToTransfer);
	}
	

	public void InitRecruitmentPanel(UI uI){
		_buildMenu.InitAllUnits(_data.GetData("Units"), this, uI);
		int pos = _armyRecruitment.unitCards.Count;
		foreach(UnitCard unit in _armyRecruitment.unitCards){
			unit.button.ButtonUp += () => _on_StopUnitConstruction(pos);
			pos--;
		}
	}

	void UpdateConstruction(Planet planet){
		// _overviewPanel.ClearPanel("Construction");
		if(planet.Controller != null){
			if(planet.Controller.PlayerID == LocalPlayerID){
				var tempLabel = new Label();
				tempLabel.Text = "\n Construction list: \n";
				// _overviewPanel.AddNodeToPanel("Construction", tempLabel);
			}
		}
	}

	public void _on_ConfirmTransferButtonUp(){
		TransferUnits?.Invoke(ArmyPanel.UnitsToTransfer, _armyTransferPanel.TransferArmy.UnitsToTransfer);
		_armyTransferPanel.Hide();
	}

	public void OpenTransferPanel(){
		_armyTransferPanel.Show();
	}

	public void _on_LabelGuiInputEvent(InputEvent input, Node node){
		if(input is InputEventMouseButton button){
			if(button.ButtonIndex == MouseButton.Left){
				//EmitSignal(nameof(SelectObjectInOrbit), _planet, node);
			}
		}
	}

	void _on_XButton_button_up(){
		Visible = false;
		//_buildingInterface.Visible = false;
		EmitSignal(nameof(SignalName.Deselect));
	}

	void _on_StartConstruction(Node node){
		if(node is IConstruct unit){
			// if(unit != null && _planet != null){
			// 	_planet.StartConstruction((IConstruct)((PackedScene)GD.Load(node.SceneFilePath)).Instantiate());
			// 	// _planet.ConstructUnit(unit);
			// }
		}
	}

	public void _on_StartUnitConstruction(Unit unit){
		_mapArmy.RecruitmentComponent.StartConstruction(_mapArmy.RecruitmentComponent.AvaialableUnitsMap.Keys.ElementAt(0), unit);

		
				//_planet.StartConstruction((IConstruct)((PackedScene)GD.Load(unit.SceneFilePath)).Instantiate());
				// _planet.ConstructUnit(unit);
	}

	public void _on_StopUnitConstruction(int Position){
		_mapArmy.RecruitmentComponent.StopConstruction(Position);
	}

	void _on_build_button_mouse_exited(){
		_buildMenu.Hide(); 
	}

	public void _on_build_button_mouse_entered(){
		var pos = GetGlobalMousePosition();
		var x = pos.X-(_buildMenu.Size.X/2);
		var y = pos.Y-_buildMenu.Size.Y+20;
		_buildMenu.Position = new Vector2(x, y);
		_buildMenu.Show();
		var rc = _mapArmy.RecruitmentComponent;
		if(rc != null){
			if(rc.AvaialableUnitsMap.Count > 0 ){
				rc.AvaialableUnitsMap.Keys.ElementAt(0).UpdateCanPay(_mapArmy.Controller.ResManager);
				_buildMenu.UpdateBuildMenu(rc.AvaialableUnitsMap.Keys.ElementAt(0));
			} 
		}
	}

	public void UpdateBuildButton()
	{
		var rc = _mapArmy.RecruitmentComponent;
		if(rc != null){
			_buildButton.Visible = rc.AvaialableUnitsMap.Count > 0;
		}
	}

	public override void _Process(double delta)
	{
		// if(Visible){
		// 	if(_planet != null && _data != null){
		// 		if(_planet.BuildingsManager.ConstructionListChanged){
		// 			UpdateBuildings(_planet);
		// 			_planet.BuildingsManager.ConstructionListChanged = false;
		// 		}
		// 		if(_planet.Constructions.HasConstruct){
		// 			UpdateConstruction(_planet);
		// 		}
		// 	}
		// }
	}
}
