using Godot;
using System.Collections.Generic;

public partial class VisionComponent : Area3D, IUpdateStat
{


    IVisible VisibleParent;

    CollisionShape3D _shape;
    private float visionRange = 4;

    [Export]
    public float VisionRange { get => visionRange; set { visionRange = value; UpdateVisionRange(); } }
    [Export]
    public bool GivesVision { get; set; } = true;

    public delegate void TargetSpottedEventHandler(ITargetable targetable);

    public event TargetSpottedEventHandler TargetSpotted;

    public event TargetSpottedEventHandler TargetLost;

    public HashSet<string> StatNames { get; set; } = new HashSet<string>() { "Vision Range" };

    public void UpdateVisionRange()
    {
        var s = new CylinderShape3D();
        s.Radius = VisionRange;
        _shape.Shape = s;
    }

    public void UpdateVisionRange(float range)
    {
        VisionRange = range;
        var s = new SphereShape3D();
        s.Radius = VisionRange;
        _shape.Shape = s;
    }
    public override void _Ready()
    {
        var node = GetParent();
        if (node is IVisible v)
            VisibleParent = v;
        ConnectStatListeners(node);
        _shape = GetNode<CollisionShape3D>("CollisionShape3D");
        UpdateVisionRange();
    }

    public void ConnectStatListeners(Node parent)
    {
        foreach (var child in parent.GetChildren())
            if (child is ITargetSpottedListener listener)
            {
                TargetSpotted += listener.OnTargetSpotted;
                TargetLost += listener.OnTargetLost;
            }
    }

    void _on_body_entered(Node body)
    {
        // to do test in galaxy
        if (body is IVisible visionObject && VisibleParent.Controller != null)
        {
            if (!VisibleParent.Controller.Equals(visionObject.Controller) /*&& visionObject.ReturnVisible() == false*/)
            {
                visionObject.VisibilityConroller.UpdateVisible(new VisibilityConroller.VisibilityStruct() { Visibility = VisibilityConroller.VisibilityState.Visible, Visible = true }, VisibleParent.Controller.PlayerID, true);
            }
        }
        if (body is ITargetable targetable)
            TargetSpotted?.Invoke(targetable);
    }

    void _on_body_exited(Node body)
    {
        if (body is IVisible visionObject && VisibleParent.Controller != null)
        {
            if (!VisibleParent.Controller.Equals(visionObject.Controller) && visionObject.ReturnVisible() == true && VisibleParent.Controller.IsLocal)
            {
                visionObject.VisibilityConroller.UpdateVisible(new VisibilityConroller.VisibilityStruct() { Visibility = VisibilityConroller.VisibilityState.Explored, Visible = false }, VisibleParent.Controller.PlayerID, false);
            }
            if (body is ITargetable targetable)
                TargetLost?.Invoke(targetable);
            // else{
            //     VisibleParent.VisibilityConroller.UpdateVisible(new VisibilityConroller.VisibilityStruct(){Visibility=VisibilityConroller.VisibilityState.Explored, Visible = false},visionObject.Controller.PlayerID, false);
            // }       
        }
    }
    
    public void UpdateStat(float value, string name = "")
    {
        switch (name)
        {
            case "Vision Range":
                UpdateVisionRange(value);
                break;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
