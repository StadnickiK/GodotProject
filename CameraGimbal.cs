using Godot;
using System;

public class CameraGimbal : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    bool drag = false;
    [Export]
    public float MovementSpeed { get; set; } = (float)Math.PI/2;
    [Export] 
    public float RotationSpeed { get; set; } = 0.7f;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

public override void _Input(InputEvent inputEvent){
    if(inputEvent is InputEventKey key){
        KeyboardAction(key);
    }
    if(inputEvent is InputEventMouseButton button){
        if((ButtonList)button.ButtonIndex == ButtonList.Right){
            if(drag != true){
                drag = true;
            }else{
                drag = false;
            }
            //MouseCameraControl(inputEvent);
        }
    }

    if(inputEvent is InputEventMouseMotion motion && drag){
        //GlobalRotate(new Vector3(0,1,0), Mathf.Deg2Rad(motion.Relative.x*RotationSpeed));
        RotateY(Mathf.Deg2Rad(motion.Relative.x*RotationSpeed));
    }
}

void KeyboardAction(InputEventKey key){
    Vector2 input_movement_vector = new Vector2();
    Vector3 Direction = new Vector3();

    switch(key.Scancode){
        case (int)KeyList.W:
        case (int)KeyList.Up:
        input_movement_vector.y -= 1;
        break;
        case (int)KeyList.S:
        case (int)KeyList.Down:
        input_movement_vector.y += 1;
        break;
        case (int)KeyList.A:
        case (int)KeyList.Left:
        input_movement_vector.x -= 1;
        break;
        case (int)KeyList.D:
        case (int)KeyList.Right:
        input_movement_vector.x += 1;
        break;
    }
    Direction = GlobalTransform.basis.z.Normalized() * input_movement_vector.y;
    Direction += GlobalTransform.basis.x.Normalized() * input_movement_vector.x*2;
    AddGlobalOrigin(Direction);
}

void AddGlobalOrigin(Vector3 position){
    var temp = GlobalTransform;
    temp.origin += position;
    GlobalTransform = temp;
}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
