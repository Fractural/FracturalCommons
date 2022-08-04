using System;
using System.Runtime.CompilerServices;
using Godot.Collections;
using Array = Godot.Collections.Array;

/* Example Usage:
	// Declare a class with the attribute
	[CSharpScript]
	public class CustomResource : Resource { ... }
	// Later, create new resources with
	CSharpScript<CustomResource>.New()
	
	// Report issues to the gist at: https://gist.github.com/cgbeutler/c4f00b98d744ac438b84e8840bbe1740
*/

namespace Godot.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class CSharpScriptAttribute : Attribute
	{
		public CSharpScriptAttribute([CallerFilePath] string sourceFilePath = "")
		{
			SourceFilePath = sourceFilePath;
		}

		public string SourceFilePath { get; set; }
	};

	public static class CSharpScript<T> where T : class
	{
		public static string ResourcePath()
		{
			var sourceInfo = (CSharpScriptAttribute) Attribute.GetCustomAttribute(typeof(T), typeof(CSharpScriptAttribute));
			if (sourceInfo == null)
			{
				GD.PushError( $"Could not file script info. Did you add '{nameof(CSharpScriptAttribute)}' to the class '{typeof(T).Name}'?" );
				return "";
			}
			if ( sourceInfo?.SourceFilePath.GetFile().BaseName() != typeof(T).Name )
			{
				GD.PushError( $"Class and script name mismatch. Class name is '{ typeof(T).Name }' for script '{ sourceInfo?.SourceFilePath }'" );
				return "";
			}
			return sourceInfo?.SourceFilePath ?? "";
		}

		public static CSharpScript AsCSharpScript()
		{
			var scriptPath = ResourcePath();
			if ( scriptPath.Empty() ) { throw new Exception("Can't load CSharp Script"); }
			// Don't worry, it will usually be a cached load
			// Also, in tool mode it can get scrapped randomly, so we kinda need to use load each time
			return GD.Load<CSharpScript>( scriptPath );
		}

		/// Returns a new instance of the script.
		public static T New()
		{
			var script = AsCSharpScript();
			// if ( Engine.EditorHint && ! script.IsTool() )
			// { GD.PushWarning($"Script is not in tool mode: '{ typeof(T).Name }'"); }
			return (T)script.New();
		}

		/// Returns the default value of the specified property.
		public static object GetPropertyDefaultValue(string property)
		{
			var script = AsCSharpScript();
			return script.GetPropertyDefaultValue( property );
		}
		
		/// Returns a dictionary containing constant names and their values.
		public static Dictionary GetScriptConstantMap()
		{
			var script = AsCSharpScript();
			return script.GetScriptConstantMap();
		}

		/// Returns the list of methods in this Godot.Script.
		public static Array GetScriptMethodList()
		{
			var script = AsCSharpScript();
			return script.GetScriptMethodList();
		}

		/// Returns the list of properties in this Godot.Script.
		public static Array GetScriptPropertyList()
		{
			var script = AsCSharpScript();
			return script.GetScriptPropertyList();
		}

		/// Returns the list of user signals defined in this Godot.Script.
		public static Array GetScriptSignalList()
		{
			var script = AsCSharpScript();
			return script.GetScriptSignalList();
		}
		
		/// Returns true if the script, or a base class, defines a signal with the given
		/// name.
		public static bool HasScriptSignal(string signalName)
		{
			var script = AsCSharpScript();
			return script?.HasScriptSignal( signalName ) ?? false;
		}

		/// Returns true if the script is a tool script. A tool script can run in the editor.
		public static bool IsTool()
		{
			var script = AsCSharpScript();
			return script?.IsTool() ?? false;
		}
	};
}