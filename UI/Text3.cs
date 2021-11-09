using Godot;
using System;

public class Text3 : Sprite3D
{
    [Export]
    public string Text { get; set; } = "Text";

    [Export]
    public Vector3 TextScale { get; set; } = new Vector3(1,1,1);

    Label label = null;

    bool TextChanged = false;

    Viewport vp = null;
    public override void _Ready()
    {
        label = (Label)GetNode("Viewport/Label");
        vp = (Viewport)GetNode("Viewport");
        UpdateText(Text);
    }

    public void UpdateText(string text){
        Text = text;
        label.Text = Text;
        vp.Size = label.RectSize;
        Texture = vp.GetTexture();
        TextChanged = true; 
    }

    public override void _Process(float delta){
        if(TextChanged){
            vp.Size = label.RectSize;
            TextChanged = false;
        }
    }
}
