using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
// using System.Collections.Generic;


public partial class PlanetInterface : Control
{

	Button _closeButton = null;

	Header _header = null;

	Planet _planet = null;

	Godot.Collections.Array _allBuildings = null;

	Array<Unit> _allUnits = null;

	public Data _data { get; set; }

	bool Cleanup = false;

	BuildingPanel _buildings = null;

	BuildMenu _buildMenu;

	BuildMenu _recruitmentMenu;

	TabContainer tabContainer;

	BuildingInterface _buildingInterface = null;

	[Export]
	public string Title { get; set; } = "Title";

	public int LocalPlayerID { get; set; }

	// BuildingLabel _selectedBuilding = null;

	[Signal]
	public delegate void SelectObjectInOrbitEventHandler(Planet planet, Node node);

	void GetNodes(){
		_closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
		_header = GetNode<Header>("VBoxContainer/Header");
		_buildingInterface = GetNode<BuildingInterface>("BuildingInterface");
		_buildMenu = GetNode<BuildMenu>("BuildMenuScroll");
		_recruitmentMenu = GetNode<BuildMenu>("VBoxContainer/Tabs/Recruitment");
		_buildings = GetNode<BuildingPanel>("VBoxContainer/Tabs/Buildings");
		tabContainer = GetNode<TabContainer>("VBoxContainer/Tabs");
	}

	void ConnectSignals(){
		_closeButton.Connect("button_up", new Callable(this, nameof(_on_XButton_button_up)));
		_buildings.ConnectBuildButtons(this);
		// _buildMenu.ConnectBuildButtons(this);
		foreach(BuildingLabel buildingLabel in _buildMenu.buildingLabels){
			buildingLabel.BButton.ButtonUp += () => _on_StartConstruction(buildingLabel.RefBuilding);
		}
	}

	public void SetTitle(string title){
		_header.SetTitle(title);
	}

	public override void _Ready()
	{
		GetNodes();
	}

    public void InitBuildingsPanel(){
        _buildings = GetNode<BuildingPanel>("VBoxContainer/Tabs/Buildings");
		_buildMenu.InitAllBuildings(_data.Buildings, this);
		_recruitmentMenu.InitAllUnits(_data.GetData("Units"), this);
		ConnectSignals();
		_buildings.InitAllBuildings(_data.Buildings, this);
	}

	public void UpdatePlanetInterface(Planet planet){
		if(planet != null){
			_planet = planet;
			SetTitle(planet.Name);
			UpdateBuildings(planet);
		}
	}

	void UpdateOverview(Planet planet){
		var label = new Label();
		if(planet.Controller != null){
			label.Name = planet.Controller.Name;
			label.Text = "Planet controller: " + planet.Controller.Name;
		}else{
			label.Text = "Planet controller: ";
		}
		//_overviewPanel.AddNodeToPanel("Overview", label);
		label = new Label();
		label.Text = "\nPlanet Resources: \n";
		//_overviewPanel.AddNodeToPanel("Overview", label);
		foreach(var resourceName in planet.ResourcesManager.Resources.Keys){
			label = new Label();
			label.Text = resourceName.ToString();
			//_overviewPanel.AddNodeToPanel("Overview", label);
		}
	}

	void UpdateOrbit(Planet planet){
		foreach(PhysicsBody3D body in planet.Orbit.GetChildren()){
			var label = new Label();
			label.Name = label.Text = body.Name;
			label.SetMeta(label.Name, body);
			var node = (Node)label.GetMeta(label.Name);
			//_overviewPanel.AddNodeToPanel("Orbit", label);
		}
	}

	void _on_ResetTransferPanel(Planet planet){
		// _overviewPanel.ClearPanel("Resource transfer");
		// _overviewPanel.GetPanel("Resource transfer").GetHeader()?.GetChild(1)?.QueueFree(); // remove reset button
		// UpdateTransferPanel(planet);
	}

	void _on_TransferResources(CollisionObject3D left, CollisionObject3D body){
		// if(left is Planet planet && body is Ship ship){
		// 	///*
		// 	foreach(TransferLabel label in _overviewPanel.GetPanel("Resource transfer").GetNode("ItemList/Items").GetChildren()){
		// 		int value = (int)label.NumInput.Value;
		// 		if(value > 0){
		// 			// if(label.IsRight){
		// 			// 	planet.ResourcesManager.TransferResources(ship.ResourcesManager, label.Name, (int)label.NumInput.Value);
		// 			// }else{
		// 			// 	ship.ResourcesManager.TransferResources(planet.ResourcesManager, label.Name, (int)label.NumInput.Value);
		// 			// }
		// 		}
		// 	}
		// 	//*/
		// }
	}

	void _on_transferButton_up(Ship body, Planet planet){
		if(body is IResourceManager manager){
			// _overviewPanel.ClearPanel("Resource transfer");
			Button button = new Button();
			button.Text = "X";
			button.SizeFlagsHorizontal = SizeFlags.Fill;
			Godot.Collections.Array arr = new Godot.Collections.Array();
			arr.Add(planet);
			button.Connect("button_up", new Callable(this, nameof(_on_ResetTransferPanel)));

			//button.Connect("button_up", new Callable(this, nameof(_on_ResetTransferPanel)), arr);

			// _overviewPanel.GetPanel("Resource transfer").GetHeader().AddChild(button);

 			button = new Button();
			button.Text = "Transfer";
			button.SizeFlagsHorizontal = SizeFlags.Fill;
			arr = new Godot.Collections.Array();
			arr.Add(planet);
			arr.Add(body);
			//button.Connect("button_up", new Callable(this, nameof(_on_TransferResources)), arr);

			button.Connect("button_up", new Callable(this, nameof(_on_TransferResources)));

			// _overviewPanel.GetPanel("Resource transfer").GetFoot().AddChild(button);

			if(manager.ResourcesManager.Resources.Count > 0){
				foreach(var resource in manager.ResourcesManager.Resources.Values){
					// TransferLabel label = (TransferLabel)_transferLabelScene.Instantiate();
					// if(planet.ResourcesManager.Resources.ContainsKey(resource.Name)){
					// 	label.UpdateLabel(resource.Name, planet.ResourcesManager.Resources[resource.Name].Value, resource.Value );
					// }else{
					// 	label.UpdateLabel(resource.Name, 0, resource.Value );
					// }
					// _overviewPanel.AddNodeToPanel("Resource transfer", label);
				}
			}else{
				// foreach(Resource resource in planet.ResourcesManager.Resources.Values){
				// 	TransferLabel label = (TransferLabel)_transferLabelScene.Instance();
				// 	label.UpdateLabel(resource.Name, planet.ResourcesManager.Resources[resource.Name].Value, 0);
				// 	_overviewPanel.AddNodeToPanel("Resource transfer", label);
				// }
			}
		}
	}

	void UpdateTransferPanel(Planet planet){
		// _overviewPanel.ClearPanel("Resource transfer");
		foreach(Ship body in planet.Orbit.GetChildren()){
			if(body is IMapObjectController controller){
				if(controller.Controller != null)
					if(controller.Controller.PlayerID == LocalPlayerID){
						var button = new Button();
						button.Name = button.Text = body.Name;
						Godot.Collections.Array arr = new Godot.Collections.Array();
						arr.Add(body);
						arr.Add(planet);
						//button.Connect("button_up", new Callable(this, nameof(_on_transferButton_up)), arr);
						button.Connect("button_up", new Callable(this, nameof(_on_transferButton_up)));
						button.SetMeta(button.Name, body);
						var node = (Node)button.GetMeta(button.Name);
						// _overviewPanel.AddNodeToPanel("Resource transfer", button);
					}
			}
		}
	}

	void UpdateBuildings(Planet planet){
		var visibility = planet.VisibilityConroller.GetVisibility(LocalPlayerID);
		if( visibility.Visible == true && visibility.Visibility == VisibilityConroller.VisibilityState.Visible){
			if(planet.Controller != null){
				if(planet.Controller.PlayerID == LocalPlayerID){
					_buildings.BuildButton.Show();
					_buildings.UpdatePlanetBuildings(planet.BuildingsManager, true);
					tabContainer.SetTabDisabled(1, false);
				}else{
					_buildings.UpdatePlanetBuildings(planet.BuildingsManager, false);
					_buildings.BuildButton.Hide();
					tabContainer.SetTabDisabled(1, true);
				}
			}else{
				_buildings.UpdatePlanetBuildings(planet.BuildingsManager, false);
				_buildings.BuildButton.Hide();
				tabContainer.SetTabDisabled(1, true);
			}
		}else{
			_buildings.HidePlanetBuildings();
			_buildings.BuildButton.Hide();
			tabContainer.SetTabDisabled(1, true);
		}
	}

	bool CheckBuildingResources(Planet planet, Building building){
		if(building.Type == Building.Category.Mine)
			foreach(var resName in building.Products.Keys){
				if(!planet.ResourcesManager.Resources.ContainsKey(resName)){
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

	public void _on_LabelGuiInputEvent(InputEvent input, Node node){
		if(input is InputEventMouseButton button){
			if(button.ButtonIndex == MouseButton.Left){
				EmitSignal(nameof(SelectObjectInOrbit), _planet, node);
			}
		}
	}

	public void _on_BuildingLabelGuiInputEvent(Building building, bool tooExpensive){
		_buildingInterface.Visible = true;
		_buildingInterface.UpdateInterface(building, tooExpensive);
	}

	public void _on_BuildingLabelGuiInputEvent(Unit unit, bool tooExpensive){
		_buildingInterface.Visible = true;
		_buildingInterface.UpdateInterface(unit, tooExpensive);
	}

	public void _on_StopBuildingConstruction(int Position){
		_planet.BuildingsManager.StopConstruction(_planet.Controller, Position);
		_buildings.UpdatePlanetBuildings(_planet.BuildingsManager, true);
	}

	public void _on_StopBuildingConstruction(Building building){
		_planet.BuildingsManager.StopConstruction(_planet.Controller, building);
		
	}

	public void _mouseLeftBuildingLabel(){
		_buildingInterface.Visible = false;
	}	

	void _on_XButton_button_up(){
		Visible = false;
		_buildingInterface.Visible = false;
	}

	void _on_StartConstruction(Node node){
		if(node is Unit unit){
			if(node != null && _planet != null){
				_planet.StartRecruitment(unit);
			}
		}else if(node is Building building){
			if(_planet.StartBuilding(building)){
				_buildMenu.HideBuilding(building);
				_buildings.UpdatePlanetBuildings(_planet.BuildingsManager, true);
			}
				
		}
	}

	public void _on_StartUnitConstruction(Unit unit){
				_planet.StartConstruction((IConstruct)((PackedScene)GD.Load(unit.SceneFilePath)).Instantiate());
				_planet.ConstructUnit(unit);
	}

	void _on_build_button_mouse_exited(){
		//_buildMenu.Hide(); 
	}

	public void _on_build_button_mouse_entered(){
		var pos = GetGlobalMousePosition();
		var x = pos.X-(_buildMenu.Size.X/2);
		var y = pos.Y-_buildMenu.Size.Y+20;
		_buildMenu.Position = new Vector2(x, y);
		_planet.BuildingsManager.UpdateCanPayBuildings(_planet.Controller.ResManager);
		_buildMenu.UpdateBuildMenu(_planet.BuildingsManager);
		_buildMenu.Show(); 
	}

	public void ConnectToSelectObjectInOrbit(Node node, string methodName){
		Connect(nameof(SelectObjectInOrbit), new Callable(node, methodName));
	}

	public override void _Process(double delta){
		if(Visible){
			if(_planet != null && _data != null){
				if(_planet.BuildingsManager.ConstructionListChanged){
					UpdateBuildings(_planet);
					_planet.BuildingsManager.ConstructionListChanged = false;
				}
				// if(_planet.Constructions.HasConstruct || Cleanup == true){
				// 	//UpdateConstruction(_planet);
				// }
			}
		}
	}
}
