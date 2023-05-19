using Godot;
using System;
using System.Collections.Generic;
using GDC = Godot.Collections;

/// <summary>
/// Utilities used by Fractural Studios.
/// </summary>
namespace Fractural.Utils
{
    /// <summary>
    /// Utilities for Godot Nodes.
    /// </summary>
    public static class GDUtils
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
        /// Gets a property from a Godot Object that is
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
        /// Checks if <paramref name="obj"/> is a remote object of type <typeparamref name="T"/>, 
        /// such as a node in the remote view of the scene hierarchy when the game is playing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsRemoteInspectedObject<T>(Godot.Object obj) where T : Godot.Object
        {
            if (obj.GetClass() != "ScriptEditorDebuggerInspectedObject") return false;
            var script = obj.Get("script");
            if (!(script is CSharpScript cSharpScript)) return false;
            return cSharpScript.ResourcePath == CSharpScript<T>.ResourcePath;
        }

        /// <summary>
        /// Checks if <paramref name="obj"/> is a remote object, such as a node in 
        /// the remote view of the scene hierarchy when the game is playing.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsRemoteInspectedObject(Godot.Object obj)
        {
            return obj.GetClass() == "ScriptEditorDebuggerInspectedObject";
        }

        /// <summary>
        /// Gets a property from a remote Godot Object that is
        /// casted to a certain type. Remote Godot Objects store all
        /// their properties under "Members/"
        /// </summary>
        /// <param name="obj">Object being checked</param>
        /// <param name="property">Name of the property</param>
        /// <typeparam name="T">Type to cast the property's value too</typeparam>
        /// <returns>The value of the property casted to "T"</returns>
        public static T GetRemote<T>(this Godot.Object obj, string property)
        {
            return (T)obj.Get($"Members/{property}");
        }

        /// <summary>
        /// Checks if a Godot Object has a property. Remote Godot Objects store all
        /// their properties under "Members/"
        /// </summary>
        /// <param name="obj">Object being used</param>
        /// <param name="property">Name of the property</param>
        /// <returns>True if the object has the property</returns>
        public static bool HasRemote(this Godot.Object obj, string property)
        {
            return obj.Get($"Members/{property}") != null;
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

        public static Theme GetThemeFromAncestor(this Node node, bool scanNonControlParents = false)
        {
            if (node == null) return null;
            if (node is Control control)
            {
                if (control.Theme != null)
                    return control.Theme;
            }
            else if (!scanNonControlParents)
                // The node wasn't a control
                // If we aren't including non control parents in the search, then our search ends here.
                return null;
            return GetThemeFromAncestor(node.GetParent(), scanNonControlParents);
        }

        public static T GetStylebox<T>(this Theme theme, string name, string themeType) where T : StyleBox
        {
            return theme.GetStylebox(name, themeType) as T;
        }

        public static T GetFont<T>(this Theme theme, string name, string themeType) where T : Font
        {
            return theme.GetFont(name, themeType) as T;
        }

        public static T GetStylebox<T>(this Control node, string name, string themeType) where T : StyleBox
        {
            return node.GetStylebox(name, themeType) as T;
        }

        public static T GetFont<T>(this Control node, string name, string themeType) where T : Font
        {
            return node.GetFont(name, themeType) as T;
        }

        public static string ToGDJSON(this object obj) => JSON.Print(obj);

        public static object FromGDJSON(this string json)
        {
            var result = JSON.Parse(json);
            if (result.Error != Error.Ok)
                return null;
            return result.Result;
        }

        /// <summary>
        /// Used to fill in the "binds" variable of Godot.Object.Connect()
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static GDC.Array GDParams(params object[] array)
        {
            var gdArray = new GDC.Array();
            foreach (var eleme in array)
                gdArray.Add(eleme);
            return gdArray;
        }

        public static object[] Params(params object[] array) => array;

        public static bool TryConnect(this Godot.Object obj, string signal, Godot.Object target, string method, GDC.Array binds = null, uint flags = 0)
        {
            if (obj.IsConnected(signal, target, method))
                return false;
            obj.Connect(signal, target, method, binds, flags);
            return true;
        }

        public static bool TryDisconnect(this Godot.Object obj, string signal, Godot.Object target, string method)
        {
            if (!obj.IsConnected(signal, target, method))
                return false;
            obj.Disconnect(signal, target, method);
            return true;
        }

        /// <summary>
        /// Returns the object's metadata for a give name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="defaultReturn"></param>
        /// <returns></returns>
        public static T GetMeta<T>(this Godot.Object obj, string name, T defaultReturn = default)
        {
            return (T)obj.GetMeta(name, defaultReturn);
        }
    }
}