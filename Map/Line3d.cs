using Godot;
using System;
using System.Collections.Generic;

public partial class Line3d : Node3D
{

	[Export]
	public int MaxSegments { get; set; } = 30;

	[Export]
	public Mesh EndPointMesh { get; set; } = new SphereMesh();

	Material Material;

	public override void _Ready()
	{
		Material = EndPointMesh.SurfaceGetMaterial(0);
	}

	public MeshInstance3D DrawPoint(Vector3 position)
	{
		var mesh = new MeshInstance3D();
		mesh.Mesh = EndPointMesh;
		mesh.Position = position;
		AddChild(mesh);
		return mesh;
	}

	public MeshInstance3D DrawPoint(Vector3 position, float radius)
	{
		var mesh = new MeshInstance3D();
		var sphere = new SphereMesh();
		sphere.Radius = radius;
		sphere.Height = 2 * radius;
		sphere.Material = EndPointMesh.SurfaceGetMaterial(0);
		mesh.Mesh = sphere;
		mesh.Position = position;
		AddChild(mesh);
		return mesh;
	}

	public MeshInstance3D DrawLine(Material material, Vector3 position, Vector3 endpoint)
	{
		var mesh = new MeshInstance3D();
		var immediate = new ImmediateMesh();
		mesh.Mesh = immediate;
		mesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
		immediate.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
		immediate.SurfaceAddVertex(position);
		immediate.SurfaceAddVertex(endpoint);
		immediate.SurfaceEnd();
		AddChild(mesh);
		return mesh;
	}
	/// <summary>
	/// Width in 2x meters
	/// </summary>
	/// <param name="material"></param>
	/// <param name="position"></param>
	/// <param name="endpoint"></param>
	/// <param name="width"></param>
	/// <returns></returns> <summary>
	/// 
	/// </summary>
	/// <param name="material"></param>
	/// <param name="position"></param>
	/// <param name="endpoint"></param>
	/// <param name="width"></param>
	/// <returns></returns>
	public MeshInstance3D DrawBox(Material material, Vector3 position, Vector3 endpoint, float width = 1f)
	{
		var mesh = new MeshInstance3D();
		var rect = new BoxMesh();
		var dir = endpoint - position;
		var length = dir.Length();
		rect.Size = new Vector3(width,width, length);
		rect.Material = material;
		mesh.Mesh = rect;
		mesh.Position = position + dir/2;
		AddChild(mesh);
		mesh.LookAt(endpoint + dir, Vector3.Up);
		return mesh;
	}

	public void RedrawLine(MeshInstance3D mesh, Vector3 position, Vector3 endpoint)
	{
		var line = (ImmediateMesh)mesh.Mesh;
		line.ClearSurfaces();
		line.SurfaceBegin(Mesh.PrimitiveType.Lines, Material);
		line.SurfaceAddVertex(position);
		line.SurfaceAddVertex(endpoint);
		line.SurfaceEnd();
	}	

	public HashSet<MeshInstance3D> LineWithPoint(Vector3 position, Vector3 endpoint)
	{
		var line = DrawLine(Material, position, endpoint);
		
		var point = DrawPoint(endpoint);
        
		var set = new HashSet<MeshInstance3D>
        { 
			line, 
			point
        };
        return set;
	}

	public HashSet<MeshInstance3D> DrawPredictionLine(Vector3 startPos, Vector3 velocity)
	{
		var delta = (float)GetProcessDeltaTime();
		var set = new HashSet<MeshInstance3D>();
		Vector3 endpoint = new Vector3();
		for (int i = 0; i < MaxSegments; i++)
		{
			endpoint = startPos + (velocity * delta);
			velocity *= float.Clamp(1 - delta, 0, 1);
			var line = DrawLine(Material, startPos, endpoint);
			AddChild(line);
			startPos = endpoint;
		}
		var point = DrawPoint(endpoint);
		AddChild(point);
		set.Add(point);
		return set;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
