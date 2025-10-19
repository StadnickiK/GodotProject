using Godot;
using System.Collections.Generic;


public interface IVisionComponentListener
{
    void _on_area_body_Entered(Node node);

    void _on_area_body_Exited(Node node);
}

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
    public Player Controller { get; set; }

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
        var parent = GetParent();
        if (parent is IVisible v)
            VisibleParent = v;
        InitListeners(parent);
        ConnectStatListeners(parent);
        _shape = GetNode<CollisionShape3D>("CollisionShape3D");
        UpdateVisionRange();
    }

    void InitListeners(Node parent)
    {
        foreach (var item in parent.GetChildren())
            if (item is IVisionComponentListener listener)
            {
                BodyEntered += listener._on_area_body_Entered;
                BodyExited += listener._on_area_body_Exited;
            }
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

    public void UpdateStat(IStat stat)
    {
        switch (stat.Name)
        {
            case "Vision Range":
                UpdateVisionRange(stat.CurrentValue);
                break;
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
