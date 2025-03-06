using Godot;
using System;
using System.Collections.Generic;

public partial class VisibilityConroller : Node
{

    public Dictionary<int, VisibilityStruct> PlayerVisibility { get; private set; } = new Dictionary<int, VisibilityStruct>();

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

    public void UpdateVisible(VisibilityStruct visibilityStruct,int playerID, bool visible){
        UpdateVisibility(visibilityStruct, playerID);
        VisibleParent.SetVisibility(visible);
    }

    public VisibilityStruct GetVisibility(int playerId)
    {
        return PlayerVisibility.ContainsKey(playerId) ? PlayerVisibility[playerId] : new VisibilityStruct() {Visibility = VisibilityState.Unexplored, Visible = DefaultVisible};
    }

        void _on_Area_body_entered(Node body){
        // to do test in galaxy
        if(body is IVisible visionObject && body.GetIndex() == VisibleParent.ReturnIndex()){
            if(visionObject.Controller != VisibleParent.Controller && body.GetParent() == GetParent() /*&& visionObject.ReturnVisible() == false*/){
                visionObject.ChangeVision();
            }
        }
    }

    void _on_Area_body_exited(Node body){
        if(body is IVisible visionObject){
            if(VisibleParent.Controller != null){
                if(body.GetParent() == GetParent()){
                    if(visionObject.Controller != VisibleParent.Controller && visionObject.ReturnVisible() == true  && VisibleParent.Controller.IsLocal){
                        UpdateVisible(new VisibilityStruct(){Visibility=VisibilityState.Hidden, Visible= DefaultVisible},visionObject.Controller.PlayerID, DefaultVisible);
                    }else{
                        UpdateVisible(new VisibilityStruct(){Visibility=VisibilityState.Explored, Visible = DefaultVisible}, visionObject.Controller.PlayerID, DefaultVisible);
                    }
                }
            }
            
        }
    }
}
