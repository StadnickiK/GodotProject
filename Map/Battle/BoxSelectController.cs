using Godot;
using System;
using System.Collections.Generic;

public partial class BoxSelectController : Node2D
{
    bool Selecting { get; set; } = false;
    private Vector2 DragStart { get; set; }
    private Rect2 SelectBox { get; set; }

    public List<IUnit3D> LocalUnits { get; set; }

    public UnitDragHandler unitDragHandler { get; set; }

    Camera3D camera3D;

    ColorProviderSingleton Colors;

    public override void _Ready()
    {
        SetProcessInput(false);
        Colors = ColorProviderSingleton.Instance;
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if(!unitDragHandler.Drag)
            if (inputEvent is InputEventMouseButton mouseButton &&
                mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.Pressed)
                {
                    Selecting = true;
                    //SelectUnits();
                    DragStart = mouseButton.Position;
                }
                else
                {
                    Selecting = false;
                    if (DragStart.IsEqualApprox(mouseButton.Position)) SelectBox = new Rect2(mouseButton.Position, Vector2.Zero);
                    SelectUnits();
                    QueueRedraw();
                }
            }
            else if (Selecting && inputEvent is InputEventMouseMotion mouseMotion)
            {
                DrawBox(mouseMotion.Position);
                SelectUnits();
                QueueRedraw();
            }
    }

    void DrawBox(Vector2 Position)
    {
        var xMin = Mathf.Min(DragStart.X, Position.X);
        var yMin = Mathf.Min(DragStart.Y, Position.Y);
        SelectBox = new Rect2(
            xMin,
            yMin,
            Mathf.Max(DragStart.X, Position.X) - xMin,
            Mathf.Max(DragStart.Y, Position.Y) - yMin
        );
    }

    public override void _Draw()
    {
        if (Selecting)
        {
            DrawRect(SelectBox, Colors.FillColor);
            DrawRect(SelectBox, Colors.OutlineColor, false, 2f);
        }
    }

    private void SelectUnits()
    {
        foreach (var unit in LocalUnits)
        {
            camera3D = GetViewport().GetCamera3D();
            var point = camera3D.UnprojectPosition(unit.GlobalPosition);
            if (SelectBox.HasPoint(point))
            {
                if (unit.InputController != null)
                    unit?.InputController.AddUnitInvoke();
            }
            else
            {
                if (unit.InputController != null)
                    unit.InputController.DeselectUnitInvoke();
                
                
            }
        }
    }
}
