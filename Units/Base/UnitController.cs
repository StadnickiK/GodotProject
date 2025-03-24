using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class UnitController : Node
{
    public List<Unit> UnitsList { get; } = new List<Unit>();

    public UpkeepComponent UpkeepComponent { get; set; }

    public delegate void ChangeUpkeepEventHandler(Dictionary<int, int> upkeep);

    public event ChangeUpkeepEventHandler AddUpkeep;

    public event ChangeUpkeepEventHandler RemoveUpkeep;

	public override void _Ready()
	{
		UpkeepComponent = GetNodeOrNull<UpkeepComponent>("UpkeepComponent");
	}

    public void AddUnit(Unit unit){
        AddChild(unit);
        UnitsList.Add(unit);
        UpkeepComponent?.UpdateUpkeep(unit);
        AddUpkeep?.Invoke(unit.Upkeep);
    }

    public void RemoveUnit(int unitID){
        UpkeepComponent?.RemoveUpkeep(UnitsList[unitID]);
        RemoveUpkeep?.Invoke(UnitsList[unitID].Upkeep);
        RemoveChild(UnitsList[unitID]);
        UnitsList.RemoveAt(unitID);
    
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
