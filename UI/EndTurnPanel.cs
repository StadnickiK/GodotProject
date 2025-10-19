using Godot;
using System;

public interface IEndTurnListener
{
    void _on_EndTurn(int TurnNumber);
}

public class EndTurnEmitter
{
    private EndTurnEmitter() { }

    private static EndTurnEmitter _instance;
    public static EndTurnEmitter Instance
    {
        get
        {
            if (_instance == null) _instance = new EndTurnEmitter();
            return _instance;
        }
        private set { _instance = value; }
    }

    public int TurnNumber { get; private set; } = 1;

    public delegate void EndTurnEventHandler(int TurnNumber);

    public event EndTurnEventHandler EndTurn;

    public void EndTurnInvoke()
    {
        EndTurn?.Invoke(TurnNumber);
        TurnNumber++;
    }
}


public partial class EndTurnPanel : VBoxContainer
{
    public Button EndTurnButton { get; set; }

    public Label TurnCount { get; set; }

    public EndTurnEmitter EndTurnEmitter { get; private set; } = EndTurnEmitter.Instance;

    public int TurnNumber { get { return EndTurnEmitter.TurnNumber; } }

    public override void _Ready()
    {
        EndTurnButton = GetNode<Button>("EndTurn");
        TurnCount = GetNode<Label>("HBoxContainer/TurnCount");
        EndTurnButton.ButtonUp += _on_EndTurn;
    }

    void _on_EndTurn()
    {
        EndTurnEmitter.EndTurnInvoke();
        GetTree().CallGroup("EndTurnListener", "_on_EndTurn");
        TurnCount.Text = TurnNumber.ToString();
    }
}

