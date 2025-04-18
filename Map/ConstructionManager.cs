using Godot;
using System.Collections.Generic;

public partial class ConstructionManager : Node
{
    public delegate void ConstrucctionFinishedEventHandler(List<IConstruct> building);

    public bool ConstructionListChanged { get; set; } = false;

    private List<IConstruct> _constructionList = new List<IConstruct>();
    public List<IConstruct> ConstructionList
    {
        get { return _constructionList; }
    }
    public bool HasConstruct{ 
        get { return _constructionList.Count > 0; }
    }

    public bool ConstructionChanged { get; set; } = false;

    [Export]
    public int ConstructionSlots { get; set; } = 1;

    public void ConstructBuilding(IConstruct building, int count = 1){
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
    ///     Update all IConstruct objects in the construction List and return a List of finished ones.
    /// </summary>
    /// <returns>List<IConstruct></returns>
    public List<IConstruct> UpdateConstruction(){
        var count = ConstructionList.Count;
        var List = new List<IConstruct>();
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

    public List<IConstruct> CurrentConstruction(){
        var count = ConstructionList.Count;
        List<IConstruct> List = new List<IConstruct>();
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
}
