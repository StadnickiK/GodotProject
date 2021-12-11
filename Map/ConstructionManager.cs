using Godot;
using System.Collections.Generic;

public class ConstructionManager : Node
{

    public bool ConstructionListChanged { get; set; } = false;

    private List<IBuilding> _constructionList = new List<IBuilding>();
    public List<IBuilding> ConstructionList
    {
        get { return _constructionList; }
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
                    if(building.CurrentTime >= building.BuildTime){
                        ConstructionList.RemoveAt(i);
                        list.Add(building);
                    }
                }
            }else{
                for(int i = 0; i < count; i++){
                    UpdateConstruction(i);
                    var building = ConstructionList[i];
                    if(building.CurrentTime >= building.BuildTime){
                        ConstructionList.RemoveAt(i);
                        list.Add(building);
                    }
                }
            }
        }
        return list;
    }

    void UpdateConstruction(int id){
        ConstructionList[id].CurrentTime++;
    }

    public List<IBuilding> CurrentConstruction(){
        var count = ConstructionList.Count;
        List<IBuilding> list = null;
        if(count > 0){
            list = new List<IBuilding>();
            if(count > ConstructionSlots){
                for(int i = 0; i < ConstructionSlots; i++){
                    list.Add(ConstructionList[i]);
                }
            }else{
                for(int i = 0; i < count; i++){
                    list.Add(ConstructionList[i]);
                }
            }
        }
        return list;
    }
}
