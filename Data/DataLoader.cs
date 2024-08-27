using Godot;
using System;
using System.Collections.Generic;

public partial class DataLoader : Node
{
    [Export]
    public string DirPath { get; set; } = null;

    public override void _Ready()
    {
        if(DirPath != null && DirPath != "")
            GetScenes(DirPath);
    }

    public DirAccess Dir { get; set; }

    bool Open(string dirPath){
        Dir = DirAccess.Open(dirPath);
        if(Dir != null){
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
                    AddChild(ResourceLoader.Load<PackedScene>(dirPath+fileName).Instantiate());
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
