using Godot;
using System;
using System.Collections.Generic;



public interface IBattleArmy : IMapObjectController
{
    public int MyProperty { get; set; }

    public UnitController Units { get; set; }
}


public partial class BattleArmyContainerHandler : VBoxContainer
{
    [Export]
    public string ItemScenePath { get; set; } = "res://UI/Unit_Card.tscn";

    protected PackedScene scene;

    public UnitCardFactory UnitCardFactory { get; set; }

    public List<ArmyContainer> ArmyContainers { get; set; } = new List<ArmyContainer>();

    UI _Ui;

    public override void _Ready()
    {
        scene = (PackedScene)ResourceLoader.Load(ItemScenePath);
        GetNodes();
    }
    void GetNodes()
    {
        foreach (var item in GetChildren())
        {
            if (item is ArmyContainer armyContainer)
                ArmyContainers.Add(armyContainer);
        }
    }

    public void UpdateArmyContainers(List<Ship> nodes)
    {
        if (nodes.Count > ArmyContainers.Count)
            AddCards(ArmyContainers.Count - nodes.Count);
        for (int i = 0; i < ArmyContainers.Count; i++)
        {
            if (i < nodes.Count)
            {
                ArmyContainers[i].UpdateArmyContainer(nodes[i]);
            }
            else
            {
                ArmyContainers[i].Hide();
            }

        }

    }

    void AddCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var card = scene.Instantiate<ArmyContainer>();
            card.UnitCardFactory = UnitCardFactory;
            card.ConnectUnitCards(_Ui);
            ArmyContainers.Add(card);
        }
    }

    public void ConnectUnitCards(UI ui)
    {
        _Ui = ui;
        foreach (var item in ArmyContainers)
        {
            item.ConnectUnitCards(ui);
        }
    }
}
