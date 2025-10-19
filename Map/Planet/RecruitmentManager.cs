using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface RecruitmentFinishedListener
{
    public void RecruitmentFinished(RecruitmentManager recruitmentManager, List<IConstruct> constructs);
}

public partial class RecruitmentManager : Node, IVisionComponentListener, IChangeControllerListener, IEndTurnListener
{
    [Export]
    public string VisionComponentPath { get; set; } = "res://Units/Base/VisionComponent.tscn";

    [Export]
    public int RecruitmentSlots { get; set; } = 1;

    [Export]
    public Godot.Collections.Dictionary<string, int> ExportUnits { get; set; } = new Godot.Collections.Dictionary<string, int>();

    public HashSet<Unit> AvaiableUnits { get; set; } = new HashSet<Unit>();

    public Player Controller { get; set; }

    ConstructionManager _constructions { get; set; }

    public List<int> CanPay { get; set; } = new List<int>();

    public List<int> CantPay { get; set; } = new List<int>();

    public delegate void RecruitmentFinishedEventHandler(RecruitmentManager recruitmentManager, List<IConstruct> constructs);

    public event RecruitmentFinishedEventHandler RecruitmentFinished;

    public override void _Ready()
    {
        var parent = GetParent();
        InitAvaiableUnits(parent);
        GetConstructionManager();
        GetController(parent);
        EndTurnEmitter.Instance.EndTurn += _on_EndTurn;
    }

    void GetController(Node parent)
    {
        if (parent is IMapObjectController controller)
            Controller = controller.Controller;
    }

    void InitAvaiableUnits(Node parent)
    {
        var buildings = parent.GetNodeOrNull<BuildingManager>("BuildingManager");
        if (buildings != null)
        {
            buildings.BuildingFinished += UpdateAvaiableUnits;
            UpdateAvaiableUnits(buildings.Buildings);
        }
        else
        {
            var data = (Data)GetTree().GetNodesInGroup("GameData")[0];
            foreach (var u in ExportUnits)
            {
                var unit = data.Units.FirstOrDefault(x => x.Name == u.Key);
                if (unit != null)
                    AvaiableUnits.Add(unit);
            }
        }
    }

    void GetConstructionManager()
    {
        _constructions = GetNodeOrNull<ConstructionManager>("ConstructionManager");
        if (_constructions == null)
        {
            _constructions = new ConstructionManager();
            AddChild(_constructions);
            _constructions.ConstructionSlots = RecruitmentSlots;
        }
    }

    public bool StartConstruction(Unit unit)
    {
        if (Controller.ResManager.PayCost(unit.BuildCost))
        {
            _constructions.ConstructBuilding(unit);
            return true;
        }
        else
        {
            //EmitSignal(nameof(GameAlertEventHandler), this); //game alert should be static
            return false;
        }
    }

    public bool StartConstruction(IConstruct unit)
    {
        if (Controller.ResManager.PayCost(unit.BuildCost))
        {
            _constructions.ConstructBuilding(unit);
            return true;
        }
        else
        {
            //EmitSignal(nameof(GameAlertEventHandler), this); //game alert should be static
            return false;
        }
    }

    public void StopConstruction(int Position)
    {
        var c = _constructions.StopConstruction(Position);
        Controller.ResManager.AddResource(c.BuildCost);
        c.QueueFree();
    }

    public void UpdateAvaiableUnits(List<Building> buildings)
    {
        foreach (var building in buildings)
        {
            foreach (var unit in building.Units)
            {
                if (!AvaiableUnits.Contains(unit))
                    AvaiableUnits.Add(unit);
            }

        }
    }

    public void UpdateCanPay(ResourceManager resourceManager)
    {
        CanPay.Clear();
        CantPay.Clear();
        foreach (var item in AvaiableUnits)
        {
            if (resourceManager.CanPayCost(item.BuildCost))
            {
                CanPay.Add(item.Index);
            }
            else
            {
                CantPay.Add(item.Index);
            }
        }
    }

    public void _on_area_body_Entered(Node node)
    {
        var rc = node.GetNodeOrNull<RecruitmentComponent>("RecruitmentComponent");
        if (rc != null)
        {
            rc.SetAvaialableUnits(this, AvaiableUnits.ToList());
        }
    }

    public void _on_area_body_Exited(Node node)
    {
        var rc = node.GetNodeOrNull<RecruitmentComponent>("RecruitmentComponent");
        if (rc != null)
        {
            rc.RemoveAvaialableUnits(this);
        }
    }

    public void ChangeContoller(Player player)
    {
        Controller = player;
    }

    public void _on_EndTurn(int TurnNumber)
    {
        var constructs = _constructions.UpdateConstruction();
        if(constructs.Count > 0)
            RecruitmentFinished?.Invoke(this, constructs);
    }
}
