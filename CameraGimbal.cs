using Godot;
using System;

public class CameraGimbal : Spatial
{
    bool drag = false;
    [Export]
    public float MovementSpeed { get; set; } = 20;
    [Export] 
    public float RotationSpeed { get; set; } = 0.7f;

    public Vector3 LimitCenter { get; set; } = Vector3.Zero;

    [Export]
    public int Limit { get; set; } = -1;



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
        }
    }

    if(inputEvent is InputEventMouseMotion motion && drag){
        //GlobalRotate(new Vector3(0,1,0), Mathf.Deg2Rad(motion.Relative.x*RotationSpeed));
        RotateY(Mathf.Deg2Rad(motion.Relative.x*RotationSpeed));
    }
}

Vector2 InputKeyToVector2(InputEventKey key){
    Vector2 input_movement_vector = new Vector2();
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
    return input_movement_vector;
}

void KeyboardAction(InputEventKey key){
    Vector2 input_movement_vector = InputKeyToVector2(key);
    
    Vector3 Direction = new Vector3();
    Direction = GlobalTransform.basis.z.Normalized() * input_movement_vector.y * MovementSpeed;
    Direction += GlobalTransform.basis.x.Normalized() * input_movement_vector.x * MovementSpeed;

    if(Limit >-1){
        UpdatePositionWithLimit(Direction);
    }else{
        AddGlobalOrigin(Direction);
    }
}

void UpdatePositionWithLimit(Vector3 Direction){
    Vector3 origin = GlobalTransform.origin + Direction;
    Vector3 dir = origin - LimitCenter;
    int distance = (int)(dir).Length();
    if(distance < Limit){
        AddGlobalOrigin(Direction);
    }else{
        SetGlobalOrigin((dir.Normalized()*(Limit-1)));
    }
}


void AddGlobalOrigin(Vector3 position){
    var temp = GlobalTransform;
    temp.origin += position;
    GlobalTransform = temp;
}

void SetGlobalOrigin(Vector3 position){
    var temp = GlobalTransform;
    temp.origin = position;
    GlobalTransform = temp;
}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
