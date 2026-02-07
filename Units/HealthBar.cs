using Godot;
using System;
using System.ComponentModel;

public partial class HealthBar : ProgressBar
{
    [Export]
    public float SeparatorWidht { get; set; } = 16;

    [Export]
    public float SmallStep { get; set; } = 100;

    [Export]
    public float LargeStep { get; set; } = 1000;

    [Export]
    public bool DrawSteps { get; set; } = true;

    [Export]
    public Color FillColor { get; set; } = new Color("4C681F");

    [Export]
    public Color SeparatorColor { get; set; } = new Color("2B3B12");

    [Export]
    public bool ShowSeparators { get; set; } = true;

    StyleBoxFlat FillStyle;

    public override void _Ready()
    {
        base._Ready();
        FillStyle = (StyleBoxFlat)GetThemeStylebox("fill");
        UpdateFillColor(FillColor);
    }


    public void Update(IStat stat, bool visible = true)
    {
        Visible = visible;
        MinValue = stat.MinValue;
        MaxValue = stat.MaxValue;
        Value = stat.CurrentValue;
        QueueRedraw();
    }

    public void UpdateFillColor(Color color)
    {
        FillStyle.BgColor = color;
        AddThemeStyleboxOverride("fill", FillStyle);
    }

    public override void _Draw()
    {
        base._Draw();
        if (ShowSeparators)
        {
            var CurrentValue = Value;
            float width = Size.X;
            var separation = (float)(Size.X / (MaxValue / 100));
            while(width >= separation)
            {
                width -= separation;
                CurrentValue -= 100;
                if(CurrentValue % 1000 == 0)
                {
                    DrawRect(new Rect2(width,0,SeparatorWidht, Size.Y), SeparatorColor, true);
                }
                else
                {
                    DrawRect(new Rect2(width,0,SeparatorWidht, Size.Y/2), SeparatorColor, true);
                }  
            }
        }
    }

}
