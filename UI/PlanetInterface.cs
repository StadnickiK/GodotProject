using Godot;
using System;
using System.Collections.Generic;


public class PlanetInterface : Panel
{

	Button _closeButton = null;

	Header _header = null;

	Planet _planet = null;

	Godot.Collections.Array _allBuildings = null;

	List<Unit> _allUnits = null;

	public Data _data { get; set; }

	OverviewPanel _overviewPanel = null;

	BuildingInterface _buildingInterface = null;

	[Export]
	public string Title { get; set; } = "Title";

	[Export]
	public Dictionary<string, string> Options { get; set; } = new Dictionary<string, string>();

	[Export]
	public string ItemScenePath { get; set; } = "res://UI/BuildingLabel.tscn";

	PackedScene _transferLabelScene = (PackedScene)ResourceLoader.Load("res://UI/TransferLabel.tscn");

	public int LocalPlayerID { get; set; }

	PackedScene ItemScene = null;

	BuildingLabel _selectedBuilding = null;

	[Signal]
	public delegate void SelectObjectInOrbit(Planet planet, Node node);

	void GetNodes(){
		_closeButton = GetNode<Button>("VBoxContainer/Header/XButton");
		_header = GetNode<Header>("VBoxContainer/Header");
		_overviewPanel = GetNode<OverviewPanel>("VBoxContainer/OverviewPanel");
		_buildingInterface = GetNode<BuildingInterface>("BuildingInterface");
	}

	void ConnectSignals(){
		_closeButton.Connect("button_up", this, nameof(_on_XButton_button_up));
		_buildingInterface.ConnecToStartConstruction(this, nameof(_on_StartConstruction));
	}

	public void SetTitle(string title){
		_header.SetTitle(title);
	}

	void InitOverviewPanel(){
		foreach(KeyValuePair<string, string> pair in Options){
			if(pair.Value == null){
				_overviewPanel.AddPanel(pair.Key);
			}else{
				PackedScene scene = (PackedScene)ResourceLoader.Load(pair.Value);
				if(scene != null){
					_overviewPanel.AddPanel(pair.Key, (CanvasItem)scene.Instance());
				}else{
					_overviewPanel.AddPanel(pair.Key);
				}
			}
		}
	}

	public override void _Ready()
	{
		GetNodes();
		ConnectSignals();
		InitOverviewPanel();
		ItemScene = (PackedScene)ResourceLoader.Load(ItemScenePath);
		//_overviewPanel.AddNodeToPanel("Resource transfer", _transfer);
	}

	void ClearPlanetInterface(){
		foreach(KeyValuePair<string, string> pair in Options){
			if(pair.Value == null){
				_overviewPanel.ClearPanel(pair.Key);
			}
		}
	}

	public void UpdatePlanetInterface(Planet planet, Godot.Collections.Array allBuildings,  List<Unit> worldUnits){
		if(planet != null){
			_planet = planet;
			_allBuildings = allBuildings;
			_allUnits = worldUnits;
			SetTitle(planet.Name);
			ClearPlanetInterface();
			if(planet.Vision){
				UpdateOverview(planet);
				UpdateOrbit(planet);
				UpdateBuildings(planet, allBuildings);
				// UpdateTransferPanel(planet);
				UpdateConstruction(planet, worldUnits);
				_overviewPanel.ConnectToGuiInputEvent(this, "Orbit", nameof(_on_LabelGuiInputEvent));
				_overviewPanel.ConnectToEvent(this, "Buildings", nameof(_on_BuildingLabelGuiInputEvent), "button_up");
				_overviewPanel.ConnectToEvent(this, "Construction", nameof(_on_BuildingLabelGuiInputEvent), "button_up");
			}
		}
	}

	public void UpdatePlanetInterface(Planet planet){
		if(planet != null){
			_planet = planet;
			SetTitle(planet.Name);
			ClearPlanetInterface();
			if(planet.Vision){
				UpdateOverview(planet);
				UpdateOrbit(planet);
				UpdateBuildings(planet, _data.GetData("Buildings"));
				// UpdateTransferPanel(planet);
				UpdateConstruction(planet, _data.GetData("Units"));
				_overviewPanel.ConnectToGuiInputEvent(this, "Orbit", nameof(_on_LabelGuiInputEvent));
				_overviewPanel.ConnectToEvent(this, "Buildings", nameof(_on_BuildingLabelGuiInputEvent), "button_up");
				_overviewPanel.ConnectToEvent(this, "Construction", nameof(_on_BuildingLabelGuiInputEvent), "button_up");
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
		_overviewPanel.AddNodeToPanel("Overview", label);
		label = new Label();
		label.Text = "\nPlanet Resources: \n";
		_overviewPanel.AddNodeToPanel("Overview", label);
		foreach(var resourceName in planet.ResourcesManager.Resources.Keys){
			label = new Label();
			label.Text = resourceName;
			_overviewPanel.AddNodeToPanel("Overview", label);
		}
	}

	void _on_orbitButton_up(){
		_buildingInterface.Visible = true;
		_buildingInterface.UpdateInterface();
	}

	void UpdateOrbit(Planet planet){
		if(planet.Controller != null){
			if(planet.Controller.PlayerID == LocalPlayerID){
				var button = new Button();
				button.Text = "Orbit build menu";
				button.Connect("button_up",this,nameof(_on_orbitButton_up));
				_overviewPanel.AddNodeToPanel("Orbit", button);
			}
		}
		foreach(PhysicsBody body in planet.Orbit.GetChildren()){
			var label = new Label();
			label.Name = label.Text = body.Name;
			label.SetMeta(label.Name, body);
			var node = (Node)label.GetMeta(label.Name);
			_overviewPanel.AddNodeToPanel("Orbit", label);
		}
	}

	void _on_ResetTransferPanel(Planet planet){
		_overviewPanel.ClearPanel("Resource transfer");
		_overviewPanel.GetPanel("Resource transfer").GetHeader()?.GetChild(1)?.QueueFree(); // remove reset button
		// UpdateTransferPanel(planet);
	}

	void _on_TransferResources(PhysicsBody left, PhysicsBody body){
		if(left is Planet planet && body is Ship ship){
			///*
			foreach(TransferLabel label in _overviewPanel.GetPanel("Resource transfer").GetNode("ItemList/Items").GetChildren()){
				int value = (int)label.NumInput.Value;
				if(value > 0){
					// if(label.IsRight){
					// 	planet.ResourcesManager.TransferResources(ship.ResourcesManager, label.Name, (int)label.NumInput.Value);
					// }else{
					// 	ship.ResourcesManager.TransferResources(planet.ResourcesManager, label.Name, (int)label.NumInput.Value);
					// }
				}
			}
			//*/
		}
	}

	void _on_transferButton_up(Ship body, Planet planet){
		if(body is IResourceManager manager){
			_overviewPanel.ClearPanel("Resource transfer");
			Button button = new Button();
			button.Text = "X";
			button.SizeFlagsHorizontal = 10;
			Godot.Collections.Array arr = new Godot.Collections.Array();
			arr.Add(planet);
			button.Connect("button_up", this, nameof(_on_ResetTransferPanel), arr);
			_overviewPanel.GetPanel("Resource transfer").GetHeader().AddChild(button);

 			button = new Button();
			button.Text = "Transfer";
			button.SizeFlagsHorizontal = 10;
			arr = new Godot.Collections.Array();
			arr.Add(planet);
			arr.Add(body);
			button.Connect("button_up", this, nameof(_on_TransferResources), arr);
			_overviewPanel.GetPanel("Resource transfer").GetFoot().AddChild(button);

			if(manager.ResourcesManager.Resources.Count > 0){
				foreach(var resource in manager.ResourcesManager.Resources.Values){
					TransferLabel label = (TransferLabel)_transferLabelScene.Instance();
					// if(planet.ResourcesManager.Resources.ContainsKey(resource.Name)){
					// 	label.UpdateLabel(resource.Name, planet.ResourcesManager.Resources[resource.Name].Value, resource.Value );
					// }else{
					// 	label.UpdateLabel(resource.Name, 0, resource.Value );
					// }
					_overviewPanel.AddNodeToPanel("Resource transfer", label);
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
		_overviewPanel.ClearPanel("Resource transfer");
		foreach(Ship body in planet.Orbit.GetChildren()){
			if(body is IMapObjectController controller){
				if(controller.Controller != null)
					if(controller.Controller.PlayerID == LocalPlayerID){
						var button = new Button();
						button.Name = button.Text = body.Name;
						Godot.Collections.Array arr = new Godot.Collections.Array();
						arr.Add(body);
						arr.Add(planet);
						button.Connect("button_up",this, nameof(_on_transferButton_up), arr);
						button.SetMeta(button.Name, body);
						var node = (Node)button.GetMeta(button.Name);
						_overviewPanel.AddNodeToPanel("Resource transfer", button);
					}
			}
		}
	}

	void UpdateBuildings(Planet planet, Godot.Collections.Array allBuildings){
		_overviewPanel.ClearPanel("Buildings");
		UpdatePlanetBuildings(planet.BuildingsManager.Buildings);
		if(planet.Controller != null){
			if(planet.Controller.PlayerID == LocalPlayerID){
				var tempLabel = new Label();
				tempLabel.Text = "\n Construction list: \n";
				_overviewPanel.AddNodeToPanel("Buildings", tempLabel);
				UpdateAllBuildings(allBuildings, planet);
			}
		}
	}

	void UpdatePlanetBuildings(List<Building> buildings){
		foreach(Building building in buildings){
			var label = (BuildingLabel)ItemScene.Instance();
			label.SetMeta(building.Name, building);
			label.Name = building.Name;
			if(label.BButton != null){
				label.BButton.Text = building.Name;
			}else{
				label.BButton = label.GetNode<Button>("Button");
				label.BButton.Text = building.Name;
			}
			_overviewPanel.AddNodeToPanel("Buildings", label);
			label.Progress.Visible = false;
			if(label.Progress != null){                             // ProgressBar was null, bcs label.getnodes method is executed when label enters the tree, so it has to be done after AddNodeToPanel
				label.Progress.Value = 0;							// has to be 0 otherwise progress is visible
				label.Progress.MaxValue = building.BuildTime;
			}
		}
	}

	void UpdateAllBuildings(Godot.Collections.Array buildings, Planet planet){
		foreach(Node node in buildings){
			if(node is Building building)
				if(CheckBuildingResources(planet, building)){
					if(!_planet.BuildingsManager.Buildings.Contains(building)){
						var label = (BuildingLabel)ItemScene.Instance();
						label.SetMeta(building.Name, building);
						label.Name = building.Name;
						if(label.BButton != null){
							label.BButton.Text = building.Name;
						}else{
							label.BButton = label.GetNode<Button>("Button");
							label.BButton.Text = building.Name;
						}
						_overviewPanel.AddNodeToPanel("Buildings", label);
						if(label.Progress != null){                             // ProgressBar was null, bcs label.getnodes method is executed when label enters the tree, so it has to be done after AddNodeToPanel
							label.Progress.Value = building.CurrentTime;
							label.Progress.MaxValue = building.BuildTime;
						}
					}
				}
		}
	}

	bool CheckBuildingResources(Planet planet, Building building){
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

	void UpdateConstruction(Planet planet, List<Unit> WorldUnits){
		_overviewPanel.ClearPanel("Construction");
		if(planet.Controller != null){
			if(planet.Controller.PlayerID == LocalPlayerID){
				var tempLabel = new Label();
				tempLabel.Text = "\n Construction list: \n";
				_overviewPanel.AddNodeToPanel("Construction", tempLabel);
				UpdateConstructionList(WorldUnits, planet);
			}
		}
	}

		void UpdateConstruction(Planet planet, Godot.Collections.Array WorldUnits){
		_overviewPanel.ClearPanel("Construction");
		if(planet.Controller != null){
			if(planet.Controller.PlayerID == LocalPlayerID){
				var tempLabel = new Label();
				tempLabel.Text = "\n Construction list: \n";
				_overviewPanel.AddNodeToPanel("Construction", tempLabel);
				//UpdateConstructionList(WorldUnits, planet);
			}
		}
	}


	// todo: Reimplement with construction list instead of constList[0]
	void UpdateConstructionList(List<Unit> WorldUnits, Planet planet){
		foreach(var unit in WorldUnits){
			var label = (BuildingLabel)ItemScene.Instance();
			label.SetMeta(unit.Name, unit);
			label.Name = unit.Name;
			if(label.BButton != null){
				label.BButton.Text = unit.Name;
			}else{
				label.BButton = label.GetNode<Button>("Button");
				label.BButton.Text = unit.Name;
			}
			_overviewPanel.AddNodeToPanel("Construction", label);
			if(label.Progress != null){                             // ProgressBar was null, bcs label.getnodes method is executed when label enters the tree, so it has to be done after AddNodeToPanel
				label.Progress.Value = unit.CurrentTime;
				label.Progress.MaxValue = unit.BuildTime;
			}
			var constList = planet.Constructions.CurrentConstruction();
			if(constList != null)
				if(constList.Count > 0){
					if(constList[0] is Unit currentUnit){
						if(currentUnit.Name == unit.Name){
							label.Progress.Value = currentUnit.CurrentTime;
							label.Progress.MaxValue = currentUnit.BuildTime;
						}
					}
				}
		}
	}

	void UpdatePlanetConstruction(Planet planet, List<Unit> WorldUnits){

	}

	public void _on_LabelGuiInputEvent(InputEvent input, Node node){
		if(input is InputEventMouseButton button){
			if(button.ButtonIndex == (int)ButtonList.Left){
				EmitSignal(nameof(SelectObjectInOrbit), _planet, node);
			}
		}
	}

	public void _on_BuildingLabelGuiInputEvent(Node node){
		// if(input is InputEventMouseButton button){
		// 	if(button.ButtonIndex == (int)ButtonList.Left){
				if(node is BuildingLabel label){
					if(label.GetMeta(label.BButton.Text) is Building building)
						if(building != null && _planet != null){
							_buildingInterface.Visible = true;
							_buildingInterface.UpdateInterface(building, _planet);
							_selectedBuilding = label;
						}
					if(label.GetMeta(label.BButton.Text) is Unit unit){
							_buildingInterface.Visible = true;
							_buildingInterface.UpdateInterface(unit, _planet);
							_selectedBuilding = label;
					}
				}
		// 	}
		// }
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

	public void ConnectToSelectObjectInOrbit(Node node, string methodName){
		Connect(nameof(SelectObjectInOrbit), node, methodName);
	}

	public override void _Process(float delta){
		if(Visible){
			if(_planet != null && _allBuildings != null){
				if(_planet.BuildingsManager.ConstructionListChanged){
					UpdateBuildings(_planet, _allBuildings);
					_planet.BuildingsManager.ConstructionListChanged = false;
				}
				if(_planet.Constructions.HasConstruct())
					UpdateConstruction(_planet, _allUnits);
			}
		}
	}
}
