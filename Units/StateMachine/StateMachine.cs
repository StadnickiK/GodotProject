using Godot;
using System;
using System.Collections.Generic;


public partial class StateMachine : GenericStateMachine<IMovable>
{
    
}

public partial class StateMachine3D : GenericStateMachine<IMovable3D>
{
    
}

// public partial class MovableState : State
// {
//     //new protected IMovable Body;

//     /// <summary>
//     /// Called when the state is entered.
//     /// </summary>
//     // public virtual void Enter(IMovable body)
//     // {
//     //     Body = body;
//     // }
// }