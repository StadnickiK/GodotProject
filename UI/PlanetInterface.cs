using Godot;
using System;
using System.Collections.Generic;


public class PlanetInterface : Panel
{

	Button _closeButton = null;

	Header _header = null;

	Planet _planet = null;

	List<Building> _allBuildings = null;

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

	public void UpdatePlanetInterface(Planet planet, List<Building> allBuildings){
		if(planet != null){
			_planet = planet;
			_allBuildings = allBuildings;
			SetTitle(planet.Name);
			ClearPlanetInterface();
			if(planet.Vision){
				UpdateOverview(planet);
				UpdateOrbit(planet);
				UpdateBuildings(planet, allBuildings);
				UpdateTransferPanel(planet);
				_overviewPanel.ConnectToGuiInputEvent(this, "Orbit", nameof(_on_LabelGuiInputEvent));
				_overviewPanel.ConnectToGuiInputEvent(this, "Buildings", nameof(_on_BuildingLabelGuiInputEvent));
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
		foreach(Resource resource in planet.ResourcesManager.Resources.Values){
			label = new Label();
			label.Text = resource.Name;
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
		UpdateTransferPanel(planet);
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
				foreach(Resource resource in manager.ResourcesManager.Resources.Values){
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

	void UpdateBuildings(Planet planet, List<Building> allBuildings){
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
			label.Text = building.Name;
			_overviewPanel.AddNodeToPanel("Buildings", label);
			label.Progress.PercentVisible = false;
			if(label.Progress != null){                             // ProgressBar was null, bcs label.getnodes method is executed when label enters the tree, so it has to be done after AddNodeToPanel
				label.Progress.Value = building.CurrentTime;
				label.Progress.MaxValue = building.BuildTime;
			}
		}
	}

	void UpdateAllBuildings(List<Building> buildings, Planet planet){
		foreach(Building building in buildings){
			if(CheckBuildingResources(planet, building)){
				if(!_planet.BuildingsManager.Buildings.Contains(building)){
					var label = (BuildingLabel)ItemScene.Instance();
					label.SetMeta(building.Name, building);
					label.Name = building.Name;
					label.Text = building.Name;
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
		foreach(Resource resource in building.Products){
			if(!planet.ResourcesManager.Resources.ContainsKey(resource.Name)){
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
			if(button.ButtonIndex == (int)ButtonList.Left){
				EmitSignal(nameof(SelectObjectInOrbit), _planet, node);
			}
		}
	}

	public void _on_BuildingLabelGuiInputEvent(InputEvent input, Node node){
		if(input is InputEventMouseButton button){
			if(button.ButtonIndex == (int)ButtonList.Left){
				if(node is BuildingLabel label){
					var building = (Building)label.GetMeta(label.Text);
					if(building != null && _planet != null){
						_buildingInterface.Visible = true;
						_buildingInterface.UpdateInterface(building, _planet);
						_selectedBuilding = label;
					}
				}
			}
		}
	}

	void _on_XButton_button_up(){
		Visible = false;
		_buildingInterface.Visible = false;
	}

	void _on_StartConstruction(Node node){
		if(node is Building building){
			if(building != null && _planet != null){
				if(_planet.Controller.ResManager.PayCost(building.BuildCost)){
					_planet.BuildingsManager.ConstructBuilding(building);
				}else{

				}
			}
		}
		if(node is Unit unit){
			if(unit != null && _planet != null){
				_planet.ConstructUnit(unit);
			}
		}
	}

	public void ConnectToSelectObjectInOrbit(Node node, string methodName){
		Connect(nameof(SelectObjectInOrbit), node, methodName);
	}

	public override void _Process(float delta){
		if(Visible){
			if(_planet != null && _allBuildings != null){
				if(_planet.BuildingsManager.BuildingsChanged){
					UpdateBuildings(_planet, _allBuildings);
					_planet.BuildingsManager.BuildingsChanged = false;
				}
			}
		}
	}
}
