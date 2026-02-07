using Godot;
using System;


public class BattlefieldSettings
{
    public Vector3 BattleZone { get; set; } = new Vector3(500,1,500);

    public float DeploymentZoneHeightMultiplier { get; set; } = 0.2f;

    public float DeploymentZoneWidhtMultiplier { get; set; } = 0.9f;

    public float UnitSizeMultiplier { get; set; } = 3f;
}

public partial class BattlefieldSettingsMenu : VBoxContainer
{

    public SpinBox Width { get; set; }

    public SpinBox Height { get; set; }
    public SpinBox VisionRangeBattlefieldSizeMultiplier { get; set; }

    public Label SizeTooltip { get; set; }

    public Vector3 BattleZone { get; set; } = new Vector3(500,1,500);

    public Vector3 OptimalBattleZone { get; set; } = new Vector3(500,1,500);

    public Vector3 DeploymentZoneSize { get; set; } = new Vector3(450,1,100);

    public Vector3 MaxUnitSize { get; set; } = Vector3.Zero;

    [Export]
    public float DeploymentZoneHeightMultiplier { get; set; } = 0.2f;

    [Export]
    public float DeploymentZoneWidhtMultiplier { get; set; } = 0.9f;

    [Export]
    public float UnitSizeMultiplier { get; set; } = 3f;

    public BattlefieldSettings BattlefieldSettings { get; set; }

    string SizeTooltipText;

    float MaxVisionRange = 0;

    public override void _Ready()
    {
        base._Ready();
        Width = GetNode<SpinBox>("Settings/Width");
        Height = GetNode<SpinBox>("Settings/Height");
        VisionRangeBattlefieldSizeMultiplier = GetNode<SpinBox>("Settings/VRmultiplier");
        SizeTooltip = GetNode<Label>("Settings/OptimalSize");
        SizeTooltipText = SizeTooltip.Text;
        UpdateOptimalSizeLabel(OptimalBattleZone);
        BattlefieldSettings = new BattlefieldSettings()
        {
            BattleZone = BattleZone,
            DeploymentZoneHeightMultiplier = DeploymentZoneHeightMultiplier,
            DeploymentZoneWidhtMultiplier = DeploymentZoneWidhtMultiplier,
            UnitSizeMultiplier = UnitSizeMultiplier
        };
    }

    public void _on_width_value_changed(float value)
    {
        var s = BattleZone;
        s.X = value;
        BattlefieldSettings.BattleZone = BattleZone = s;
    }

    public void _on_height_value_changed(float value)
    {
        var s = BattleZone;
        s.Z = value;
        BattlefieldSettings.BattleZone = BattleZone = s;
    }

    public void UpdateMaxUnitSize(Vector3 size)
    {
        var s = MaxUnitSize;
        if (size.X > MaxUnitSize.X)
            s.X = size.X;
        if (size.Y > MaxUnitSize.Y)
            s.Y = size.Y;
        if (size.Z > MaxUnitSize.Z)
            s.Z = size.Z;
        MaxUnitSize = s;
    }

    public void UpdateMaxVisionRange(float range)
    {
        MaxVisionRange = Mathf.Max(MaxVisionRange, range);
        UpdateOptimalBattlefieldSize();
        UpdateOptimalSizeLabel(OptimalBattleZone);;
    }

    void UpdateOptimalSizeLabel(Vector3 size)
    {
        SizeTooltip.Text = SizeTooltipText + size.X +"x"+size.Z;
    }

    void UpdateOptimalBattlefieldSize()
    {
        var max = Mathf.Max(OptimalBattleZone.X, OptimalBattleZone.Y);
        max = Mathf.Max(max, OptimalBattleZone.Z);
        max = Mathf.Max(max, (float)VisionRangeBattlefieldSizeMultiplier.Value * MaxVisionRange);        
        
        if(MaxUnitSize.Z * UnitSizeMultiplier > DeploymentZoneHeightMultiplier * max)
        {
            max = MaxUnitSize.Z * UnitSizeMultiplier * (1f/DeploymentZoneHeightMultiplier);
            DeploymentZoneSize = new Vector3(DeploymentZoneWidhtMultiplier*max, 1, DeploymentZoneHeightMultiplier*max);
        }
        else
        {
            DeploymentZoneSize = new Vector3(DeploymentZoneWidhtMultiplier*max, 1, DeploymentZoneHeightMultiplier*max);
        }
        OptimalBattleZone = new Vector3(Mathf.Round(max), 1, Mathf.Round(max));
    }

    public void _on_MaxUnitSizeChanged(Vector3 size)
    {
        MaxUnitSize = size;
        UpdateOptimalBattlefieldSize();
        UpdateOptimalSizeLabel(OptimalBattleZone);;
    }

}
