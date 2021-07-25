using Godot;
using System.Collections.Generic;

public class ConstructionManager : Node
{

    public bool ConstructionListChanged { get; set; } = false;

    // TargetManager<IBuilding> ConstructionList = new TargetManager<IBuilding>();

    private List<IBuilding> _constructionList = new List<IBuilding>();
    public List<IBuilding> ConstructionList
    {
        get { return _constructionList; }
    }
    

    public IBuilding CurrentConstruction
    {
        get { return ConstructionList[0]; }
    }

    public bool ConstructionChanged { get; set; } = false;

    public int ConstructionSlots { get; set; } = 1;

    public void ConstructBuilding(IBuilding building){
        if(building != null){
            ConstructionList.Add(building);
            // ConstructionListChanged = true;
        }
    }

    public override void _Ready()
    {
        
    }

    public List<IBuilding> UpdateConstruction(){
        var count = ConstructionList.Count;
        var list = new List<IBuilding>();
        if(count > 0){
            if(count > ConstructionSlots){
                for(int i = 0; i < ConstructionSlots; i++){
                    UpdateConstruction(i);
                    var building = ConstructionList[i];
                    if(building.CurrentTime >= building.BuildTime)
                        list.Add(building);
                }
            }else{
                for(int i = 0; i < count; i++){
                    UpdateConstruction(i);
                    var building = ConstructionList[i];
                    if(building.CurrentTime >= building.BuildTime)
                        list.Add(building);
                }
            }
        }
        return list;
        // if(CurrentConstruction != null){
        //     CurrentConstruction.CurrentTime++;
        //     if(CurrentConstruction.CurrentTime >= CurrentConstruction.BuildTime){
        //         // Buildings.Add(CurrentConstruction);
        //         ConstructionList.NextTarget();
        //         ConstructionListChanged = true;
        //     }
        //     ConstructionChanged = true;
        // }
    }

    void UpdateConstruction(int id){
        ConstructionList[id].CurrentTime++;
    }
}
