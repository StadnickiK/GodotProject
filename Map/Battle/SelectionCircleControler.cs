using Godot;
using System;

public partial class SelectionCircleControler : MeshInstance3D
{

    private Color _color = new Color("00ff00");

    private float circleWidth = 0.03f;

    Vector2 size = new Vector2(3, 3);

    [Export]
    public Color CurrentColor
    {
        get { return _color; }
        set { _color = value; UpdateColor(value); }
    }

    [Export]
    public string ColorParameterName { get; set; } = "BaseColor";

    [Export]
    public float CircleWidth { get => circleWidth; set { circleWidth = value; UpdateWidth(value); } }

    [Export]
    public string CricleWidthParameterName { get; set; } = "CricleWidth";

    /// <summary>
    /// Selection effect is size from model AaBb.Size multiplied by this value
    /// </summary> <summary>
    /// 
    /// </summary>
    /// <value></value>
    [Export]
    public float SizeMultiplier { get; set; } = 1.3f;


    public Vector2 Size { get => size; set { size = value; UpdateSize(value); } }

    ShaderMaterial shaderMaterial;

    PlaneMesh planeMesh;

    public override void _Ready()
    {
        planeMesh = (PlaneMesh)Mesh;
        shaderMaterial = planeMesh.Material as ShaderMaterial;
    }

    void UpdateColor(Color value)
    {
        shaderMaterial.SetShaderParameter(ColorParameterName, value);
    }

    void UpdateWidth(float value)
    {
        shaderMaterial.SetShaderParameter(CricleWidthParameterName, value);
    }

    void UpdateSize(Vector2 vector)
    {
        vector *= SizeMultiplier;
        planeMesh.Size = vector;
    }
}
