using Godot;
using System;

public class JSONLoader : Node
{

    [Export]
    public string Path { get; set; } = null;

    public Godot.Collections.Dictionary Data { get; set; } = null;

    public override void _Ready()
    {
        LoadData();
    }

    public Godot.Collections.Dictionary LoadData(){
        var file = new Godot.File();
        if(Path != null){
            Error er = file.Open(Path, File.ModeFlags.Read);
            if(er == 0){
                string fileData = file.GetAsText();
                Data = StringToGodotDictionary(fileData);
                file.Close();
                return Data;
                // GD.Print(Data);
            }else{
                GD.Print(er);
                return null;
            }
        }else{
            GD.Print(Name+": Path is null");
            return null;
        }
    }

    public Godot.Collections.Dictionary StringToGodotDictionary(string text){
        // GD.Print(text);
        if(text != null){
            JSONParseResult result = JSON.Parse(text);
            return (result.Result as Godot.Collections.Dictionary);
        }
        return null;
    }

        bool Open(string dirPath){
        Directory Dir = new Directory();
        if(Dir.Open(dirPath) == Godot.Error.Ok){
            Dir.ListDirBegin();

            var fileName = Dir.GetNext();
            while(fileName != "" || fileName != null){
                if(!Dir.CurrentIsDir()){
                    var scene = (PackedScene)GD.Load(fileName);   
                }
            }
            fileName = Dir.GetNext();

            Dir.ListDirEnd();
            return true;
        }
        return false;
    }

}
