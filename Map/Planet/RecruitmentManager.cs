using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RecruitmentManager : Node, IMapObjectController
{
    [Export]
    public string VisionComponentPath { get; set; } = "res://Units/Base/VisionComponent.cs";

    [Export]
    public int RecruitmentSlots { get; set; } = 1;

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportUnits { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public Dictionary<int, int> AvaiableUnits { get; set; } = new Dictionary<int, int>();

    public Player Controller { get; set; }

    ConstructionManager _constructions { get; set; }

    public List<int> CanPay { get; set; } = new List<int>();

    public List<int> CantPay { get; set; } = new List<int>();

    public override void _Ready()
    {
        var parent = GetParent();
        InitVisionComponent(parent);
        InitAvaiableUnits(parent);
        GetConstructionManager();
    }

    void InitVisionComponent(Node parent){
        var node = parent.GetNodeOrNull<VisionComponent>("VisionComponent");
        if(node == null){
            var scene = (PackedScene)ResourceLoader.Load(VisionComponentPath);
            node = scene.Instantiate<VisionComponent>();
            parent.AddChild(node);
        }
        node.BodyEntered += _on_area_body_Entered;
        node.BodyExited += _on_area_body_Exited;
    }

    void InitAvaiableUnits(Node parent){
        var buildings = parent.GetNodeOrNull<BuildingManager>("BuildingManager");
        if(buildings != null){
            buildings.BuildingFinished += UpdateAvaiableUnits;
            UpdateAvaiableUnits(buildings.Buildings);
        }else{
            var data = (Data)GetTree().GetNodesInGroup("GameData")[0];
            foreach(var u in ExportUnits){
                var id = data.Units.FirstOrDefault(x => x.Name == u.Key);
                if(id != null)
                    AvaiableUnits.Add(id.Index, u.Value);
            }
        }
    }

    void GetConstructionManager(){
        _constructions = GetNodeOrNull<ConstructionManager>("ConstructionManager");
        if(_constructions == null){
            _constructions = new ConstructionManager();
            AddChild(_constructions);
            _constructions.ConstructionSlots = RecruitmentSlots;
        }
    }

    public bool StartConstruction(Unit unit){
            if(Controller.ResManager.PayCost(unit.BuildCost)){
                _constructions.ConstructBuilding(unit);
                return true;
            }else{
                //EmitSignal(nameof(GameAlertEventHandler), this); //game alert should be static
                return false;
            }
    }

    public void UpdateAvaiableUnits(List<Building> buildings){
        foreach(var building in buildings){
            foreach(var unit in building.Units){
                if(!AvaiableUnits.ContainsKey(unit.Key))
                    AvaiableUnits.Add(unit.Key, unit.Value);
            }
            
        }
    }

    public void UpdateCanPayUnits(ResourceManager resourceManager, List<Unit> units){
        CanPay.Clear();
        CantPay.Clear();
        foreach (var building in AvaiableUnits){
            if (resourceManager.CanPayCost(units[building.Key].BuildCost)){
                CanPay.Add(building.Key);
            }else{
                CantPay.Add(building.Key);
            }   
        }
    }

    void _on_area_body_Entered(Node node){
        var rc = node.GetNodeOrNull<RecruitmentComponent>("RecruitmentComponent");
        if (rc != null){
            rc.AddAvaialableUnits(AvaiableUnits.Keys.ToList());
            rc.RecruitmentManager = this;
        }
    } 

    void _on_area_body_Exited(Node node){
        var rc = node.GetNodeOrNull<RecruitmentComponent>("RecruitmentComponent");
        if (rc != null){
            rc.RemoveAvaialableUnits(AvaiableUnits.Keys.ToList());
            rc.RecruitmentManager = this;
        }
    } 
}
