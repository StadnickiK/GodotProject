using Godot;
using System.Collections.Generic;

public class BuildingLoader : Node
{

    [Export]
    public string Path { get; set; } = null;
    JSONLoader _loader = new JSONLoader();

    public List<Building> WorldBuildings { get; } = new List<Building>();

    public override void _Ready()
    {
        if(Path != null){
            _loader.Path = Path;
            TranslateJSON(_loader.LoadData());
        }
    }

    List<Building> TranslateJSON(Godot.Collections.Dictionary Data){
        if(Data != null){
            foreach(var buildingName in Data.Keys){
                string name = (string)buildingName;

                Godot.Collections.Dictionary<string, int> buildCost = new Godot.Collections.Dictionary<string, int>(), 
                products = new Godot.Collections.Dictionary<string, int>(), 
                productCost = new Godot.Collections.Dictionary<string, int>(),
                resourceLimits = new Godot.Collections.Dictionary<string, int>();
                
                var buildingData = (Godot.Collections.Dictionary)Data[name];

                if(buildingData.Contains("BuildCost"))
                    buildCost = TranslateDictionary(_loader.StringToGodotDictionary((string)buildingData["BuildCost"]));
                if(buildingData.Contains("Products"))
                    products = TranslateDictionary(_loader.StringToGodotDictionary((string)buildingData["Products"]));
                if(buildingData.Contains("ProductCost"))
                    productCost = TranslateDictionary(_loader.StringToGodotDictionary((string)buildingData["ProductCost"]));
                if(buildingData.Contains("ResourceLimits"))
                    resourceLimits = TranslateDictionary(_loader.StringToGodotDictionary((string)buildingData["ResourceLimits"]));

                var building = new Building(){
                    Name = name,
                    IsStarter = (bool)buildingData["Starter"],
                    BuildTime = (int)(float)buildingData["BuildTime"],
                    ResourceLimits = resourceLimits, //buildingData.Contains("ProductCapacity") ? (int)(float)buildingData["ProductCapacity"] : 0,
                    BuildCost = buildCost,
                    Products = products,
                    ProductCost = productCost
                };
                WorldBuildings.Add(building);
                // GD.Print(res);
            }
            return WorldBuildings;
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
