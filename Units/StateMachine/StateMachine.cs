using Godot;
using System;
using System.Collections.Generic;


public partial class StateMachine : Node
{
    private State _currentState;
    public Ship Body { get; set; }

    public StateMachine(){}
    public StateMachine(Ship ship, State state){
        Body = ship;
        _currentState = state;
        _currentState.Enter(ship);
    }

    public State CurrentState { set {_currentState = value;} get { return _currentState;}}

    public void Enter(State state) { state.Enter(Body); CurrentState = state; }

    public override void _Process(double delta)
    {
        Update(delta);
    }


    public void Update(double delta)
    {
        // Process the current state and possibly get a new state.
        State newState = _currentState.ProcessState(delta);

        // If a state transition is triggered...
        if (newState.GetType() != _currentState.GetType())
        {
            // Exit the current state.
            _currentState.Exit();

            // Switch to the new state.
            _currentState = newState;

            // Enter the new state.
            _currentState.Enter(Body);
        }
    }
}

public partial class State
{
    protected Ship Body;

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public virtual void Enter(Ship body)
    {
        Body = body;
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public virtual void Exit() { }

    /// <summary>
    /// Processes the state logic.
    /// Return a new state (or the same state) to indicate if a state change is required.
    /// </summary>
    public virtual State ProcessState(double delta){ return this; }
}