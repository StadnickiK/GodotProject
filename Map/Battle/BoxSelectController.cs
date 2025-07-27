using Godot;
using System;

public partial class BoxSelectController : Node2D
{
    bool Selecting { get; set; } = false;
    private Vector2 DragStart { get; set; }
    private Rect2 SelectBox { get; set; }

    [Export]
    public Color OutlineColor { get; set; } = new Color("00ff00");

    [Export]
    public Color FillColor { get; set; } = new Color("00ff0066");

    public override void _Ready()
    {
        SetProcessInput(false);
    }


    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left)
        {
            if (mouseButton.Pressed)
            {
                Selecting = true;
                DragStart = mouseButton.Position;
            }
            else
            {
                Selecting = false;
                QueueRedraw();
            }
        }
        else if (Selecting && inputEvent is InputEventMouseMotion mouseMotion)
        {
            var xMin = Mathf.Min(DragStart.X, mouseMotion.Position.X);
            var yMin = Mathf.Min(DragStart.Y, mouseMotion.Position.Y);
            SelectBox = new Rect2(
                xMin,
                yMin,
                Mathf.Max(DragStart.X, mouseMotion.Position.X) - xMin,
                Mathf.Max(DragStart.Y, mouseMotion.Position.Y) - yMin
            );
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        if (Selecting)
        {
            DrawRect(SelectBox, FillColor);
            DrawRect(SelectBox, OutlineColor, false, 2f);
        }
    }


}
