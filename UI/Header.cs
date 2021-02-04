using Godot;
using System;

public class Header : HBoxContainer
{

    private Button _button;
    public Button HButton
    {
        get { return _button; }
    }
    
    

    Label _TitleLabel = null;

    [Export]
    public string Title { get; set; } = "Title";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNodes();
        _TitleLabel.Text = Title;
    }

    public void SetTitle(string title){
        _TitleLabel.Text = title;
    }

    void GetNodes(){
        _button = GetNode<Button>("XButton");
        _TitleLabel = GetNode<Label>("Title");
    }

    public void ConnectToButtonUp(Node node, string methodName){
        _button.Connect("button_up", node, methodName);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
