using System.Linq;
using Godot;
using System.Collections.Generic;
using Fractural.Information;
using System.Text.RegularExpressions;

/// <summary>
/// Utilities used by Fractural Studios.
/// </summary>
namespace Fractural.Utils
{
	/// <summary>
	/// Utilities for Engine related things.
	/// </summary>
	public static class EngineUtils
	{
		public static VersionInfo CurrentVersionInfo
		{
			get
			{
				return new VersionInfo(
					(int)Engine.GetVersionInfo()["major"],
					(int)Engine.GetVersionInfo()["minor"],
					(int)Engine.GetVersionInfo()["patch"]
				);
			}
		}

		#region Version Preprocessor Defines

		public static VersionInfo[] AllGodotVersions => new VersionInfo[]
		{
			"4.0",

			"3.6",

			"3.5",

			"3.4.5",
			"3.4.4",
			"3.4.3",
			"3.4.2",
			"3.4.1",
			"3.4.0",

			"3.3.3",
			"3.3.2",
			"3.3.1",
			"3.3.0",

			"3.2.3",
			"3.2.2",
			"3.2.1",
			"3.2.0",

			"3.1.2",
			"3.1.1",
			"3.1.0",

			"3.0.6",
			"3.0.5",
			"3.0.4",
			"3.0.3",
			"3.0.2",
			"3.0.1",
			"3.0.0",

			// No support for 2.0.0 and below since C# support 
			// does not exist for versions below 3.0.0.
		};

		public static void GenerateVersionPreprocessorDefines()
		{
			var dir = new Directory();
			List<string> projectFiles = FileUtils.GetDirFiles("res://", true, new[] { "csproj" });
			HashSet<VersionInfo> allGodotVersionsHashset = new HashSet<VersionInfo>(AllGodotVersions);
			bool atleastOneProjectFilesChanged = false;
			if (projectFiles.Any())
				foreach (string project in projectFiles)
				{
					bool projectFileChanged = false;
					File file = new File();
					file.Open(project, File.ModeFlags.Read);
					string text = file.GetAsText();
					// No need to try and remove a version since we know all Godot versions
					// will generate 
					//		a full version define,
					//		a major version define,
					//		and a major minor version define.
					if (TryWriteCurrVersion(ref text) | TryWriteCurrVersionMajor(ref text) | TryWriteCurrVersionMajorMinor(ref text))
						projectFileChanged = true;

					foreach (VersionInfo info in AllGodotVersions)
					{
						if (CurrentVersionInfo >= info)
						{
							if (TryWriteVersionOrNewer(ref text, info))
								projectFileChanged = true;
						}
						else if (TryRemoveVersionOrNewer(ref text, info))
							projectFileChanged = true;
					}
					file.Close();
					if (projectFileChanged)
					{
						file = new File();
						file.Open(project, File.ModeFlags.Write);
						file.StoreString(text);
						file.Close();
					}

					if (projectFileChanged)
						atleastOneProjectFilesChanged = true;
				}

			if (atleastOneProjectFilesChanged)
			{
				GD.PushWarning("The Godot version saved in a .csproj is different from the current Godot version so it was overwritten. Please rebuild the solution for the updated .csproj file(s) to take effect.");
			}
		}

		private static bool TryWriteCurrVersion(ref string text)
		{
			Regex regex = new Regex(@"<DefineConstants>\$\(DefineConstants\);GODOT_\d+_\d+_\d+<\/DefineConstants>", RegexOptions.Compiled);

			var match = regex.Match(text);
			string updatedText = "";
			if (match.Success)
			{
				VersionInfo savedVersionInfo = new VersionInfo(match.Value.Replace("<DefineConstants>$(DefineConstants);GODOT_", "").Replace("</DefineConstants>", "").Split('_').Select(x => int.Parse(x)).ToArray());
				if (CurrentVersionInfo != savedVersionInfo)
				{
					updatedText = regex.Replace(text, $"<DefineConstants>$(DefineConstants);GODOT_{CurrentVersionInfo.Major}_{CurrentVersionInfo.Minor}_{CurrentVersionInfo.Patch}</DefineConstants>");
				}
			}
			else
			{
				updatedText = AddVersionInfoDefine(text, $"<DefineConstants>$(DefineConstants);GODOT_{CurrentVersionInfo.Major}_{CurrentVersionInfo.Minor}_{CurrentVersionInfo.Patch}</DefineConstants>");
			}

			if (updatedText != "")
			{
				text = updatedText;
				return true;
			}
			return false;
		}

		private static bool TryWriteCurrVersionMajor(ref string text)
		{
			Regex regex = new Regex(@"<DefineConstants>\$\(DefineConstants\);GODOT_\d+<\/DefineConstants>", RegexOptions.Compiled);

			var match = regex.Match(text);
			string updatedText = "";
			if (match.Success)
			{
				VersionInfo savedVersionInfo = new VersionInfo(match.Value.Replace("<DefineConstants>$(DefineConstants);GODOT_", "").Replace("</DefineConstants>", "").Split('_').Select(x => int.Parse(x)).ToArray());
				if (CurrentVersionInfo.Major != savedVersionInfo.Major)
				{
					updatedText = regex.Replace(text, $"<DefineConstants>$(DefineConstants);GODOT_{CurrentVersionInfo.Major}</DefineConstants>");
				}
			}
			else
			{
				updatedText = AddVersionInfoDefine(text, $"<DefineConstants>$(DefineConstants);GODOT_{CurrentVersionInfo.Major}</DefineConstants>");
			}

			if (updatedText != "")
			{
				text = updatedText;
				return true;
			}
			return false;
		}

		private static bool TryWriteCurrVersionMajorMinor(ref string text)
		{
			Regex regex = new Regex(@"<DefineConstants>\$\(DefineConstants\);GODOT_\d+_\d+<\/DefineConstants>", RegexOptions.Compiled);

			var match = regex.Match(text);
			string updatedText = "";
			if (match.Success)
			{
				VersionInfo savedVersionInfo = new VersionInfo(match.Value.Replace("<DefineConstants>$(DefineConstants);GODOT_", "").Replace("</DefineConstants>", "").Split('_').Select(x => int.Parse(x)).ToArray());
				if (CurrentVersionInfo.Major != savedVersionInfo.Major || CurrentVersionInfo.Minor != savedVersionInfo.Minor)
				{
					updatedText = regex.Replace(text, $"<DefineConstants>$(DefineConstants);GODOT_{CurrentVersionInfo.Major}_{CurrentVersionInfo.Minor}</DefineConstants>");
				}
			}
			else
			{
				updatedText = AddVersionInfoDefine(text, $"<DefineConstants>$(DefineConstants);GODOT_{CurrentVersionInfo.Major}_{CurrentVersionInfo.Minor}</DefineConstants>");
			}

			if (updatedText != "")
			{
				text = updatedText;
				return true;
			}
			return false;
		}

		private static bool TryWriteVersionOrNewer(ref string text, VersionInfo versionInfo)
		{
			Regex regex = new Regex($@"\n\t\t<DefineConstants>\$\(DefineConstants\);GODOT_{versionInfo.Major}_{versionInfo.Minor}_{versionInfo.Patch}_OR_NEWER<\/DefineConstants>", RegexOptions.Compiled);

			var match = regex.Match(text);
			string updatedText = "";
			if (match.Success)
			{
				VersionInfo savedVersionInfo = new VersionInfo(match.Value.Replace("\n\t\t<DefineConstants>$(DefineConstants);GODOT_", "").Replace("_OR_NEWER</DefineConstants>", "").Split('_').Select(x => int.Parse(x)).ToArray());
				if (savedVersionInfo != versionInfo)
				{
					updatedText = regex.Replace(text, $"\n\t\t<DefineConstants>$(DefineConstants);GODOT_{versionInfo.Major}_{versionInfo.Minor}_{versionInfo.Patch}_OR_NEWER</DefineConstants>");
				}
			}
			else
			{
				updatedText = AddVersionInfoDefine(text, $"<DefineConstants>$(DefineConstants);GODOT_{versionInfo.Major}_{versionInfo.Minor}_{versionInfo.Patch}_OR_NEWER</DefineConstants>");
			}

			if (updatedText != "")
			{
				text = updatedText;
				return true;
			}
			return false;
		}

		private static bool TryRemoveVersionOrNewer(ref string text, VersionInfo versionInfo)
		{
			Regex regex = new Regex($@"\n\t\t<DefineConstants>\$\(DefineConstants\);GODOT_{versionInfo.Major}_{versionInfo.Minor}_{versionInfo.Patch}_OR_NEWER<\/DefineConstants>", RegexOptions.Compiled);

			var match = regex.Match(text);
			string updatedText = "";
			if (match.Success)
			{
				// Erase the VersionOrNewer predefine since it's wrong.
				updatedText = regex.Replace(text, "");
			}

			if (updatedText != "")
			{
				text = updatedText;
				return true;
			}
			return false;
		}

		private static string AddVersionInfoDefine(string text, string versionInfoDefineString)
		{
			string header = "\t<!-- GODOT VERSION DEFINES (KEEP THIS COMMENT) -->\n\t<PropertyGroup>";
			// We use this header to group all the versions together. We can't add
			// names to PropertyGroups so this is the best I could do to reduce
			// clutter when adding multiple version defines.
			if (text.Contains(header))
				return text.Replace(header, header + "\n\t\t" + versionInfoDefineString);
			else
				return text.Replace("</Project>", header + "\n\t\t" + versionInfoDefineString + "\n\t</PropertyGroup>\n</Project>");
		}

		#endregion

	}
}