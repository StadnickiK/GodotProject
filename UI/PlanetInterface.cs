using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
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
		_buildMenu.InitAllBuildings(_data.GetData("Buildings"), this);
		_recruitmentMenu.InitAllUnits(_data.GetData("Units"), this);
		ConnectSignals();
		_buildings.InitAllBuildings(_data.Buildings, this);
	}

	public void UpdatePlanetInterface(Planet planet){
		if(planet != null){
			_planet = planet;
			SetTitle(planet.Name);
			if(planet.Vision){
				UpdateOverview(planet);
				UpdateOrbit(planet);
				UpdateBuildings(planet);
				// UpdateTransferPanel(planet);
				//UpdateConstruction(planet);
				// if(planet.BuildingsManager.HasConstruction)
				// 	_buildQueue.UpdateBuildQueue(planet.BuildingsManager.CurrentConstruction());
			}
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
		// _overviewPanel.ClearPanel("Buildings");
		UpdatePlanetBuildings(planet.BuildingsManager.Buildings);
		if(planet.Controller != null){
			if(planet.Controller.PlayerID == LocalPlayerID){
				var tempLabel = new Label();
				tempLabel.Text = "\n Construction list: \n";
				_buildMenu.UpdateBuildMenu(planet.BuildingsManager);
				_buildings.UpdatePlanetBuildings(planet.BuildingsManager);
				UpdateAllBuildings(planet);
			}
		}
	}

	void UpdatePlanetBuildings(List<Building> buildings){
		// foreach(Building building in buildings){
		// 	var label = (BuildingLabel)ItemScene.Instantiate();
		// 	//label.SetMeta(building.Name, building);
		// 	label.RefBuilding = building;
		// 	label.Name = building.Name;
		// 	if(label.BButton != null){
		// 		label.BButton.Text = building.Name;
		// 	}else{
		// 		label.BButton = label.GetNode<Button>("Button");
		// 		label.BButton.Text = building.Name;
		// 	}
		// 	// _overviewPanel.AddNodeToPanel("Buildings", label);
		// 	label.Progress.Visible = false;
		// 	if(label.Progress != null){                             // ProgressBar was null, bcs label.getnodes method is executed when label enters the tree, so it has to be done after AddNodeToPanel
		// 		label.Progress.Value = 0;							// has to be 0 otherwise progress is visible
		// 		label.Progress.MaxValue = building.BuildTime;
		// 	}
		// }
	}

	void UpdateAllBuildings(Planet planet){ // todo: Separate construction and building list into separate nodes for better organization
		// var construction = planet.BuildingsManager.CurrentConstruction();
		// foreach(Node node in _data.GetData("Buildings")){
		// 	if(node is Building building)
		// 		if(CheckBuildingResources(planet, building)){
		// 			if(_planet.BuildingsManager.Buildings.Find(x => x.Name == building.Name) == null){
		// 				var label = (BuildingLabel)ItemScene.Instantiate();
		// 				label.RefBuilding = building;
		// 				// label.SetMeta(building.Name.Replace(" ",""), building);
		// 				label.Name = building.Name;
		// 				if(label.BButton != null){
		// 					label.BButton.Text = building.Name;
		// 				}else{
		// 					label.BButton = label.GetNode<Button>("Button");
		// 					label.BButton.Text = building.Name;
		// 				}
		// 				// _overviewPanel.AddNodeToPanel("Buildings", label);
		// 				if(label.Progress != null && construction.Count > 0){                             // ProgressBar was null, bcs label.getnodes method is executed when label enters the tree, so it has to be done after AddNodeToPanel
		// 					var currentBuilding = construction.Find(x => x.Name == building.Name);
		// 					if(currentBuilding != null){
		// 						label.Progress.Value = currentBuilding.CurrentTime;
		// 						label.Progress.MaxValue = currentBuilding.BuildTime;
		// 					}
		// 				}
		// 			}
		// 		}
		// }
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

	void UpdateConstruction(Planet planet){
		// _overviewPanel.ClearPanel("Construction");
		if(planet.Controller != null){
			if(planet.Controller.PlayerID == LocalPlayerID){
				var tempLabel = new Label();
				tempLabel.Text = "\n Construction list: \n";
				// _overviewPanel.AddNodeToPanel("Construction", tempLabel);
				UpdateConstructionList(planet);
			}
		}
	}

	void UpdateConstructionList(Planet planet){
		// Cleanup = true;
		// foreach(var node in _data.GetData("Units"))
		// 	if(node is Unit unit){
		// 		var label = (BuildingLabel)ItemScene.Instantiate();
		// 		label.SetMeta(unit.Name.ToString().Replace(" ", ""), unit);
		// 		label.Name = unit.Name;
		// 		if(label.BButton != null){
		// 			label.BButton.Text = unit.Name;
		// 		}else{
		// 			label.BButton = label.GetNode<Button>("Button");
		// 			label.BButton.Text = unit.Name;
		// 		}
		// 		// _overviewPanel.AddNodeToPanel("Construction", label);
		// 		var constList = planet.Constructions.CurrentConstruction();
		// 		if(constList != null){
		// 			if(constList.Count > 0){
		// 				if(constList[0] is Unit currentUnit){
		// 					if(currentUnit.Name == unit.Name){
		// 						label.Progress.Value = currentUnit.CurrentTime;
		// 						label.Progress.MaxValue = currentUnit.BuildTime;
		// 					}
		// 				}
		// 			}else{
		// 				label.Progress.Value = 0;
		// 				label.Progress.MaxValue = unit.BuildTime;
		// 				Cleanup = false;
		// 			}
		// 		}
		// 	}
	}

	public void _on_LabelGuiInputEvent(InputEvent input, Node node){
		if(input is InputEventMouseButton button){
			if(button.ButtonIndex == MouseButton.Left){
				EmitSignal(nameof(SelectObjectInOrbit), _planet, node);
			}
		}
	}

	public void _on_BuildingLabelGuiInputEvent(Building building){
		_buildingInterface.Visible = true;
		_buildingInterface.UpdateInterface(building);
	}

	public void _on_BuildingLabelGuiInputEvent(Unit unit){
		_buildingInterface.Visible = true;
		_buildingInterface.UpdateInterface(unit);
	}

	public void _mouseLeftBuildingLabel(){
		_buildingInterface.Visible = false;
	}	

	void _on_XButton_button_up(){
		Visible = false;
		_buildingInterface.Visible = false;
	}

	void _on_StartConstruction(Node node){
		if(node is IBuilding unit){
			if(unit != null && _planet != null){
				_planet.StartConstruction(unit);
				// _planet.ConstructUnit(unit);
			}
		}
	}

	void _on_StartUnitConstruction(Unit unit){
				_planet.StartConstruction((IBuilding)((PackedScene)GD.Load(unit.SceneFilePath)).Instantiate());
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
				if(_planet.Constructions.HasConstruct || Cleanup == true){
					UpdateConstruction(_planet);
				}
			}
		}
	}
}
