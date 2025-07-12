using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;

public partial class ModelLoader : Node
{
    [Export]
    string ModelPath { get; set; } = "res://Models/";

    [Export]
    string ModelName { get; set; } = "model";

    [Export]
    string MiniModelName { get; set; } = "miniature";

    [Export]
    string ColliderName { get; set; } = "collision";

    [Export]
    bool Log { get; set; } = false;

    private static readonly HashSet<string> validExtensions = new HashSet<string>()
{
    "glb",
    "GLB",
    "gltf"
    // Other possible extensions
};

    GameLogger gameLogger = GameLogger.Instance;

    public Dictionary<string, Node> Models { get; set; } = new Dictionary<string, Node>();

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
            var gltfSceneRootNode = gltfDocumentLoad.GenerateScene(gltfStateLoad);
            //gltfSceneRootNode.Name = Path.GetFileNameWithoutExtension(path);
            AddChild(gltfSceneRootNode);
            Models.Add(gltfSceneRootNode.Name, gltfSceneRootNode);
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
                    LoadFile(path+"/"+fileName);
                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.Print("An error occurred when trying to access the path.");
        }
    }

    public MeshInstance3D GetMeshInstance3D(string name)
    {
        return Models[name].GetNodeOrNull<MeshInstance3D>(ModelName);
    }

    public MeshInstance3D GetMiniMeshInstance3D(string name)
    {
        return Models[name].GetNodeOrNull<MeshInstance3D>(MiniModelName);
    }

    public CollisionShape3D GetCollisionShape3D(string name)
    {
        return Models[name].GetNodeOrNull<CollisionShape3D>(ColliderName);
    }
}
