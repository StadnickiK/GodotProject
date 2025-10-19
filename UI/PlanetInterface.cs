using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Diagnostics.Contracts;
// using System.Collections.Generic;


public interface IPlanetInterface : IMapObjectController, IVisible, INode, ISelection
{
	public BuildingManager BuildingManager { get; set; }

	public RecruitmentManager RecruitmentManager { get; set; }

	public RecruitmentComponent RecruitmentComponent { get; set; }

	public UnitController UnitController { get; set; }

	public InputController InputController { get; set; }
}

public partial class PlanetInterface : Control
{

	Button _closeButton = null;

	Header _header = null;

	IPlanetInterface _planet = null;

	Godot.Collections.Array _allBuildings = null;

	Array<Unit> _allUnits = null;

	public Data _data { get; set; }

	bool Cleanup = false;

	BuildingPanel _buildingsPanel = null;

	BuildMenu _buildMenu;

	BuildMenu _unitMenu;

	TabsOrganizer tabsOrganizer;

	BuildingInterface _buildingInterface = null;

	ArmyView ArmyView;

	ArmyTransferPanel _armyTransferPanel;

	public bool IsLocalPlayer { get { return _planet.Controller.PlayerID == LocalPlayerID; } }

	[Export]
	public string Title { get; set; } = "Title";

	public int LocalPlayerID { get; set; }

	IEnterCombat _transferTarget;

	// BuildingLabel _selectedBuilding = null;

	[Signal]
	public delegate void SelectObjectInOrbitEventHandler(Planet planet, Node node);

	void GetNodes()
	{
		_header = GetNode<Header>("TabsOrganizer/Header");
		_closeButton = _header.HButton;
		_buildingInterface = GetNode<BuildingInterface>("BuildingInterface");
		_buildMenu = GetNode<BuildMenu>("BuildMenuScroll");
		_unitMenu = GetNode<BuildMenu>("UnitMenuScroll");
		_buildingsPanel = GetNode<BuildingPanel>("TabsOrganizer/Tabs/Buildings");
		tabsOrganizer = GetNode<TabsOrganizer>("TabsOrganizer");
		ArmyView = GetNode<ArmyView>("TabsOrganizer/Tabs/Orbit");
		_armyTransferPanel = GetNode<ArmyTransferPanel>("ArmyTransferPanel");
	}

	void ConnectSignals()
	{
		_closeButton.Connect("button_up", new Callable(this, nameof(_on_XButton_button_up)));
		_buildingsPanel.ConnectBuildButtons(this);
		// _buildMenu.ConnectBuildButtons(this);
		foreach (BuildingLabel buildingLabel in _buildMenu.buildingLabels)
		{
			buildingLabel.BButton.ButtonUp += () => _on_StartUnitConstruction(buildingLabel.RefBuilding);
		}

		foreach (BuildingLabel buildingLabel in _unitMenu.buildingLabels)
		{
			var duplicate = (Unit)buildingLabel.RefUnit.Duplicate();
			buildingLabel.BButton.ButtonUp += () => _on_StartUnitConstruction(duplicate);
		}
		ArmyView.ArmyInterfaceContainer.BuildButton.MouseEntered += _on_recruit_button_mouse_entered;
		_armyTransferPanel.ConfrimButton.ButtonUp += _on_TransferArmy;
	}

	public void SetTitle(string title)
	{
		_header.SetTitle(title);
	}

	public override void _Ready()
	{
		GetNodes();
	}

	public void InitBuildingsPanel()
	{
		//_buildingsPanel = GetNode<BuildingPanel>("TabsOrganizer/Tabs/Buildings");
		_buildMenu.InitAllBuildings(_data.Buildings, this);
		ConnectSignals();
		_buildingsPanel.InitAllBuildings(_data.Buildings, this);
	}

	public void InitRecruitmentPanel()
	{
		_unitMenu.InitAllUnits(_data.GetData("Units"), this);
		int pos = ArmyView.ArmyInterfaceContainer.ArmyRecruitment.unitCards.Count;
		foreach (UnitCard unit in ArmyView.ArmyInterfaceContainer.ArmyRecruitment.unitCards)
		{
			unit.button.ButtonUp += () => _on_StopUnitConstruction(pos);
			pos--;
		}
	}

	public void UpdatePlanetInterface(IPlanetInterface planet)
	{
		if (planet != null)
		{
			_planet = planet;
			SetTitle(planet.Name);
			UpdateBuildings(planet);
			planet.RecruitmentComponent.UpdateCurrentlyRecruitedUnitsEvent += UpdateRecruitment;
			ArmyView.ArmyInterfaceContainer.ArmyRecruitment.UpdateRecruitment(planet.RecruitmentComponent, IsLocalPlayer);
			ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateArmyList(planet.UnitController.UnitsList, IsLocalPlayer);
			ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateUnitsToTransfer -= planet.UnitController._on_UpdateUnitsToTransfer;
			ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateUnitsToTransfer += planet.UnitController._on_UpdateUnitsToTransfer;
		}
	}

	public void UpdatePlanetInterface(IPlanetInterface ship, IEnterCombat target)
	{
		UpdatePlanetInterface(ship);
		_transferTarget = target;
		_armyTransferPanel.UpdateTransferPanel(target.UnitController.UnitsList);
	}

	void _on_TransferArmy(){
		_planet.UnitController.TransferUnits(_transferTarget.UnitController, _armyTransferPanel.TransferArmy.UnitsToTransfer);
		_armyTransferPanel.Hide();
		ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateArmyList(_planet.UnitController.UnitsList, IsLocalPlayer);
	}

	void UpdateBuildings(IPlanetInterface planet)
	{
		var visibility = planet.VisibilityConroller.GetVisibility(LocalPlayerID);
		if (visibility.Visible == true && visibility.Visibility == VisibilityConroller.VisibilityState.Visible && planet.BuildingManager != null)
		{
			tabsOrganizer.ShowTab(0);
			if (planet.Controller != null)
			{
				if (planet.Controller.PlayerID == LocalPlayerID)
				{
					_buildingsPanel.BuildButton.Show();
					_buildingsPanel.UpdatePlanetBuildings(planet.BuildingManager, true);
					tabsOrganizer.TabContainer.SetTabDisabled(1, false);
				}
				else
				{
					_buildingsPanel.UpdatePlanetBuildings(planet.BuildingManager, false);
					_buildingsPanel.BuildButton.Hide();
					tabsOrganizer.TabContainer.SetTabDisabled(1, true);
				}
			}
			else
			{
				_buildingsPanel.UpdatePlanetBuildings(planet.BuildingManager, false);
				_buildingsPanel.BuildButton.Hide();
				tabsOrganizer.TabContainer.SetTabDisabled(1, true);
			}
		}
		else
		{
			// _buildingsPanel.HidePlanetBuildings();
			// _buildingsPanel.BuildButton.Hide();
			tabsOrganizer.HideTab(0);
		}
	}

	void _on_ConstructionListChanged(List<Building> buildings)
	{
		UpdateBuildings(_planet);
	}

	bool CheckBuildingResources(Planet planet, Building building)
	{
		if (building.Type == Building.Category.Mine)
			foreach (var resName in building.Products.Keys)
			{
				if (!planet.ResourcesManager.Resources.ContainsKey(resName))
				{
					return false;
				}
			}
		// foreach(Resource resource in building.ProductCost){
		// 	if(!planet.ResourcesManager.Resources.ContainsKey(resource.Name)){
		// 		//return false;
		// 	}
		// }
		return true;
	}

	public void _on_BuildingLabelGuiInputEvent(Building building, bool tooExpensive)
	{
		_buildingInterface.Visible = true;
		_buildingInterface.UpdateInterface(building, tooExpensive);
	}

	public void _on_BuildingLabelGuiInputEvent(Unit unit, bool tooExpensive)
	{
		_buildingInterface.Visible = true;
		_buildingInterface.UpdateInterface(unit, tooExpensive);
	}

	public void _on_StopBuildingConstruction(int Position)
	{
		_planet.BuildingManager.StopConstruction(_planet.Controller, Position);
		_buildingsPanel.UpdatePlanetBuildings(_planet.BuildingManager, true);
	}

	public void _on_StopBuildingConstruction(Building building)
	{
		_planet.BuildingManager.StopConstruction(_planet.Controller, building);

	}

	public void _mouseLeftBuildingLabel()
	{
		_buildingInterface.Visible = false;
	}

	void _on_XButton_button_up()
	{
		Visible = false;
		_buildingInterface.Visible = false;
	}

	public void UpdateRecruitment(RecruitmentComponent recruitmentComponent)
	{
		ArmyView.ArmyInterfaceContainer.ArmyRecruitment.UpdateRecruitment(recruitmentComponent, IsLocalPlayer);
		ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateArmyList(_planet.UnitController.UnitsList, IsLocalPlayer);
	}

	public void _on_StartUnitConstruction(Unit unit)
	{
		//_planet.StartConstruction((IConstruct)((PackedScene)GD.Load(unit.SceneFilePath)).Instantiate());
		var rc = _planet.RecruitmentComponent.AvaialableUnitsMap.Keys.ElementAt(0);
		_planet.RecruitmentComponent.StartConstruction(rc, unit);
		rc.UpdateCanPay(_planet.Controller.ResManager);
		ArmyView.ArmyInterfaceContainer.ArmyRecruitment.UpdateRecruitment(_planet.RecruitmentComponent, IsLocalPlayer);
	}

	public void _on_StartUnitConstruction(Building building)
	{
		if (_planet.BuildingManager.StartBuilding(building))
		{
			_buildMenu.HideBuilding(building);
			_buildingsPanel.UpdatePlanetBuildings(_planet.BuildingManager, true);
		}
	}

	public void _on_StopUnitConstruction(int Position)
	{
		_planet.RecruitmentComponent.StopConstruction(Position);
		ArmyView.ArmyInterfaceContainer.ArmyRecruitment.UpdateRecruitment(_planet.RecruitmentComponent, IsLocalPlayer);	
	}

	void _on_build_button_mouse_exited()
	{
		//_buildMenu.Hide(); 
	}

	public void _on_build_button_mouse_entered()
	{
		var pos = GetGlobalMousePosition();
		var x = pos.X - (_buildMenu.Size.X / 2);
		var y = pos.Y - _buildMenu.Size.Y + 20;
		_buildMenu.Position = new Vector2(x, y);
		_planet.BuildingManager.UpdateCanPayBuildings(_planet.Controller.ResManager);
		_buildMenu.UpdateBuildMenu(_planet.BuildingManager);
		_buildMenu.Show();
	}

	public void _on_recruit_button_mouse_entered()
	{
			var pos = GetGlobalMousePosition();
			var x = pos.X - (_unitMenu.Size.X / 2);
			var y = pos.Y - _unitMenu.Size.Y + 20;
			_unitMenu.Position = new Vector2(x, y);
			//_planet.RecruitmentManager.UpdateCanPay(_planet.Controller.ResManager);
			var rc = _planet.RecruitmentComponent;
			if(rc != null){
				if(rc.AvaialableUnitsMap.Count > 0 ){
					rc.AvaialableUnitsMap.Keys.ElementAt(0).UpdateCanPay(_planet.Controller.ResManager);
					_unitMenu.UpdateBuildMenu(rc.AvaialableUnitsMap.Keys.ElementAt(0));
				} 
			}
			_unitMenu.Show();
		
	}

	public void ConnectToSelectObjectInOrbit(Node node, string methodName)
	{
		Connect(nameof(SelectObjectInOrbit), new Callable(node, methodName));
	}

	new public void Hide()
    {
		base.Hide();
		ArmyView.ArmyInterfaceContainer.ArmyPanel.ResetPanel();
		if(_planet != null) ArmyView.ArmyInterfaceContainer.ArmyPanel.UpdateUnitsToTransfer -= _planet.UnitController._on_UpdateUnitsToTransfer;
    }
}
