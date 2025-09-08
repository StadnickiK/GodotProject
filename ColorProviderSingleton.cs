using Godot;
using System;
using System.IO;

public partial class ColorProviderSingleton
{
    // Map object select colors
    [ExportGroup("Map object select colors")]
    [Export]
    private string colorsFilePath = "settings/colors.json";

    [Export]
    public Color PlayerColor { get; set; } = new Color("00ff00");

    [Export]
    public Color EnemyColor { get; set; } = new Color("ff0000");

    [Export]
    public Color AllyColor { get; set; } = new Color("0000ff");

    [Export]
    public Color NeutralColor { get; set; } = new Color("808080");

    // BoxSelect Colors
    [ExportGroup("BoxSelect Colors")]
    [Export]
    public Color OutlineColor { get; set; } = new Color("00ff00");

    [Export]
    public Color FillColor { get; set; } = new Color("00ff0066");

    private ColorProviderSingleton()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(colorsFilePath));
        File.WriteAllText(colorsFilePath, string.Empty); // This ensures the file is clean
    }

    public static ColorProviderSingleton Instance { get; private set; } = new ColorProviderSingleton();

}
