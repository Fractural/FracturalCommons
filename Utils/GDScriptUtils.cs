using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using GDDictionary = Godot.Collections.Dictionary;

/// <summary>
/// Utilities used by Fractural Studios.
/// </summary>
namespace Fractural.Utils
{
	/// <summary>
	/// Utilities for Godot Nodes.
	/// </summary>
	public static class GDScriptUtils
	{
		// Bridges the custom GDScipt typing system
		// into the C# world.

		/// <summary>
		/// Checks the type of a GDScript class.
		/// Only use this when you want compatability 
		/// with GDScript classes.
		/// </summary>
		/// <param name="obj">Object being checked</param>
		/// <returns>Type of "obj" as a string</returns>
		public static string GetTypeName(object obj)
		{
			if (obj is Godot.Object gdObj && gdObj.GetScript() is Godot.GDScript && gdObj.HasMethod("get_types"))
			{
				// GDScript custom type name
				return (gdObj.Call("get_types") as string[])[0];
			}
			return obj.GetType().Name;
		}

		/// <summary>
		/// Check the type of a GDScript class.
		/// Only use this when you want compatability
		/// with GDScript classes.
		/// </summary>
		/// <param name="obj">Object being checked</param>
		/// <param name="type">Type that we want to check</param>
		/// <returns>True if "obj" is "type"</returns>
		public static bool IsType(object obj, string type)
		{
			if (obj is Godot.Object gdObj && gdObj.GetScript() is Godot.GDScript && gdObj.HasMethod("get_types"))
			{
				// GDScript custom type checking
				return Array.Exists((gdObj.Call("get_types") as string[]), typeString => typeString == type);
			}
			return obj.GetType().Name == "type";
		}

		/// <summary>
		/// Attempts to free a Godot Object.
		/// </summary>
		/// <returns>True if the object could be freed</returns>
		public static bool TryFree(Godot.Object obj)
		{
			if (obj != null && !(obj is Reference))
			{
				obj.Free();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Attempts to free a collection of Godot Objects.
		/// </summary>
		/// <param name="collection">Collection to be freed</param>
		/// <returns>True if all elements in "collection" could be freed</returns>
		public static bool TryFree(this IEnumerable<Godot.Object> collection)
		{
			foreach (Godot.Object obj in collection)
			{
				if (!TryFree(obj))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Getse a property from a Godot Object that is
		/// casted to a certain type.
		/// </summary>
		/// <param name="obj">Object being checked</param>
		/// <param name="property">Name of the property</param>
		/// <typeparam name="T">Type to cast the property's value too</typeparam>
		/// <returns>The value of the property casted to "T"</returns>
		public static T Get<T>(this Godot.Object obj, string property)
		{
			return (T)obj.Get(property);
		}

		/// <summary>
		/// Checks if a Godot Object has a property.
		/// </summary>
		/// <param name="obj">Object being used</param>
		/// <param name="property">Name of the property</param>
		/// <returns>True if the object has the property</returns>
		public static bool Has(this Godot.Object obj, string property)
		{
			return obj.Get(property) != null;
		}

		/// <summary>
		/// Calls a method on a Godot Object and returns the 
		/// result as a certain type.
		/// </summary>
		/// <param name="obj">Object being used</param>
		/// <param name="method">Name of method to call</param>
		/// <param name="args">Arguments to pass into the method</param>
		/// <typeparam name="T">Type to cast the result to</typeparam>
		/// <returns>The result of the method call, casted to "T"</returns>
		public static T Call<T>(this Godot.Object obj, string method, params object[] args)
		{
			return (T)obj.Call(method, args);
		}

		public static T AsWrapper<T>(this Godot.Object source) where T : GDScriptWrapper
		{
			return (T)Activator.CreateInstance(typeof(T), new object[] { source });
		}

		public static T GetNodeAsWrapper<T>(this Node node, NodePath path) where T : GDScriptWrapper
		{
			return node.GetNode(path).AsWrapper<T>();
		}

		public static T Get<T>(this GDDictionary dictionary, object key, T defaultReturn = default)
		{
			if (dictionary.Contains(key))
				return (T)dictionary[key];
			return defaultReturn;
		}

		public static T Get<T>(this GDDictionary dictionary, string key, T defaultReturn = default)
		{
			var keys = key.Split(".");
			for (int i = 0; i < keys.Length; i++)
			{
				if (i == keys.Length - 1)
				{
					if (dictionary.Contains(key))
						return (T)dictionary[key];
					return defaultReturn;
				}
				dictionary = dictionary.Get<GDDictionary>(keys[i]);
				if (dictionary == null)
					return defaultReturn;
			}
			return defaultReturn;
		}

		public static GDDictionary ToGDDict(this object obj)
		{
			GDDictionary dict = new GDDictionary();
			foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				dict[prop.Name] = prop.GetValue(obj, null);
			}
			return dict;
		}

		public static Vector2 Lerp(this Vector2 start, Vector2 end, float weight)
		{
			return new Vector2(
				Mathf.Lerp(start.x, end.x, weight),
				Mathf.Lerp(start.y, end.y, weight)
			);
		}

		public static Vector3 Lerp(this Vector3 start, Vector3 end, float weight)
		{
			return new Vector3(
				Mathf.Lerp(start.x, end.x, weight),
				Mathf.Lerp(start.y, end.y, weight),
				Mathf.Lerp(start.z, end.z, weight)
			);
		}
	}
}