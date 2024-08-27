using Godot;
using System;

public partial class Icon3D : Area3D
{
    
    MeshInstance3D _mesh = null;

    void GetNodes(){
        _mesh = GetNode<MeshInstance3D>("MeshInstance3D");
    }

    public override void _Ready()
    {
        GetNodes();
        SetYellow();
        // SetProcess(false);
        SetPhysicsProcess(false);
    }

    public void ChangeColor(Color color){
        var material = _mesh.GetSurfaceOverrideMaterial(0);
        if(material is StandardMaterial3D spatialMaterial){
            spatialMaterial.AlbedoColor = color;
        }
    }

    public void SetRed(){
        ChangeColor(new Color(1,0,0));
    }

    public void SetGreen(){
        ChangeColor(new Color(0,1,0));
    }

    public void SetYellow(){
        ChangeColor(new Color(1,1,0));
    }

}
