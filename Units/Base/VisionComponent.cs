using Godot;
using System;

public partial class VisionComponent : Area3D
{


    IVisible VisibleParent;

    CollisionShape3D _shape;

    [Export]
    public int VisionRange { get; set; } = 4;

    [Export]
    public bool GivesVision { get; set; } = true;   

    public void UpdateVisionRange(){
        var s = new CylinderShape3D();
        s.Radius = VisionRange;
        _shape.Shape = s;
    }

    public void UpdateVisionRange(int range){
        VisionRange = range;
        var s = new CylinderShape3D();
        s.Radius = VisionRange;
        _shape.Shape = s;
    }
    public override void _Ready()
    {
        
        var node = GetParent();
        if (node is IVisible v)
            VisibleParent = v;
        _shape = GetChild<CollisionShape3D>(0);
        UpdateVisionRange();
    }

    void _on_body_entered(Node body){
        // to do test in galaxy
        if(body is IVisible visionObject && VisibleParent.Controller != null){
                if(!VisibleParent.Controller.Equals(visionObject.Controller) /*&& visionObject.ReturnVisible() == false*/){
                    visionObject.VisibilityConroller.UpdateVisible(new VisibilityConroller.VisibilityStruct(){Visibility=VisibilityConroller.VisibilityState.Visible, Visible = true},VisibleParent.Controller.PlayerID, true);
                }
        }
    }

    void _on_body_exited(Node body){
        if(body is IVisible visionObject && VisibleParent.Controller != null){
                    if(!VisibleParent.Controller.Equals(visionObject.Controller) && visionObject.ReturnVisible() == true  && VisibleParent.Controller.IsLocal){
                        visionObject.VisibilityConroller.UpdateVisible(new VisibilityConroller.VisibilityStruct(){Visibility=VisibilityConroller.VisibilityState.Explored, Visible = false},VisibleParent.Controller.PlayerID, false);
                    }
                    // else{
                    //     VisibleParent.VisibilityConroller.UpdateVisible(new VisibilityConroller.VisibilityStruct(){Visibility=VisibilityConroller.VisibilityState.Explored, Visible = false},visionObject.Controller.PlayerID, false);
                    // }       
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
