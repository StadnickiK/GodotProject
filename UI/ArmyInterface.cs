using Godot;
using System;
using Godot.Collections;
using System.Linq;
using System.Collections.Generic;

public partial class ArmyInterface : Control
{
	[Signal]
    public delegate void DeselectEventHandler();

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/BuildingLabel.tscn";

	Button _closeButton = null;

	Button _recruitButton = null;

	Header _header = null;

	Ship _mapArmy = null;

	public Data _data { get; set; }

	ArmyPanel _armyPanel = null;

	ArmyRecruitment _armyRecruitment= null;

	InfoPanel info;

	BuildMenu _buildMenu;

	BuildingInterface _buildingInterface = null;

	[Export]
	public string Title { get; set; } = "Army Name";

	public int LocalPlayerID { get; set; }

	// BuildingLabel _selectedBuilding = null;

	[Signal]
	public delegate void SelectObjectInOrbitEventHandler(Planet planet, Node node);

	void GetNodes(){
		_closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
		_recruitButton = GetNode<Button>("VBoxContainer/HBoxContainer/BuildButton");
		_header = GetNode<Header>("VBoxContainer/Header");
		_buildingInterface = GetNode<BuildingInterface>("UnitInterface");
		_buildMenu = GetNode<BuildMenu>("BuildMenuScroll");
		_armyPanel = GetNode<ArmyPanel>("VBoxContainer/HBoxContainer/ArmyPanel");
		_armyRecruitment = GetNode<ArmyRecruitment>("VBoxContainer/HBoxContainer/Recruitment");
		info = GetNode<InfoPanel>("VBoxContainer/InfoPanel");
	}

	void UpdateInfo(Ship ship, System.Collections.Generic.List<Resource> resources){
		info.Controller.Text = "Owned by " + ship.Controller.Name;
		var upkeep = "\tUpkeep:";
		foreach (var item in ship.Units.UpkeepComponent.Upkeep)
		{
			upkeep += " " + item.Value+" " + resources[item.Key].IconPlaceholder;
		}
		info.Upkeep.Text = upkeep;
	}

	void ConnectSignals(){
		_closeButton.Connect("button_up", new Callable(this, nameof(_on_XButton_button_up)));
		_armyPanel.ConnectUnitCards(this);
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

	public void UpdateArmyPanel(Ship ship, System.Collections.Generic.List<Resource> resources){
		if(ship != null){
			var visibility = ship.VisibilityConroller.GetVisibility(LocalPlayerID);
			Visible = true;
			_mapArmy = ship;
			SetTitle(ship.Name);
			UpdateInfo(ship, resources);
			_armyPanel.UpdateArmyList(ship.Units.UnitsList);
			_armyRecruitment.UpdateRecruitment(ship.RecruitmentComponent, _data.Units, ship.Controller.PlayerID == LocalPlayerID);
			ship.RecruitmentComponent.UpdateCurrentlyRecruitedUnitsEvent += UpdateRecruitment;
			// if(ship.Vision){

			// }
		}
	}

	void UpdateRecruitment(RecruitmentComponent recruitmentComponent){
		_armyRecruitment.UpdateRecruitment(recruitmentComponent, _data.Units, _mapArmy.Controller.PlayerID == LocalPlayerID);
	}

	public void DeselectArmy(){
		_mapArmy.RecruitmentComponent.UpdateCurrentlyRecruitedUnitsEvent -= UpdateRecruitment;
	}

	

	public void InitRecruitmentPanel(){
		_buildMenu.InitAllUnits(_data.GetData("Units"), this);
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

	public void _on_LabelGuiInputEvent(InputEvent input, Node node){
		if(input is InputEventMouseButton button){
			if(button.ButtonIndex == MouseButton.Left){
				//EmitSignal(nameof(SelectObjectInOrbit), _planet, node);
			}
		}
	}

	public void _on_BuildingLabelGuiInputEvent(Unit unit, bool tooExpensive){
		_buildingInterface.Visible = true;
		_buildingInterface.UpdateInterface(unit, tooExpensive);
	}

	public void _mouseLeftBuildingLabel(){
		_buildingInterface.Visible = false;
	}	

	void _on_XButton_button_up(){
		Visible = false;
		_buildingInterface.Visible = false;
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
		_mapArmy.RecruitmentComponent.StartConstruction(unit);

		
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
		var rc = _mapArmy.GetNodeOrNull<RecruitmentComponent>("RecruitmentComponent");
		if(rc != null){
			if(rc.AvaialableUnitsMap.Count > 0 ){
				rc.AvaialableUnitsMap.Keys.ElementAt(0).UpdateCanPayUnits(_mapArmy.Controller.ResManager, _data.Units);
				_buildMenu.UpdateBuildMenu(rc.AvaialableUnitsMap.Keys.ElementAt(0));
			} 
		}
			
	}

	public override void _Process(double delta){
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
