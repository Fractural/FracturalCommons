using Godot;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fractural.Utils
{
	public static class FileUtils
	{
		/// <summary>
		/// Gets all the files in a directory. This works in standalone builds of the game, as it checks .import files.
		/// </summary>
		/// <param name="rootPath"></param>
		/// <param name="searchSubDirectories"></param>
		/// <param name="fileExtensions"></param>
		/// <returns></returns>
		public static List<string> GetDirFiles(
			string rootPath,
			bool searchSubDirectories = true,
			IEnumerable<string> fileExtensions = null,
			IEnumerable<string> directoryBlacklist = null)
		{
			return GetDirContents(rootPath, searchSubDirectories, fileExtensions, directoryBlacklist).files;
		}

		/// <summary>
		/// Gets all the directories in a directory. This works in standalone builds of the game, as it checks .import files.
		/// </summary>
		/// <param name="rootPath"></param>
		/// <param name="searchSubDirectories"></param>
		/// <param name="fileExtensions"></param>
		/// <returns></returns>
		public static List<string> GetDirDirectories(
			string rootPath,
			bool searchSubDirectories = true,
			IEnumerable<string> fileExtensions = null,
			IEnumerable<string> directoryBlacklist = null)
		{
			return GetDirContents(rootPath, searchSubDirectories, fileExtensions, directoryBlacklist).directories;
		}

		/// <summary>
		/// Gets all the contents in a directory. This works in standalone builds of the game, as it checks .import files.
		/// </summary>
		/// <param name="rootPath"></param>
		/// <param name="searchSubDirectories"></param>
		/// <param name="fileExtensions"></param>
		/// <returns></returns>
		public static (List<string> files, List<string> directories) GetDirContents(
			string rootPath,
			bool searchSubDirectories = true,
			IEnumerable<string> fileExtensions = null,
			IEnumerable<string> directoryBlacklist = null)
		{
			var fileExtensionsHashSet = fileExtensions == null ? null : new HashSet<string>(fileExtensions);
			var directoryBlacklistHashset = directoryBlacklist == null ? null : new HashSet<string>(directoryBlacklist);
			return GetDirContentsHelper(rootPath, searchSubDirectories, fileExtensionsHashSet, directoryBlacklistHashset);
		}

		private static (List<string> files, List<string> directories) GetDirContentsHelper(
			string rootPath,
			bool searchSubDirectories = true,
			HashSet<string> fileExtensions = null,
			HashSet<string> directoryBlacklist = null)
		{
			var files = new List<string>();
			var directories = new List<string>();
			var dir = new Directory();

			Debug.Assert(rootPath != "", "Expected rootPath to not be empty!");

			var error = dir.Open(rootPath);
			if (error == Error.Ok)
			{
				dir.ListDirBegin(true, false);
				AddDirContents(dir, files, directories, searchSubDirectories, fileExtensions, directoryBlacklist);
			}
			else
			{
				GD.PushWarning($"GetDirContents(): An error occured when trying to access the path. Error code: \"{error}\" ");
			}

			return (files, directories);
		}

		private static void AddDirContents(
			Directory directory,
			List<string> files,
			List<string> directories,
			bool searchSubDirectories = true,
			HashSet<string> fileExtensions = null,
			HashSet<string> directoryBlacklist = null)
		{
			var fileName = directory.GetNext();

			while (fileName != "")
			{
				var path = directory.GetCurrentDir() + "/" + fileName;
				if (directory.CurrentIsDir())
				{
					var subDir = new Directory();
					subDir.Open(path);
					subDir.ListDirBegin(true, false);
					directories.Add(path);

					if (searchSubDirectories && (directoryBlacklist == null || (directoryBlacklist != null && !directoryBlacklist.Contains(fileName))))
					{
						AddDirContents(subDir, files, directories, searchSubDirectories, fileExtensions);
					}
				}
				else
				{
					if (fileExtensions == null)
						files.Add(path);
					else
					{
						if (!Engine.EditorHint)
							path = path.TrimSuffix(".import");
						if (fileExtensions.Contains(path.GetExtension()))
							files.Add(path);
					}
				}

				fileName = directory.GetNext();
			}

			directory.ListDirEnd();
		}
	}
}