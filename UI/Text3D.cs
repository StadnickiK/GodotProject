using Godot;
using System;

public class Text3D : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public string Text { get; set; } = "Text";

    [Export]
    public Vector3 TextScale { get; set; } = new Vector3(1,1,1);

    Label label = null;

    MeshInstance mesh = null;
    Viewport vp = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LoadChildren();
        UpdateText(Text);
    }

    void LoadChildren(){
        label = (Label)GetNode("Viewport/Label");
        vp = (Viewport)GetNode("Viewport");
        mesh = (MeshInstance)GetNode("Mesh");
    }

    public void UpdateText(String Text){
        label.Text = Text;
        vp.Size = label.RectSize;
        label.RectPosition = Vector2.Zero;
        mesh.Scale = TextScale;
        vp.RenderTargetUpdateMode = Viewport.UpdateMode.Once;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
