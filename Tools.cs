using Godot;
using System;
using System.Collections.Generic;


    public static class Tools
	{
	public static Vector3 GetMouseWorldPosition(Node3D node, Camera3D camera, PhysicsRayQueryParameters3D _querry, float ray_length = 1000f)
    {
        var mousePos = node.GetViewport().GetMousePosition();
        var from = camera.ProjectRayOrigin(mousePos);
        var to = from + camera.ProjectRayNormal(mousePos) * ray_length;
        _querry.From = from;
        _querry.To = to;
        Vector3 p = Vector3.Zero;
        var space_state = node.GetWorld3D().DirectSpaceState;
        var state = space_state.IntersectRay(_querry);
        if (state.ContainsKey("position"))
        {
            p = (Vector3)state["position"];
        }
        return p;
    }
		// public static Dictionary<T, Y> GodotDictionaryToSystemDictionary<T,Y>( Godot.Collections.Dictionary<T, Y> sourceDictionary){
		// 	var result = new Dictionary<T,Y>();
		// 	foreach(var item in sourceDictionary){
		// 		result.Add(item.Key, item.Value);
		// 	}
		// 	return result;
		// }

		// public static List<T> GodotArrayToSystemList<T>( Godot.Collections.Array<T> sourceList){
		// 	var result = new List<T>();
		// 	foreach(var item in sourceList){
		// 		result.Add(item);
		// 	}
		// 	return result;
		// }    

		// 	public static Dictionary<T, List<int>> GodotDictionaryWithListToSystemDictionary<T>( Godot.Collections.Dictionary<T, Godot.Collections.Array<int>> sourceDictionary){
		// 	var result = new Dictionary<T,List<int>>();
		// 	foreach(var item in sourceDictionary){
		// 		var list = GodotArrayToSystemList<int>(sourceDictionary[item.Key]);
		// 		result.Add(item.Key, list);
		// 	}
		// 	return result;
		// }
	}


