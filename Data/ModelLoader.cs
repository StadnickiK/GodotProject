using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;


public class ModelData
{
    // rework for variants
    public Node Model { get; set; }
    public MeshInstance3D MeshInstance3D { get; set; }

    public MeshInstance3D MiniMeshInstance3D { get; set; }

    public CollisionShape3D CollisionShape3D { get; set; }

    public Dictionary<string, Node3D> DestroyedMeshes { get; set; } = new Dictionary<string, Node3D>();

    public List<MeshInstance3D> FireEmitters { get; set; } = new List<MeshInstance3D>();  

    public List<MeshInstance3D> MiniFireEmitters { get; set; } = new List<MeshInstance3D>();  

    public List<TurretData> Turrets { get; set; } = new List<TurretData>();    
}

public class TurretData
{
    public int TurretIndex { get; set; }

    public MeshInstance3D Body { get; set; }

    public List<MeshInstance3D> Barrels { get; set; } = new List<MeshInstance3D>();
}

public partial class ModelLoader : Node3D
{
    [Export]
    string ModelPath { get; set; } = "res://Models/";

    [Export]
    string ModelName { get; set; } = "model";

    [Export]
    string MiniModelName { get; set; } = "mini";

    [Export]
    string MiniEmissionName { get; set; } = "miniem";

    [Export]
    string ColliderName { get; set; } = "collision-colonly";

    [Export]
    string EmissionName { get; set; } = "emission";

    [Export]
    string TurretName { get; set; } = "turret";

    [Export]
    string BarrelName { get; set; } = "barrel";

    [Export]
    bool Log { get; set; } = false;

    [Export]
    public char SplitChar { get; set; } = '#';

    private static readonly HashSet<string> validExtensions = new HashSet<string>()
    {
        "glb",
        "GLB",
        "gltf"
        // Other possible extensions
    };

    GameLogger gameLogger = GameLogger.Instance;

    public Dictionary<string, ModelData> Models { get; set; } = new Dictionary<string, ModelData>();

    void LogMessage(string message)
    {
        if (Log) gameLogger.LogInfo(message);
    }

    public override void _Ready()
    {
        LoadDirContents(ModelPath);
    }

    void LoadFile(string path)
    {
        // Load an existing glTF scene.
        // GLTFState is used by GLTFDocument to store the loaded scene's state.
        // GLTFDocument is the class that handles actually loading glTF data into a Godot node tree,
        // which means it supports glTF features such as lights and cameras.
        var gltfDocumentLoad = new GltfDocument();
        var gltfStateLoad = new GltfState();
        var error = gltfDocumentLoad.AppendFromFile(path, gltfStateLoad);
        if (error == Error.Ok)
        {
            var gltfSceneRootNode = (Node3D)gltfDocumentLoad.GenerateScene(gltfStateLoad);
            //gltfSceneRootNode.Name = Path.GetFileNameWithoutExtension(path);
            var collMesh = gltfSceneRootNode.GetChildren().FirstOrDefault(x => x.Name.ToString().Contains("-col"));
            if (collMesh != null)
                ((MeshInstance3D)collMesh).CreateConvexCollision(true, true);
            gltfSceneRootNode.Hide();
            AddChild(gltfSceneRootNode);
            var model = new ModelData()
            {
                MeshInstance3D = GetMeshInstance3D(gltfSceneRootNode),
                MiniMeshInstance3D = GetMiniMeshInstance3D(gltfSceneRootNode),
                CollisionShape3D = GetCollisionShape3D(gltfSceneRootNode),
                MiniFireEmitters = GetMiniFireEmmiters(gltfSceneRootNode),
                FireEmitters = GetFireEmmiters(gltfSceneRootNode),
                Turrets = GetTurrets(gltfSceneRootNode)
            };
            Models.Add(gltfSceneRootNode.Name, model);
        }
        else
        {
            GD.PrintErr($"Couldn't load glTF scene (error code: {error}).");
            LogMessage($"Couldn't load glTF scene (error code: {error}).");
        }
    }


    void LoadDirContents(string path)
    {
        using var dir = DirAccess.Open(path);
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (validExtensions.Contains(fileName.GetExtension()))
                    LoadFile(path + "/" + fileName);
                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.Print("An error occurred when trying to access the path " + path);
        }
    }


    MeshInstance3D GetMeshInstance3D(Node node)
    {
        return node.GetNodeOrNull<MeshInstance3D>(ModelName);
    }

    public MeshInstance3D GetMeshInstance3D(string name)
    {
        return Models[name].MeshInstance3D;
    }

    MeshInstance3D GetMiniMeshInstance3D(Node node)
    {
        return node.GetNodeOrNull<MeshInstance3D>(MiniModelName);
    }

    public MeshInstance3D GetMiniMeshInstance3D(string name)
    {
        return Models[name].MiniMeshInstance3D;
    }

    CollisionShape3D GetCollisionShape3D(Node node)
    {
        // var children = Models[name].GetNodeOrNull(ColliderName);
        // var c2 = Models[name].GetNodeOrNull(ColliderName).GetChildren();
        // var coll = Models[name].GetNodeOrNull(ColliderName);
        // GD.Print("Colltype " + coll.GetType());
        // var collType = coll.GetType();
        var collisionShape3D = node.GetNodeOrNull(ColliderName)?.GetChildren()[0].GetNode<CollisionShape3D>("CollisionShape3D");
        if(collisionShape3D != null) collisionShape3D.Disabled = true;
        return collisionShape3D;
    }

    public CollisionShape3D GetCollisionShape3D(string name)
    {
        // var children = Models[name].GetNodeOrNull(ColliderName);
        // var c2 = Models[name].GetNodeOrNull(ColliderName).GetChildren();
        // var coll = Models[name].GetNodeOrNull(ColliderName);
        // GD.Print("Colltype " + coll.GetType());
        // var collType = coll.GetType();
        return Models[name].CollisionShape3D;
    }

    public List<MeshInstance3D> GetFireEmmiters(Node node)
    {
        var list = new List<MeshInstance3D>();
        foreach (var item in node.GetChildren())
            if (item.Name.ToString().Contains(EmissionName))
                list.Add((MeshInstance3D)item);
        return list;
    }

    public List<MeshInstance3D> GetFireEmmiters(string name)
    {
        return Models[name].FireEmitters;
    }

    public List<MeshInstance3D> GetMiniFireEmmiters(Node node)
    {
        var list = new List<MeshInstance3D>();
        foreach (var item in node.GetChildren())
            if (item.Name.ToString().Contains(MiniEmissionName))
                list.Add((MeshInstance3D)item);
        return list;
    }

    public List<MeshInstance3D> GetMiniFireEmmiters(string name)
    {
        return Models[name].MiniFireEmitters;
    }

    public List<TurretData> GetTurrets(Node node)
    {
        var list = new List<TurretData>();
        var children = node.GetChildren();
        foreach (var item in children)
        {
            if (item.Name.ToString().Contains(TurretName))
            {
                var turretData = new TurretData();
                var name = item.Name.ToString();
                turretData.TurretIndex = Int32.Parse(name.Split(SplitChar)[1]);
                turretData.Body = (MeshInstance3D)item;
                var itemChildren = item.GetChildren();
                foreach (var child in itemChildren)
                {
                    if (child.Name.ToString().Contains(BarrelName) && child is MeshInstance3D mesh)
                        turretData.Barrels.Add(mesh);
                }
                list.Add(turretData);
            }
        }
        return list;
    }

    public List<TurretData> GetTurrets(string name)
    {
        return Models[name].Turrets;
    }
}
