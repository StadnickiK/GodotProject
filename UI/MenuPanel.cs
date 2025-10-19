using Godot;
//using System.Collections.Generic;
using Godot.Collections;

public partial class MenuPanel : Panel
{
    Button _closeButton = null;

    Label _titleLabel = null;

    Node _valueContainer = null;

    Node ResourceContainer;

    Godot.Collections.Dictionary<string, int> WorldGenParameters = new Godot.Collections.Dictionary<string, int>();

    public Dictionary<int, ValueSlider> ResourceValueSliders { get; private set; } = new Dictionary<int, ValueSlider>();

    [Export]
    public string Title { get; set; } = "Title";
    // Called when the node enters the scene tree for the first time.

    [Export(PropertyHint.File, "*.tscn")]
    string ValueSliderPath = "res://UI/ValueSlider.tscn";

    [Export]
    public int MaxResource { get; set; } = 5000;

    [Signal]
    public delegate void StartNewGameEventHandler(Godot.Collections.Dictionary<string, int> WorldGenParameters);

    PackedScene ValueSliderScene;

    void GetNodes()
    {
        _closeButton = GetNode<Button>("Scroll/VBoxContainer/Header/XButton");
        _titleLabel = GetNode<Label>("Scroll/VBoxContainer/Header/Title");
        _valueContainer = GetNode<Node>("Scroll/VBoxContainer/VBoxContainer/ValueContainer");
        ResourceContainer = GetNode<Node>("Scroll/VBoxContainer/VBoxContainer/ScrollContainer/Resources");
    }

    public override void _Ready()
    {
        GetNodes();
        _closeButton.Connect("button_up", new Callable(this, nameof(_on_XButton_button_up)));
        ValueSliderScene = (PackedScene)ResourceLoader.Load(ValueSliderPath);
        //Connect("StartNewGame", new Callable(parent, "_on_StartNewGame"));
    }

    void _on_XButton_button_up()
    {
        Visible = false;
    }

    void _on_confirm_button_up()
    {
        foreach (Node n in _valueContainer.GetChildren())
        {
            if (n is ValueSlider)
            {
                ValueSlider value = (ValueSlider)n;
                if (WorldGenParameters.ContainsKey(value.ValueName))
                {
                    WorldGenParameters[value.ValueName] = value.CurrentValue;
                }
                else
                {
                    WorldGenParameters.Add(value.ValueName, value.CurrentValue);
                }
            }
        }
        //EmitSignal(nameof(StartNewGameEventHandler), WorldGenParameters);
        var e = EmitSignal(SignalName.StartNewGame, WorldGenParameters);
    }

    public void LoadGameResources(System.Collections.Generic.List<Resource> resources)
    {
        foreach (var resource in resources)
        {
            var slider = ValueSliderScene.Instantiate<ValueSlider>();
            ResourceContainer.AddChild(slider);
            slider.RemoveChild(slider.NameLabel);
            ResourceContainer.RemoveChild(slider);
            ResourceContainer.AddChild(slider.NameLabel);
            ResourceContainer.AddChild(slider);
            slider.SetMaxValue(MaxResource);
            slider.SetValueName(resource.Name);
            ResourceValueSliders.Add(resource.Index, slider);
        }
    }

}

