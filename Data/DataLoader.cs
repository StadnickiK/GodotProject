using Godot;
using System;
using System.Collections.Generic;

public class DataLoader : Node
{
    [Export]
    public string Path { get; set; } = null;

    public override void _Ready()
    {
        if(Path != null && Path != "")
            GetScenes(Path);
    }

    public Directory Dir { get; set; } = new Directory();

    bool Open(string dirPath){
        
        if(Dir.Open(dirPath) == Error.Ok){
            return true;
        }
        return false;
    }

    public List<PackedScene> GetScenes(string dirPath){
        if(Open(dirPath)){
            List<PackedScene> scenes = new List<PackedScene>();
            
            Dir.ListDirBegin();

            var fileName = Dir.GetNext();
            while(fileName != "" && fileName != null){
                if(!Dir.CurrentIsDir()){
                    AddChild(
                        ResourceLoader.Load<PackedScene>(dirPath+fileName).Instance()
                        
                    );
                }
                fileName = Dir.GetNext();
            }
            Dir.ListDirEnd();
            return scenes;
        }
        return null;
    }

    public Node GetData(string name){
        return GetNode(name);
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
