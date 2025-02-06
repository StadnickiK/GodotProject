using Godot;
using System;
using System.Collections.Generic;

namespace Game
{
    public static class Tools
{
	public static Dictionary<T, Y> GodotDictionaryToSystemDictionary<T,Y>( Godot.Collections.Dictionary<T, Y> sourceDictionary){
		var result = new Dictionary<T,Y>();
		foreach(var item in sourceDictionary){
			result.Add(item.Key, item.Value);
		}
		return result;
	}

	public static List<T> GodotArrayToSystemList<T>( Godot.Collections.Array<T> sourceList){
		var result = new List<T>();
		foreach(var item in sourceList){
			result.Add(item);
		}
		return result;
	}    

		public static Dictionary<T, List<int>> GodotDictionaryWithListToSystemDictionary<T>( Godot.Collections.Dictionary<T, Godot.Collections.Array<int>> sourceDictionary){
		var result = new Dictionary<T,List<int>>();
		foreach(var item in sourceDictionary){
			var list = GodotArrayToSystemList<int>(sourceDictionary[item.Key]);
			result.Add(item.Key, list);
		}
		return result;
	}
}
}

