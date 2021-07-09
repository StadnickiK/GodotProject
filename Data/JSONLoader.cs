using Godot;
using System;

public class JSONLoader : Node
{

    [Export]
    public string Path { get; set; } = null;

    public Godot.Collections.Dictionary Data { get; set; } = null;

    public override void _Ready()
    {
        var file = new Godot.File();
        if(Path != null){
            Error er = file.Open(Path, File.ModeFlags.Read);
            if(er == 0){
                string fileData = file.GetAsText();
                JSONParseResult result = JSON.Parse(fileData);
                Data = result.Result as Godot.Collections.Dictionary;
                // GD.Print(Data);
            }else{
                GD.Print(er);
            }
        }else{
            GD.Print(Name+": Path is null");
        }
        file.Close();
    }

}
