using Godot;
using System;

public partial class CustomBatttleMenu : Control
{
    public PlayerContainerControler PlayerContainerControler { get; set; }

    public Button PlayButton { get; set; }

    public Button CloseButton { get; set; }

    public Header Header { get; set; }

    public BuildingInterface BuildingInterface { get; set; }

    public BattlefieldSettingsMenu MapSettings { get; set; }

    public override void _Ready()
    {
        PlayerContainerControler = GetNode<PlayerContainerControler>("VBoxContainer/MarginContainer/PlayerContainerControler");
        PlayButton = GetNode<Button>("VBoxContainer/Play Controls/Play");
        Header = GetNode<Header>("VBoxContainer/Header");
        BuildingInterface = GetNode<BuildingInterface>("BuildingInterface");
        PlayerContainerControler.InitializeBuildingInterface(BuildingInterface);
        MapSettings = GetNode<BattlefieldSettingsMenu>("VBoxContainer/MarginContainer/PlayerContainerControler/MapSettings");
        CloseButton = Header.HButton;
        CloseButton.ButtonUp += OnCloseButtonUp;
        PlayerContainerControler.UnitSizeChanged += MapSettings._on_MaxUnitSizeChanged;
        PlayerContainerControler.MaxVisionRangeChanged += MapSettings.UpdateMaxVisionRange;
    }

    public  void Initialize(Data data)
    {
        PlayerContainerControler.Initialize(data);
    }


    void OnCloseButtonUp()
    {
        Hide();
    }
}
