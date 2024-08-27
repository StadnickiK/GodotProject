using Godot;
using System.Collections.Generic;

public partial class ConstructionManager : Node
{

    public bool ConstructionListChanged { get; set; } = false;

    private List<IBuilding> _constructionList = new List<IBuilding>();
    public List<IBuilding> ConstructionList
    {
        get { return _constructionList; }
    }

    public bool ConstructionChanged { get; set; } = false;

    public int ConstructionSlots { get; set; } = 1;

    public void ConstructBuilding(IBuilding building, int count = 1){
        if(building != null){
            for(int i = 0; i<count;i++)
                ConstructionList.Add(building);
            // ConstructionListChanged = true;
        }
    }

    public override void _Ready()
    {
        
    }

    /// <summary>
    ///     Update all IBuilding objects in the construction List and return a List of finished ones.
    /// </summary>
    /// <returns>List<IBuilding></returns>
    public List<IBuilding> UpdateConstruction(){
        var count = ConstructionList.Count;
        var List = new List<IBuilding>();
        if(count > 0){
            if(count > ConstructionSlots){
                for(int i = 0; i < ConstructionSlots; i++){
                    UpdateConstruction(i);
                    var building = ConstructionList[i];
                    if(building.CurrentTime >= building.BuildTime){
                        ConstructionList.RemoveAt(i);
                        List.Add(building);
                    }
                }
            }else{
                for(int i = 0; i < count; i++){
                    UpdateConstruction(i);
                    var building = ConstructionList[i];
                    if(building.CurrentTime >= building.BuildTime){
                        ConstructionList.RemoveAt(i);
                        List.Add(building);
                    }
                }
            }
        }
        return List;
    }

    void UpdateConstruction(int id){
        ConstructionList[id].CurrentTime++;
    }

    public List<IBuilding> CurrentConstruction(){
        var count = ConstructionList.Count;
        List<IBuilding> List = new List<IBuilding>();
        if(count > 0){
            if(count > ConstructionSlots){
                for(int i = 0; i < ConstructionSlots; i++){
                    List.Add(ConstructionList[i]);
                }
            }else{
                for(int i = 0; i < count; i++){
                    List.Add(ConstructionList[i]);
                }
            }
        }
        return List;
    }

    public bool HasConstruct(){
        if(_constructionList.Count > 0) return true;    
        return false;
    }
}
