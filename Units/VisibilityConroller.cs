using Godot;
using System;
using System.Collections.Generic;

public partial class VisibilityConroller : Node
{

    public Dictionary<int, VisibilityStruct> PlayerVisibility { get; set; } = new Dictionary<int, VisibilityStruct>();

    public enum VisibilityState
    {
        Unexplored,
        Hidden,
        Explored,
        Visible
    }

    public struct VisibilityStruct
    {
        public VisibilityState Visibility { get; set; }
        public bool Visible { get; set; }
    }

    public IVisible VisibleParent { get; set; }
    
    [Export]
    public bool DefaultVisible { get; set; } = false;

    public override void _Ready(){
            var parent = GetParent();
            if (parent is IVisible iv)
                VisibleParent = iv;  
    }

    public void UpdateVisibility(VisibilityStruct visibility, int playerID){
            if(PlayerVisibility.ContainsKey(playerID)){
                PlayerVisibility[playerID] = visibility;
            }else{
                PlayerVisibility.Add(playerID, visibility);
            }
    }

    public void _on_VisibilityChanged(VisibilityStruct visibility, int objID, int playerID){
        if(VisibleParent.ReturnIndex() == objID)
            if(PlayerVisibility.ContainsKey(playerID)){
                PlayerVisibility[playerID] = visibility;
            }else{
                PlayerVisibility.Add(playerID, visibility);
            }
    }

    public void CopyVisibility(VisibilityConroller visibilityConroller)
    {
        PlayerVisibility = new Dictionary<int, VisibilityStruct>(visibilityConroller.PlayerVisibility);
        VisibleParent = visibilityConroller.VisibleParent;
    }

    public void UpdateVisible(VisibilityStruct visibilityStruct, int playerID, bool visible)
    {
        UpdateVisibility(visibilityStruct, playerID);
        VisibleParent.SetVisibility(visible);
    }

    public VisibilityStruct GetVisibility(int playerId)
    {
        return PlayerVisibility.ContainsKey(playerId) ? PlayerVisibility[playerId] : new VisibilityStruct() {Visibility = VisibilityState.Unexplored, Visible = DefaultVisible};
    }
}
