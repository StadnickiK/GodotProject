using Godot;
using System;
using System.Collections.Generic;

public class Ship : RigidBody, ISelectMapObject, IMapObjectController, IVision, IResourceManager
{
    [Signal]
    public delegate void SelectUnit(RigidBody unit);

    [Signal]
    public delegate void SelectTarget(RigidBody target);

    [Signal]
    public delegate void EnterCombat(PhysicsBody ship, PhysicsBody enemy, Node parent);

    [Signal]
    public delegate void SignalEnterMapObject(Node node, Vector3 aproachVec, PhysicsDirectBodyState state);

    [Signal]
    public delegate void SignalExitMapObject(Node node, Vector3 aproachVec, PhysicsDirectBodyState state); 

    [Export]
    public int effectiveRange = 10;

    [Export]
    public int ID_Owner { get; set; }

    [Export]
    public bool IsLocal { get; set; } = false;
    public Player Controller { get; set; } = null;

    public IEnterMapObject MapObject { get; set; } = null;

    public StatManager StatManager { get; set; } = new StatManager();

    public ResourceManager ResourcesManager { get; set; } = new ResourceManager();

    public List<BaseStat> Stats { get; set; } = new List<BaseStat>();

    public List<Unit> Units { get; set; } = new List<Unit>();

    public Vector3 PlanetPos { get; set; } = Vector3.Zero;

    public MeshInstance Mesh { get; set; } = null;  

    public BaseStat Power { get; set; } = new BaseStat("Power");

    [Export]
    public int VisionRange { get; set; } = 4;

    public VisionArea _area { get; set; }

    protected VelocityController _velocityController = new VelocityController();
    public TargetManager<Spatial> targetManager { get; set; } = new TargetManager<Spatial>();

    public CmdPanel.CmdPanelOption Task { get; set; } = CmdPanel.CmdPanelOption.None;

    SimpleFireControl _control = null;

    protected void UpdateLinearVelocity(PhysicsDirectBodyState state){
            state.LinearVelocity = _velocityController.GetAcceleratedVelocity(GlobalTransform.basis.Xform(new Vector3(0, 0, 1)),GlobalTransform.origin,targetManager.currentTarget.Transform.origin);
    }

    protected void ResetVelocity(){
        _velocityController.ResetSpeed();
        LinearVelocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
        Sleeping = true;
    }

    protected void ResetVelocity(PhysicsDirectBodyState state){
        _velocityController.ResetSpeed();
        state.LinearVelocity = Vector3.Zero;
        state.AngularVelocity = Vector3.Zero;
        state.Sleeping = true;
    }

    protected Vector3 PosToTargetWithEffectiveRange(Vector3 targetPos){
        Vector3 effectivePos = ((targetPos) - ((targetPos - GlobalTransform.origin).Normalized()*effectiveRange));
        return effectivePos;
    }

    protected Vector3 DirToCurrentTarget(){
        if(targetManager.HasTarget){
            return (targetManager.currentTarget.Transform.origin-Transform.origin).Normalized();
        }
        return Vector3.Zero;
    }

    public void MoveToTarget(Spatial target){
        Sleeping = false;
        Node starSysObj = null;
        if((GetParent() is Orbit orbit)){ 
            starSysObj = orbit.GetParent().GetParent();
        }else{
            starSysObj = GetParent();
        }
        if(target.GetParent() != starSysObj){   // check if target is in the same map object (f.e. star system)
            if(starSysObj.GetParent() is StarSystem system){
                var tempTarget = new Spatial(); // used as point to exit current system
                var tempTrans = tempTarget.Transform;
                tempTrans.origin = (target.Transform.origin-system.Transform.origin).Normalized()*(((float)system.Radius)*1.2f);
                tempTarget.Transform = tempTrans;
                tempTarget.Name = "tempTarget";
                targetManager.SetTarget(tempTarget);    // set temp target to leave star system
                if(target.GetParent().GetParent() is StarSystem targetSystem){
                    targetManager.AddTarget(targetSystem);
                }
                targetManager.AddTarget(target);
                return;
            }else{
                if(target.GetParent().GetParent() is StarSystem targetSystem){
                    targetManager.SetTarget(targetSystem);
                }
                targetManager.AddTarget(target);
                return;
            }
        }
        targetManager.SetTarget(target); // if target is in the same map object (f.e. star system) set as target
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

    void UpdateShipVelocities(PhysicsDirectBodyState state , Vector3 targetPos){
        if(targetManager.HasTarget){
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
                    NextTarget();
                }  
            }
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state){
        if(targetManager.HasTarget){
            Vector3 targetPos = Vector3.Zero;
            if(targetManager.currentTarget is PhysicsBody){
                targetPos = targetManager.currentTarget.GlobalTransform.origin;
            }else{
                targetPos = targetManager.currentTarget.Transform.origin;
            }
            if(targetManager.currentTarget is IEnterMapObject targetObject && (targetPos - GlobalTransform.origin).Length()<2){
                EmitSignal(nameof(SignalEnterMapObject), this, targetObject, DirToCurrentTarget(), state);
                MapObject = targetObject;
                NextTarget();
            }
            if(MapObject != null){
                //if(MapObject is StarSystem)
                    EmitSignal(nameof(SignalExitMapObject), this, MapObject, DirToCurrentTarget(), state);
            }
            /*
            if(System != null){
                if(Transform.origin.Length()>System.Radius){
                    EmitSignal(nameof(LeaveSystem), this, DirToCurrentTarget(), state);
                    System = null;
                }
            }
            ///
            if(GetParent().Name == "Orbit" && _Planet != null && ((_Planet.Transform.origin - GlobalTransform.origin) - PlanetPos).Length()>2){
                var transform = Transform;
                transform.origin = _Planet.GlobalTransform.origin;
                EmitSignal(nameof(LeavePlanet), this);
                state.Transform = transform;
            }
            //*/
            if(targetManager.currentTarget is Ship ship && (targetPos - GlobalTransform.origin).Length()<2){
                EmitSignal(nameof(EnterCombat), (PhysicsBody)this, (PhysicsBody)ship, GetParent());
                targetManager.ClearTargets();
                ResetVelocity(state);
            }
            UpdateShipVelocities(state, targetPos);
            _control.Start();
        }else{
            state.AngularVelocity = Vector3.Zero;
            state.LinearVelocity = Vector3.Zero;
            Sleeping = true;
            state.Sleeping = true;
            _control.Stop();
        }
    }

    public void SelectMapObject(){
        EmitSignal(nameof(SelectUnit), (PhysicsBody)this);
    }

    void _on_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
      if(inputEvent is InputEventMouseButton eventMouseButton){
        switch((ButtonList)eventMouseButton.ButtonIndex){
          case ButtonList.Left:
            SelectMapObject();
            break;
          case ButtonList.Right:
            EmitSignal(nameof(SelectTarget), (PhysicsBody)this);
            break;
        }
      } 
    }

    public void ChangeVision(){
        if(Visible){
            Visible = false;
        }else{
            Visible = true;
        }
    }

    void _on_Area_body_entered(Node body){
        // to do test in galaxy
        if(body is IVisible visionObject){
            if(visionObject.Controller != Controller && visionObject.IsVisible() == false && body.GetParent() == GetParent()){
                visionObject.ChangeVision();
            }
            if(Controller != null)
                if(body is Planet planet){ 
                        if(Controller.IsLocal && planet.Orbit.GetChildren().Count > 0){
                            planet.IcoOrbit.Visible = true;
                            if(planet.Orbit.HasEnemy(this))
                                planet.IcoOrbit.SetRed();
                        }
                }
        }
        /*
        if(body is Ship ship){
            if(ship.ID_Owner != ID_Owner && ship.Visible == false && ship.GetParent() == GetParent() && !ship.IsLocal){
                ship.Visible = true;
            }
        }
        if(body is Planet planet){
            if(planet.Vision == false && planet.Controller != Controller && planet.GetParent() == GetParent() ){
                planet.Vision = true;
            }
        }*/
    }

    void _on_Area_body_exited(Node body){
        if(body is IVisible visionObject){
            if(visionObject.Controller != Controller && visionObject.IsVisible() == true && body.GetParent() == GetParent() && Controller.IsLocal){
                visionObject.ChangeVision();
            }
            if(Controller != null)
                if(body is Planet planet){
                    if(Controller.IsLocal && visionObject.Controller != Controller){
                        planet.IcoOrbit.Visible = false;
                    }
                }
        }
        /*
        if(body is Ship ship){
            if(ship.ID_Owner != ID_Owner && ship.Visible == true && ship.GetParent() == GetParent()  && !ship.IsLocal){
                ship.Visible = false;
            }
        }
        if(body is Planet planet){
            if(planet.Vision == true && planet.Controller != Controller && planet.System == System){
                planet.Vision = false;
            }
        }
        */
    }

    void _on_Ship_body_entered(Node node){
        if(node is Ship ship){
            if(ship.MapObject == MapObject){
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
        map.ConnectToEnterCombat(this);
        map.ConnectToEnterMapObject(this);
        map.ConnectToExitMapObject(this);
    }

    void GetNodes(){
        _area = GetNode<VisionArea>("Area");
        Mesh = GetNode<MeshInstance>("ship model/Cube");
        _control = GetNode<SimpleFireControl>("FireControl");
    }

    public void ConnectToEnterCombat(Node node, string methodName){
         Connect("EnterCombat", node, methodName);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //VisionRange = GetNode<Spatial>("VisionRange");
        //VisionRange.Scale = new Vector3(Range,0,Range);

        _velocityController.Mass = 10;
        _ConnectSignal();
        GetNodes();
        _area.UpdateVisionRange(VisionRange);
        //Stats.Add(new BaseStat("Attack", 5));
        //Stats.Add(new BaseStat("Defence", 3));
        //Stats.Add(new BaseStat("Hit points", 100));
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
