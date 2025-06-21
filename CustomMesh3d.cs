using Godot;
using System;
using System.Collections.Generic;

public partial class CustomMesh3d : MeshInstance3D
{
    ArrayMesh arrayMesh;

    [Export]
    public Material Material { get; set; }

    [Export]
    public int Subdivisions { get; set; } = 16;

    [Export]
    public float Radius { get; set; } = 1.0f;

    public void RegenerateMesh()
    {
        if (arrayMesh == null)
        {
            arrayMesh = new ArrayMesh();
            Mesh = arrayMesh;
        }

        arrayMesh.ClearSurfaces();

        // Replace this with your actual sphere generation function
        Godot.Collections.Array surfaceArray = GenerateSphere(Subdivisions);

        arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
        int surfaceIndex = arrayMesh.GetSurfaceCount() - 1;

        if (Material != null)
            arrayMesh.SurfaceSetMaterial(surfaceIndex, Material);
    }

public Godot.Collections.Array GeneratePlane(int subDivs, int index, Vector3 direction, Vector3 center)
    {
        var surfaceArray = new Godot.Collections.Array();
        surfaceArray.Resize((int)Mesh.ArrayType.Max);

        var verts = new List<Vector3>();
        var uvs = new List<Vector2>(); // Not used currently, but declared for completeness
        var normals = new List<Vector3>();
        var indices = new List<int>();

        direction = direction.Normalized();
        Vector3 binormal = new Vector3(direction.Z, direction.X, direction.Y) / subDivs;
        Vector3 tangent = binormal.Rotated(direction, Mathf.Pi / 2.0f);
        Vector3 offset = -subDivs * (binormal + tangent) / 2.0f + center;

        for (int x = 0; x < subDivs; x++)
        {
            for (int y = 0; y < subDivs; y++)
            {
                Vector3 vertexOffset = (binormal * x + tangent * y + offset);
                int indexOffset = 4 * (x * subDivs + y) + index;

                verts.AddRange(
                [
                    vertexOffset,
                    vertexOffset + tangent,
                    vertexOffset + binormal + tangent,
                    vertexOffset + binormal
                ]);

                normals.AddRange(
                [
                    direction, direction, direction, direction
                ]);

                indices.AddRange(
                [
                    indexOffset, indexOffset + 1, indexOffset + 2,
                    indexOffset, indexOffset + 2, indexOffset + 3
                ]);
            }
        }

        surfaceArray[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
        // surfaceArray[(int)Mesh.ArrayType.TexUv] = uvs;
        surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

        return surfaceArray;
    }

    public Godot.Collections.Array GenerateCube(int subDivs)
{
    var surfaceArray = new Godot.Collections.Array();
    surfaceArray.Resize((int)Mesh.ArrayType.Max);

        var verts = new List<Vector3>();
        var uvs = new List<Vector2>(); // Not used currently, but declared for completeness
        var normals = new List<Vector3>();
        var indices = new List<int>();

    var directions = new List<Vector3>()
    {
        Vector3.Up,
        Vector3.Down,
        Vector3.Left,
        Vector3.Right,
        Vector3.Forward,
        Vector3.Back
    };

    for (int i = 0; i < directions.Count; i++)
    {
        int index = 4 * i * subDivs * subDivs;
        var plane = GeneratePlane(subDivs, index, directions[i], directions[i] / 2.0f);

        verts.AddRange(plane[(int)Mesh.ArrayType.Vertex].AsVector3Array());
        normals.AddRange(plane[(int)Mesh.ArrayType.Normal].AsVector3Array());
        indices.AddRange(plane[(int)Mesh.ArrayType.Index].AsInt32Array());
    }

    surfaceArray[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
    // surfaceArray[(int)Mesh.ArrayType.TexUv] = uvs;
    surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
    surfaceArray[(int)Mesh.ArrayType.Index] = indices.ToArray();

    return surfaceArray;
}

public Godot.Collections.Array GenerateSphere(int subDivs)
{
    var surfaceArray = GenerateCube(subDivs);

    var verts = surfaceArray[(int)Mesh.ArrayType.Vertex].AsVector3Array();
    var normals = surfaceArray[(int)Mesh.ArrayType.Normal].AsVector3Array();

    for (int i = 0; i < verts.Length; i++)
    {
        Vector3 vertex = verts[i];
        verts[i] = vertex.Normalized() / 2.0f * Radius;
        normals[i] = vertex.Normalized();
    }

    surfaceArray[(int)Mesh.ArrayType.Vertex] = verts;
    surfaceArray[(int)Mesh.ArrayType.Normal] = normals;

    return surfaceArray;
}

    public override void _Ready()
    {
        RegenerateMesh();
    }

}
