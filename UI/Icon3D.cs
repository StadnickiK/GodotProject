using Godot;
using System;

public class Icon3D : StaticBody
{
    
    MeshInstance _mesh = null;

    void GetNodes(){
        _mesh = GetNode<MeshInstance>("MeshInstance");
    }

    public override void _Ready()
    {
        GetNodes();
        SetYellow();
    }

    public void ChangeColor(Color color){
        var material = _mesh.GetSurfaceMaterial(0);
        if(material is SpatialMaterial spatialMaterial){
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
