using Godot;
using System;
using System.Collections.Generic;

public class Generator : Node
{

    Random Rand { get; set; }

    Dictionary<string, int> parameters = null;

    World _world = null;

    public int SystemSize { get; set; } = 10;

    PackedScene _GalaxyScene = (PackedScene)ResourceLoader.Load("res://Map/Galaxy.tscn");

    PackedScene StarSystemScene = (PackedScene)GD.Load("res://Map/Starsystem/StarSystem.tscn");

    PackedScene SunScene = (PackedScene)GD.Load("res://Map/StarSystem/Star.tscn");

    PackedScene PlanetScene = (PackedScene)GD.Load("res://Map/Planet/Planet.tscn");

    public void InitGenerator(World world, Random random, Dictionary<string, int> Parameters){
        Rand = random;
        parameters = Parameters;
        _world = world;
    }

	Galaxy InitGalaxy(){
		var Galaxy = (Galaxy)_GalaxyScene.Instance();
		if(parameters != null){
			if(parameters.ContainsKey("Systems")){
				Galaxy.StarSystemNumber = parameters["Systems"];
			}
		}
		Galaxy.Connect("CameraLookAt",this,"_on_CameraLookAt");
		Galaxy.Connect("LookAtStarSystem", this, "_on_LookAtStarSystem");
        return Galaxy;
	}

    public Galaxy GenerateGalaxy(){

        var galaxy = InitGalaxy();
        int dist = Rand.Next(5, 10);
        float angle = Rand.Next(0, 70);
        for(int i = 0;i < galaxy.StarSystemNumber; i++){
            galaxy.RotateY(angle);
            var starSystem = GenerateStarSystem(galaxy, dist, i);
            galaxy.AddChild(starSystem);
            galaxy.StarSystems.Add(starSystem);
            dist += Rand.Next(10, 15);
            angle += Rand.Next(0, 60);
        }
        galaxy.Radius = (int)(1.2f*dist);
        var biggestRadius = GetBiggestStarSystemRadius(galaxy);
        if(galaxy.Radius < biggestRadius){
            galaxy.Radius = (int)(biggestRadius*1.5f);
        }
        galaxy.Rotation = Vector3.Zero;

        return galaxy;
    }

    int GetBiggestStarSystemRadius(Galaxy galaxy){
        int max = 0;
        foreach(StarSystem system in galaxy.StarSystems){
            if(system.Radius > max) max = system.Radius;
        }
        return max;
    }

    StarSystem InitSystem(Galaxy galaxy, int dist, int i = 0){
            var pos = galaxy.Transform.basis.Xform(new Vector3(0, 0, dist));
            var starSystem = (StarSystem)StarSystemScene.Instance();
            starSystem.Rand = Rand;
            starSystem.SystemID = i;
            starSystem.SystemName = "System " + i;
            starSystem.Connect("ViewStarSystem", galaxy, ("_on_ViewStarSystem"));
            starSystem.Connect("ViewGalaxy", galaxy, ("_on_ViewGalaxy"));
            var temp = starSystem.Transform;
            temp.origin = pos;
            starSystem.Transform = temp;
            return starSystem;
    }

    StarSystem GenerateStarSystem(Galaxy galaxy, int dist, int i = 0){
        var system = InitSystem(galaxy, dist, i);
		var sun = SunScene.Instance();
        system.GetNodes();
		system.StarSysObjects.AddChild(sun);
		system.SystemStar = (Star)sun;
		system.SystemName3D.Text = system.SystemName;
		dist = Rand.Next(5, 15);
		float angle = Rand.Next(0, 70);
		for(i = 0;i < SystemSize; i++){
			system.RotateY(angle);
			var planet = GeneratePlanet(system, dist, i);
			system.StarSysObjects.AddChild(planet);
			system.Planets.Add(planet);
            ConnectPlanet(planet);
			dist += Rand.Next(4, 10);
			angle += Rand.Next(0,50);
		}
		system.Radius = (int)(dist*1.2f);
		system.StarSysObjects.Visible = false;
		system.Rotation = Vector3.Zero;
        return system;
	}

    Planet GeneratePlanet(StarSystem system, int dist, int i = 0){
        var pos = system.Transform.basis.Xform(new Vector3(0, 0, dist));
		var planet = (Planet)PlanetScene.Instance();
        planet.GetNodes(); 
		planet.PlanetName = system.SystemName +" "+ i;
		planet.Rand = Rand;
		planet.System = system;
		var temp = planet.Transform;
		temp.origin = pos;
		planet.Transform = temp;
        return planet;
    }

    void ConnectPlanet(Planet planet){
        _world.ConnectTo_OpenPlanetInterface(planet);
        planet.Connect("CreateShip", _world, "_on_CreateShip");
        var arr = new Godot.Collections.Array();
        arr.Add(planet.Orbit);
        planet.IcoOrbit.Connect("mouse_entered", _world.UInterface,"_on_OrbitIconFocus", arr);
        var arr2 = new Godot.Collections.Array();
        arr2.Add(planet);
        planet.Connect("OpenCmdPanel", _world, "_on_OpenPlanetCmdPanel");
        _world.WCC.ConnectToSelectTarget(planet);  
    }


}
