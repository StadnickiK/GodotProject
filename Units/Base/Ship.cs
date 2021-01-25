using Godot;
using System;
using System.Collections.Generic;

public class Ship : RigidBody
{
    [Signal]
    public delegate void SelectUnit(RigidBody unit);

    [Signal]
    public delegate void SelectTarget(RigidBody target);

    [Signal]
    public delegate void LeaveSystem(int id);

    [Signal]
    public delegate void EnterSystem(Ship shipID, StarSystem system, Vector3 aproachVec, PhysicsDirectBodyState state);

    [Signal]
    public delegate void LeavePlanet(Ship ship);

    [Signal]
    public delegate void EnterPlanet(Ship ship, Planet planet);

    [Signal]
    public delegate void EnterCombat(PhysicsBody ship, PhysicsBody enemy, Node parent);

    [Export]
    public int effectiveRange = 10;

    [Export]
    public int ID_Owner { get; set; }

    [Export]
    public bool IsLocal { get; set; } = false;

    public Player ShipOwner { get; set; } = null;

    public StarSystem System { get; set; } = null;

    public Planet _Planet { get; set; } = null;

    public StatManager StatManager { get; set; } = new StatManager();

    public List<BaseStat> Stats { get; set; } = new List<BaseStat>();

    public List<Unit> Units { get; set; } = new List<Unit>();

    public Vector3 PlanetPos { get; set; } = Vector3.Zero;

    public MeshInstance Mesh { get; set; } = null;  

    public BaseStat Power { get; set; } = new BaseStat("Power");

    [Export]
    public int VisionRange { get; set; } = 4;

    Spatial Area = null;

    protected VelocityController _velocityController = new VelocityController();
    public TargetManager<Spatial> targetManager { get; set; } = new TargetManager<Spatial>();

    protected void UpdateLinearVelocity(PhysicsDirectBodyState state){
            state.LinearVelocity = _velocityController.GetAcceleratedVelocity(GlobalTransform.basis.Xform(new Vector3(0, 0, 1)),GlobalTransform.origin,targetManager.currentTarget.Transform.origin);
    }

    protected void ResetVelocity(){
        _velocityController.ResetSpeed();
        LinearVelocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
        Sleeping = true;
    }

    protected Vector3 PosToTargetWithEffectiveRange(Vector3 targetPos){
        Vector3 effectivePos = ((targetPos) - ((targetPos - GlobalTransform.origin).Normalized()*effectiveRange));
        return effectivePos;
    }

    protected Vector3 DirToCurrentTarget(){
        if(targetManager.HasTarget){
            return (targetManager.currentTarget.GlobalTransform.origin-GlobalTransform.origin).Normalized();
        }
        return Vector3.Zero;
    }

    public void MoveToTarget(PhysicsBody target){
        Sleeping = false;
        targetManager.SetTarget(target);
    }

    public void MoveToPos(Vector3 destination){
        Sleeping = false;
        var target = new Spatial();
        target.SetProcess(false);
        var transform = target.Transform;
        transform.origin = destination;
        target.Transform = transform;
        target.Name = "tempTarget";
        targetManager.SetTarget(target);
    }

    public void NextTarget(){
        if(targetManager.currentTarget is StaticBody body){
            if(body.Name == "tempTarget"){
                body.QueueFree();
                targetManager.NextTarget();
                return;
            }
        }
        targetManager.NextTarget();
    }

    void UpdateShipVelocities(PhysicsDirectBodyState state){
        var targetPos = targetManager.currentTarget.Transform.origin;
        float angle = _velocityController.GetAngleToTarget(GlobalTransform, targetPos); 
            if(angle > 0.01f || angle < -0.01f ){
                state.AngularVelocity = _velocityController.GetAngularVelocity(GlobalTransform,targetPos);
                UpdateLinearVelocity(state);
            }else{
                AngularVelocity = Vector3.Zero;
                UpdateLinearVelocity(state);
            }
            if(targetPos != Vector3.Zero){
                Vector3 posDiff = (targetPos.Abs() - GlobalTransform.origin.Abs());
                Vector3 MultyScale = Scale;
                if(posDiff.x < MultyScale.x && posDiff.z < MultyScale.z &&
                posDiff.x > -MultyScale.x && posDiff.z > -MultyScale.z){
                    ResetVelocity();
                }  
            }
    }

    public void UpdateVisionRange(){
        Area.Scale = new Vector3(VisionRange, 0.1f, VisionRange);
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state){
        if(targetManager.HasTarget){
            var targetPos = targetManager.currentTarget.Transform.origin;
            UpdateShipVelocities(state);
            if(System == null){
                if((targetManager.currentTarget is StarSystem) && (targetPos - GlobalTransform.origin).Length()<2){
                    System = (StarSystem)targetManager.currentTarget;
                    EmitSignal(nameof(EnterSystem), this, System, DirToCurrentTarget(), state);
                }
            }
            if(System != null){
                if(Transform.origin.Length()>System.Radius){
                    EmitSignal(nameof(LeaveSystem), this, DirToCurrentTarget(), state);
                    System = null;
                }
            }
            if((targetManager.currentTarget is Planet planet) && (targetPos - GlobalTransform.origin).Length()<2){
                EmitSignal(nameof(EnterPlanet), this, planet);
                ResetVelocity();
                var transform = state.Transform;
                transform.origin = planet.GlobalTransform.origin;
                state.Transform = transform;
                targetManager.ClearTargets();
                
            }
            if(GetParent().Name == "Orbit" && _Planet != null && ((_Planet.Transform.origin - GlobalTransform.origin) - PlanetPos).Length()>2){
                var transform = Transform;
                transform.origin = _Planet.GlobalTransform.origin;
                EmitSignal(nameof(LeavePlanet), this);
                state.Transform = transform;
            }

        }else{
            state.AngularVelocity = Vector3.Zero;
            state.LinearVelocity = Vector3.Zero;
            Sleeping = true;
            state.Sleeping = true;
        }
    }

    void _on_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
      if(inputEvent is InputEventMouseButton eventMouseButton){
        switch((ButtonList)eventMouseButton.ButtonIndex){
          case ButtonList.Left:
            EmitSignal(nameof(SelectUnit), (PhysicsBody)this);
            break;
          case ButtonList.Right:
            EmitSignal(nameof(SelectTarget), (PhysicsBody)this);
            break;
        }
      } 
    }

    void _on_Area_body_entered(Node body){
        if(body is Ship ship){
            if(ship.ID_Owner != ID_Owner && ship.Visible == false && ship.System == System && !ship.IsLocal){
                ship.Visible = true;
            }
        }
        if(body is Planet planet){
            if(planet.Vision == false && planet.PlanetOwner != ShipOwner && planet.System == System){
                planet.Vision = true;
            }
        }
    }

    void _on_Area_body_exited(Node body){
        if(body is Ship ship){
            if(ship.ID_Owner != ID_Owner && ship.Visible == true && ship.System == System && ship.IsLocal){
                ship.Visible = false;
            }
        }
        if(body is Planet planet){
            if(planet.Vision == true && planet.PlanetOwner != ShipOwner && planet.System == System){
                planet.Vision = false;
            }
        }
    }

    void _on_Ship_body_entered(Node node){
        if(node is Ship ship){
            if(ship.System == System){
                EmitSignal(nameof(EnterCombat), (PhysicsBody)this, (PhysicsBody)node, GetParent());
                ResetVelocity();
            }
        }
    }

    protected void _ConnectSignal(){
        World world = GetNode<World>("/root/Game/World");
        world.ConnectToSelectUnit(this);
        WorldCursorControl WCC = GetNode<WorldCursorControl>("/root/Game/World/WorldCursorControl");
        WCC.ConnectToSelectTarget(this);
        Map map = GetNode<Map>("/root/Game/World/Map/");
        map.ConnectToEnterSystem(this);
        map.ConnectToLeaveSystem(this);
        map.ConnectToEnterCombat(this);
        map.ConnectToEnterPlanet(this);
        map.ConnectToLeavePlanet(this);
    }

    void GetNodes(){
        Area = GetNode<Spatial>("Area");
        Mesh = GetNode<MeshInstance>("ship model/Cube");
    }

    public void ConnectToEnterCombat(Node node, string methodName){
         Connect("EnterCombat", node, methodName);
    }

    public void ConnectToEnterSystem(Node node, string methodName){
        Connect("EnterSystem", node, methodName);
    }

    public void ConnectToLeaveSystem(Node node, string methodName){
         Connect("LeaveSystem", node, methodName);
    }

    public void ConnectToEnterPlanet(Node node, string methodName){
        Connect("EnterPlanet", node, methodName);
    }

    public void ConnectToLeavePlanet(Node node, string methodName){
        Connect(nameof(LeavePlanet), node, methodName);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //VisionRange = GetNode<Spatial>("VisionRange");
        //VisionRange.Scale = new Vector3(Range,0,Range);

        _velocityController.Mass = 10;
        _ConnectSignal();
        GetNodes();
        UpdateVisionRange();
        Stats.Add(new BaseStat("Attack", 5));
        Stats.Add(new BaseStat("Defence", 3));
        Stats.Add(new BaseStat("Hit points", 100));
        for(int i = 0;i<5;i++){
            Units.Add(new Unit());
            Power.CurrentValue += new Unit().Stats["HitPoints"].CurrentValue;
        }
    }

    public void UpdatePower(){
        Power.CurrentValue = 0;
        foreach(Unit unit in Units){
            Power.CurrentValue += unit.Stats["HitPoints"].CurrentValue;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
