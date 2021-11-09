using Godot;
using System;
using System.Collections.Generic;

public class ResourcesLoader : Node
{
    [Export]
    public string Path { get; set; } = null;
    JSONLoader _loader = new JSONLoader();

    public List<Resource> WorldResources { get; } = new List<Resource>();

    public override void _Ready()
    {
        if(Path != null){
            _loader.Path = Path;
            TranslateJSON(_loader.LoadData());
        }
    }

    void TranslateJSON(Godot.Collections.Dictionary Data){
        if(Data != null){
            foreach(var resName in Data.Keys){
                string name = (string)resName;
                var resData = (Godot.Collections.Dictionary)Data[name];
                Resource.Type type;
                Enum.TryParse((string)resData["Type"], out type);
                var res = new Resource(){
                    Name = name,
                    ResourceType = type,
                    Rarity = (int)(float)resData["Rarity"],
                    IsStarter = (bool)resData["Starter"]
                };
                WorldResources.Add(res);
                // GD.Print(res);
            }
        }
    }

    public Godot.Collections.Dictionary<string, int> IfContainsTranslateDictionary(string key, Godot.Collections.Dictionary source){
        if(source.Contains(key)){
            return TranslateDictionary(source);
        }
        return null;    
    }

    public Godot.Collections.Dictionary<string, int> TranslateDictionary(Godot.Collections.Dictionary source){
        if(source != null){
            var temp = new Godot.Collections.Dictionary<string, int>();
            foreach(var name in source.Keys){
                temp.Add(
                    ((string)name),
                    ((int)(float)source[name])
                );
            }
            return temp;
            }
        return null;
    }



}
