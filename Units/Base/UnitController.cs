using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class UnitController : Node
{
    public List<Unit> UnitsList { get; } = new List<Unit>();

    public Dictionary<int, int> Upkeep { get; set; }

    public UpkeepComponent UpkeepComponent { get; set; }

	public override void _Ready()
	{
		UpkeepComponent = GetNodeOrNull<UpkeepComponent>("UpkeepComponent");
	}

    public void AddUnit(Unit unit){
        AddChild(unit);
        UnitsList.Add(unit);
        UpkeepComponent?.UpdateUpkeep(unit);
    }

    public void RemoveUnit(int unitID){
        UpkeepComponent?.RemoveUpkeep(UnitsList[unitID]);
        UnitsList.RemoveAt(unitID);
        //GetChildren()[unitID].QueueFree();
    }

    public void TranferUnit(UnitController unitController, int unitID){
        unitController.AddUnit(UnitsList[unitID]);
        RemoveUnit(unitID);
    }

    public void TranferUnit(UnitController unitController, List<int> unitIDs){
        var units  = UnitsList.Where(x => unitIDs.Contains(x.GetIndex())).ToArray();
        for (int i = 0; i < unitIDs.Count ; i++)
        {
            TranferUnit(unitController, unitIDs[i]);
        }
    }
}
