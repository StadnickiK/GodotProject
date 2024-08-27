using Godot;
using System;

public partial class Text3 : Sprite3D
{
    [Export]
    public string Text { get; set; } = "Text";

    [Export]
    public Vector3 TextScale { get; set; } = new Vector3(1,1,1);

    Label label = null;

    bool TextChanged = false;

    SubViewport vp = null;
    public override void _Ready()
    {
        label = (Label)GetNode("SubViewport/Label");
        vp = (SubViewport)GetNode("SubViewport");
        UpdateText(Text);
    }

    public void UpdateText(string text){
        Text = text;
        label.Text = Text;
        vp.Size = (Vector2I)label.Size;
        Texture = vp.GetTexture();
        TextChanged = true; 
    }

    public override void _Process(double delta){
        if(TextChanged){
            vp.Size = (Vector2I)label.Size;
            TextChanged = false;
        }
    }
}
