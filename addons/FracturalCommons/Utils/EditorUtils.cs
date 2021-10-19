using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractural.Utils
{
	public static class EditorUtils
	{
		public static Texture GetIconRecursive<T>(this Control controlNode)
		{
			return GetIconRecursive(controlNode, typeof(T));
		}

		public static Texture GetIconRecursive(this Control controlNode, object obj)
		{
			Type godotBaseType = obj.GetType();
			while (godotBaseType.Namespace?.Split(".").FirstOrDefault() != nameof(Godot) && godotBaseType != null)
				godotBaseType = godotBaseType.BaseType;
			return controlNode.GetIcon(godotBaseType.Name, "EditorIcons");
		}

		public static Type GetRealType(Node node)
		{
			var attachedCSharpScript = ((CSharpScript)node.GetScript());
			if (attachedCSharpScript != null)
			{
				var type = Type.GetType(attachedCSharpScript.ResourcePath.GetFileName());
				if (type != null)
					return type;
			}
			return node.GetType();
		}
	}
}
