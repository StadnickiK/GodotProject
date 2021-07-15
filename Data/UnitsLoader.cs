using Godot;
using System.Collections.Generic;

public class UnitsLoader : Node
{

    [Export]
    public string Path { get; set; } = null;
    JSONLoader _loader = new JSONLoader();

    public List<Unit> WorldUnits { get; } = new List<Unit>();

    public override void _Ready()
    {
        if(Path != null){
            _loader.Path = Path;
            TranslateJSON(_loader.LoadData());
        }
    }

    List<Unit> TranslateJSON(Godot.Collections.Dictionary Data){
        if(Data != null){
            foreach(var UnitName in Data.Keys){
                string name = (string)UnitName;

                Godot.Collections.Dictionary<string, int> buildCost = new Godot.Collections.Dictionary<string, int>();
                
                var UnitData = (Godot.Collections.Dictionary)Data[name];

                if(UnitData.Contains("BuildCost"))
                    buildCost = TranslateDictionary(_loader.StringToGodotDictionary((string)UnitData["BuildCost"]));
                
                var stats = new List<BaseStat>();
                
                stats.Add(new BaseStat("HitPoints", (int)(float)UnitData["HitPoints"]));
                stats.Add(new BaseStat("Attack", (int)(float)UnitData["Attack"]));
                stats.Add(new BaseStat("Defence", (int)(float)UnitData["Defence"]));

                var unit = new Unit(name, stats);
                WorldUnits.Add(unit);
                // GD.Print(res);
            }
            return WorldUnits;
        }
        return null;
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

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
