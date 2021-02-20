using Godot;
using System;

public class Text3 : Sprite3D
{
    [Export]
    public string Text { get; set; } = "Text";

    [Export]
    public Vector3 TextScale { get; set; } = new Vector3(1,1,1);

    Label label = null;

    Viewport vp = null;
    public override void _Ready()
    {
        label = (Label)GetNode("Viewport/Label");
        vp = (Viewport)GetNode("Viewport");
        label.Text = Text;
        label.RectSize = Vector2.Zero;
        vp.Size = label.RectSize;
        Texture = vp.GetTexture();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
