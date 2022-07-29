using Godot;
using System;

public class WorldCursorControl : Spatial
{

    Select select = null;

    public int LocalPlayerID { get; set; }
    public Camera camera = null;

    [Signal]
    public delegate void Deselect();

    public void ConnectToSelectUnit(Node node){
        node.Connect("SelectUnit", this, nameof(_SelectUnit));
    }

    public void ConnectToSelectTarget(Node node){
        node.Connect("SelectTarget", this, nameof(_SelectTarget));
    }

    void GetNodes(){
        select = GetNode<Select>("Select");
    }

    public override void _Ready()
    {
        GetNodes();
        /*
        foreach(Node n in GetTree().GetNodesInGroup("Selectable")){
            n.Connect("SelectUnit", this, nameof(_SelectUnit));
        }
        foreach(Node n in GetTree().GetNodesInGroup("Targetable")){
            n.Connect("SelectTarget", this, nameof(_SelectTarget));
        }
        //GetNode<Ship>("/root/World/Ship").Connect("SelectUnit", this, nameof(_SelectUnit)); 
        //*/
    }

    public void _SelectUnit(CollisionObject unit){
        select.AddSelectedUnit(unit);
    }

    public void _SelectTarget(CollisionObject target){;
        select.AddTarget(target);
    }

    public void SetTask(CollisionObject target, CmdPanel.CmdPanelOption task){;
        select.AddTarget(target, task);
    }

    Vector3 GetMouseWorldPosition(){
        var ray_length = 1000;
        var mousePos = GetViewport().GetMousePosition();
        var from = camera.ProjectRayOrigin(mousePos);
        var to = from + camera.ProjectRayNormal(mousePos) * ray_length;
        var space_state = GetWorld().DirectSpaceState;
        var state = space_state.IntersectRay(from, to);
        Vector3 p = Vector3.Zero;
        if(state.Contains("position")){
            p = (Vector3)state["position"];
        }
        return p;
    }

    void _on_Ground_input_event(Node camera, InputEvent inputEvent,Vector3 click_position,Vector3 click_normal, int shape_idx){
        if(inputEvent is InputEventMouseButton button){
            if(HasSelected()){ // mouse
                if((ButtonList)button.ButtonIndex == ButtonList.Right){   // right click
                    select.MoveToPosition(GetMouseWorldPosition());
                }
                if((ButtonList)button.ButtonIndex == ButtonList.Left && select != null){    // left click
                    select.ClearSelection();
                    EmitSignal(nameof(Deselect));
                }
            }
        }
    }

     public override void _Input(InputEvent inputEvent){
        if(HasSelected()){
            if(inputEvent is InputEventMouseButton button){ // mouse
                if((ButtonList)button.ButtonIndex == ButtonList.Left && select != null){    // left click
                    //select.ClearSelection();
                    //EmitSignal(nameof(Deselect));
                }
            }
        }
    }

    public bool HasSelected(){
        return select.HasSelected();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
