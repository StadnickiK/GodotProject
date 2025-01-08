using Godot;
//using System.Collections.Generic;
using Godot.Collections;

public partial class MenuPanel : Panel
{
    Button _closeButton = null;

    Label _titleLabel = null;

    Node _valueContainer = null;

    Dictionary<string, int> WorldGenParameters = new Dictionary<string, int>();

    [Export]
    public string Title { get; set; } = "Title";
    // Called when the node enters the scene tree for the first time.

    [Export(PropertyHint.File, "*.tscn")]
    string newScene;

    [Signal]
    public delegate void StartNewGameEventHandler(Dictionary<string,int> WorldGenParameters);

    void GetNodes(){
        _closeButton = GetNode<Button>("Header/XButton");
        _titleLabel = GetNode<Label>("Header/Title");
        _valueContainer = GetNode<Node>("VBoxContainer/ValueContainer");
    }

    public override void _Ready()
    {
        GetNodes();
        _closeButton.Connect("button_up", new Callable(this, nameof(_on_XButton_button_up)));
        var parent = (Game)GetParent().GetParent();
        parent.ConnectToStartNewGame(this);
        StartNewGame += parent._on_StartNewGame;
        //Connect("StartNewGame", new Callable(parent, "_on_StartNewGame"));
    }

    void _on_XButton_button_up(){
        Visible = false;
    }

    void _on_Confirm_button_up(){
        foreach(Node n in _valueContainer.GetChildren()){
            if(n is ValueSlider){
                ValueSlider value = (ValueSlider)n;
                if(WorldGenParameters.ContainsKey(value.ValueName)){
                    WorldGenParameters[value.ValueName] = value.CurrentValue;
                }else{
                    WorldGenParameters.Add(value.ValueName, value.CurrentValue);
                }
            }
        }
        //EmitSignal(nameof(StartNewGameEventHandler), WorldGenParameters);
        var e = EmitSignal("StartNewGame", WorldGenParameters);
    }

}

