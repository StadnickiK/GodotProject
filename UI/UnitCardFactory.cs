using Godot;
using System;

public partial class UnitCardFactory : NodeFactoryBase<UnitCard>
{

    public override void _Ready()
    {
        base._Ready();
    }

    public UI UI { get; set; }

    new public UnitCard GetInstance(Node parent)
    {
        var uc = base.GetInstance(parent);
        var b = uc.GetChild(0).GetNode<Button>("Button");
        b.MouseEntered += () => UI._on_BuildingLabelGuiInputEvent(uc.Unit, b.Disabled);
        b.MouseExited += UI._mouseLeftBuildingLabel;
        return uc;
    }

}
