using System.Runtime.CompilerServices;
using Godot;
using Fractural.Results;
using ProjectSettings = Godot.ProjectSettings;
using Fractural.Utils;

/*
 
MIT License

Copyright (c) 2022 Cory Beutler

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 
*/

#nullable enable

namespace Fractural.IO
{
    /// Contains all io extension methods
    /// Operations usually treat relative paths as relative to res directory.
    /// Use realative path if you want it relative to the script you are in.
    public static class IO
    {
        #region  Path Operations 

        /// <summary> Globalize a given path. Evaluated at runtime. </summary>
        public static string GlobalizePath(string path) => ProjectSettings.GlobalizePath(path);
        /// <summary> Localize a given path. Evaluated at runtime. </summary>
        public static string LocalizePath(string path) => ProjectSettings.LocalizePath(path);


        /// <summary> The path of your project at compile time. </summary>
        public static string ProjectPath { get; }
        private static string InitProjectPath([CallerFilePath] string callerPath = "")
        {
            callerPath = callerPath.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            if (!callerPath.EndsWith("/" + IOSettings.IoScriptProjectPath))
            {
                // If you get the following error, update the constant 'IOToolsSettings.IOSCRIPTPROJPATH' to match THIS file's new project path
                GD.PushError("Failed to get project path. Path to IO.cs may have changed. Verify that the constant 'IOSCRIPTPROJPATH' in 'IOToolsSettings.cs' is correct.");
                throw new System.Exception("Failed to get project path. Path to IO.cs may have changed. Verify that the constant 'IOSCRIPTPROJPATH' in 'IOToolsSettings.cs' is correct.");
            }
            return callerPath.Remove(callerPath.Length - IOSettings.IoScriptProjectPath.Length);
        }
        static IO() { ProjectPath = InitProjectPath(); }


        /// <summary> Localize a project-item's path using the compile-time project path. </summary>
        public static string LocalizeProjectPath(string globalProjectPath)
        {
            globalProjectPath = globalProjectPath.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            if (!globalProjectPath.BeginsWith(ProjectPath))
            {
                GD.PushError($"Can't get localize project path:  Raw path didn't start with project path.");
                return "";
            }
            return "res://" + globalProjectPath.Substring(ProjectPath.Length);
        }


        /// <summary> Localize a relative project-item's path using the compile-time project path. </summary>
        public static string LocalizeRelativeProjectPath(string pathRelativeToCallerPath, [CallerFilePath] string callerPath = "")
        {
            callerPath = callerPath.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            if (!callerPath.BeginsWith(ProjectPath))
            {
                GD.PushError($"Can't get relative path:  Caller path didn't start with project path");
                return "";
            }
            if (!callerPath.EndsWith(".cs"))
            {
                GD.PushError($"Can't get relative path:  Expected caller path to be csharp script ending in '.cs'");
                return "";
            }
            var lastSlash = callerPath.FindLast("/");
            if (lastSlash < 0)
            {
                GD.PushError($"Can't get relative path:  Couldn't find last '/' in caller path");
                return "";
            }
            // Build the path to the caller, then over to the relative item
            var relpath = "res://" + callerPath.Substring(ProjectPath.Length, lastSlash + 1 - ProjectPath.Length) + pathRelativeToCallerPath;
            // The following should remove all "./" and "../" in the proper way, giving a simplified path
            return ProjectSettings.LocalizePath(relpath);
        }

        private static void LocalizeIfRelative(ref string path, string callerPath = "")
        {
            if (path.BeginsWith("./") || path.BeginsWith("../") || path.BeginsWith(".\\") || path.BeginsWith("..\\"))
            {
                path = LocalizeRelativeProjectPath(path, callerPath);
            }
        }

        private static string RemovePrefix(string s, string prefix) => prefix.Length == 0 ? s : s.Remove(0, prefix.Length);

        #endregion ==== Path Operations ====

        #region  Directory and File Operations 

        /// <summary> Checks to see if a directory exists. </summary>
        /// <param name="dirpath">
        ///   Path to directory to look for.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        public static bool DirExists(string dirpath, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref dirpath, callerPath);
            return new Directory().DirExists(dirpath);
        }

        /// <summary> Checks to see if a file exists. </summary>
        /// <param name="filepath">
        ///   Path to file to look for.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        public static bool FileExists(string filepath, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref filepath, callerPath);
            return new Directory().FileExists(filepath);
        }

        /// <summary> Checks to see if a file or directory exists. </summary>
        /// <param name="path">
        ///   Path to file or directory to look for.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        public static bool Exists(string path, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref path, callerPath);
            var dir = new Directory();
            return dir.DirExists(path) || dir.FileExists(path);
        }


        /// <summary>
        ///   Open a directory. Can create directory if it is missing and permissions allow creation.
        /// </summary>
        /// <param name="dirpath">
        ///   Path to a directory to open.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="createMissing">
        ///   If true, will try to create the directory if it is not initially found
        /// </param>
        /// <returns>
        ///   DataResult with <see cref="Directory"/> opened to dirpath on success.
        ///   Will have Error.CantOpen if it doesn't exist and createMissing is false.
        ///   Will have the error from 'MakeDirRecursive' if createMissing is true and that operation failed.
        /// </returns>
        public static DataResult<Directory> OpenDir(string dirpath, bool createMissing = false, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref dirpath, callerPath);
            var result = new DataResult<Directory>($"Open directory '{ dirpath }'");

            var dir = new Directory();
            if (!dir.DirExists(dirpath) && createMissing)
            {
                result.SetError(dir.MakeDirRecursive(dirpath), "Can't create directory");
                if (result.HasError) { return result; }
            }
            var err = dir.Open(dirpath);
            result.SetError(err == Error.InvalidParameter ? Error.CantOpen : err);
            if (result.IsOk) { result.Data = dir; }
            return result;
        }

        /// <summary>
        ///   Tries to ensure the existence of a directory, creating it if it is missing
        /// </summary>
        /// <param name="dirpath">
        ///   Path to a directory to ensure.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <returns>
        ///   Result with success or failure status set.
        ///   Forwards the error of 'MakeDirRecursive', if that failed.
        ///   Will have Error.CantOpen if something was there/made but can't be opened.
        /// </returns>
        public static Result EnsureDir(string dirpath, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref dirpath, callerPath);
            var result = new Result($"Ensure directory '{ dirpath }'");

            if (DirExists(dirpath)) { return result; }
            var dir = new Directory();
            result.SetError(dir.MakeDirRecursive(dirpath), "Can't create directory");
            if (result.HasError) { return result; }
            var err = dir.Open(dirpath);
            return result.WithError(err == Error.InvalidParameter ? Error.CantOpen : err, "Can't open directory");
        }

        /// <summary>
        ///   Tries to ensure that the base directory of a filepath exists, creating it if it is missing
        /// </summary>
        /// <param name="filepath">
        ///   Path to a file who's base directory will be ensured.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <returns>
        ///   Result with success or failure status set.
        ///   Forwards the error of 'MakeDirRecursive', if that failed.
        ///   Will have Error.CantOpen if something was there/made but can't be opened.
        /// </returns>
        public static Result EnsureBaseDir(string filepath, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref filepath, callerPath);
            var result = new Result("Ensure base directory for '{ filepath }'");
            if (filepath.GetFile().Empty())
            {
                return result.WithError(Error.InvalidDeclaration, "Filepath is missing file. Did you mean to use EnsureDir?");
            }
            var dirpath = filepath.GetBaseDir();
            if (dirpath.Empty())
            {
                return result.WithError(Error.InvalidDeclaration, "Filepath doesn't have base directory");
            }

            if (DirExists(dirpath)) { return result; } // OK!
            var dir = new Directory();
            result.SetError(dir.MakeDirRecursive(dirpath), "Can't create directory");
            if (result.HasError) { return result; }
            var err = dir.Open(dirpath);
            return result.WithError(err == Error.InvalidParameter ? Error.CantOpen : err, "Can't open directory");
        }


        /// <summary>
        ///   Delete the specified file.
        /// </summary>
        /// <remarks>
        ///   For some safety, this will produce error if deleting in Editor or if
        ///   deleting outside of the 'user://' folder. Use 'iAcceptFate' to
        ///   suppress those errors.
        /// </remarks>
        /// <param name="filepath">
        ///   Path to a file to delete.
        ///   Cannot take relative paths.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="iAcceptFate">
        ///   For some safety, this will produce error if deleting in Editor or if
        ///   deleting outside of the 'user://' folder. If true, this will suppress
        ///   those errors.
        /// </param>
        /// <returns>
        ///   Result with success or failure status set.
        ///   Will have Error.Failed if 'iAcceptFate' was not set and used dangrously.
        ///   Will have Error.FileNotFound if the file wasn't there.
        ///   Will forward the error of Godot's 'Remove' otherwise.
        /// </returns>
        public static Result DeleteFile(string filepath, bool iAcceptFate = false)
        {
            var result = new Result($"Delete '{ filepath }'");
            if (filepath.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return result.WithError(Error.Bug, "Invalid Parameter:  Path was emtpy");
            }

            if (!iAcceptFate)
            {
                if (Engine.EditorHint)
                {
                    return result.WithError(Error.Failed, "Dangerous code. Use 'iAcceptFate' while in editor");
                }
                if (!filepath.BeginsWith("user://"))
                {
                    return result.WithError(Error.Failed, "Dangerous code. Use 'iAcceptFate' for files outside 'user://'");
                }
            }

            var dir = new Directory();
            if (!dir.FileExists(filepath))
            {
                return result.WithError(Error.FileNotFound, "Found directory instead of file");
            }
            result.SetError(dir.Remove(filepath));
            return result;
        }


        /// <summary>
        ///   Delete the specified directory recursively.
        /// </summary>
        /// <remarks>
        ///   For some safety, this will produce error if deleting in Editor or if
        ///   deleting outside of the 'user://' folder. Use 'iAcceptFate' to
        ///   suppress those errors.
        /// </remarks>
        /// <param name="dirpath">
        ///   Path to a directory to delete.
        ///   Cannot take relative paths.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="iAcceptFate">
        ///   For some safety, this will produce error if deleting in Editor or if
        ///   deleting outside of the 'user://' folder. If true, this will suppress
        ///   those errors.
        /// </param>
        /// <returns>
        ///   BatchResult with success or failure statuses for each operation within.
        ///   Will have Error.Failed if 'iAcceptFate' was not set and used dangrously.
        ///   Sub-results will match those of 'DeleteFile'
        /// </returns>
        public static BatchResult DeleteDir(string dirpath, bool iAcceptFate = false)
        {
            var results = new BatchResult($"Delete '{ dirpath }'");
            if (dirpath.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return results.WithError(Error.Bug, "Invalid Parameter:  Path was emtpy");
            }

            // Check for dangerous usage
            if (!iAcceptFate)
            {
                if (Engine.EditorHint)
                {
                    return results.WithError(Error.Failed, "Dangerous code. Use 'iAcceptFate' while in editor");
                }
                if (!dirpath.BeginsWith("user://"))
                {
                    return results.WithError(Error.Failed, "Dangerous code. Use 'iAcceptFate' for files outside 'user://'");
                }
            }

            // Get the directory
            var dirResult = OpenDir(dirpath);
            if (dirResult.HasError) { return results.WithError(dirResult); }
            var dir = dirResult.Data;

            // Delete everything inside
            var contents = dir.ListAll();
            foreach (var item in contents)
            {
                if (item.EndsWith("/")) { results.Add(item, DeleteDir(item, iAcceptFate)); }
                else { results.Add(item, DeleteFile(item)); }
            }

            // Delete the dir itself (if emptied)
            if (results.IsOk) { results.WithError(dir.Remove(dirpath)); }
            return results;
        }


        /// <summary>
        ///   Copy a file from the provided path to the dest path.
        ///   Creates directories if needed.
        /// </summary>
        /// <param name="filepath">
        ///   Path to a file to copy.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="destFilepath">
        ///   Destination path for the file copy.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <returns>
        ///   Result with success or failure status set.
        ///   Will have Error.DoesNotExist if the file source file was missing.
        ///   Will forward the errors of 'EnsureDir' or Godot's 'CopyFile' otherwise.
        /// </returns>
        public static Result CopyFile(string filepath, string destFilepath, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref filepath, callerPath);
            LocalizeIfRelative(ref destFilepath, callerPath);
            var result = new Result($"Copy file '{ filepath }' to '{ destFilepath }'");
            if (filepath.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return result.WithError(Error.Bug, "Invalid Parameter:  Path was emtpy");
            }
            if (destFilepath.Empty())
            {
                GD.PushError("Invalid Parameter:  Dest was emtpy");
                return result.WithError(Error.Bug, "Invalid Parameter:  Dest was emtpy");
            }

            // open source
            var dir = new Directory();
            if (!dir.FileExists(filepath)) { return result.WithError(Error.DoesNotExist); }

            // ensure dest dir
            result.WithError(EnsureDir(destFilepath.GetBaseDir()));
            if (result.HasError) { return result; }

            // Perform the copy
            return result.WithError(dir.Copy(filepath, destFilepath));
        }


        /// <summary>
        ///   Copy a directory recursively from the provided path to the dest path.
        ///   Creates directories if needed.
        /// </summary>
        /// <remarks>
        ///   Because this is a recursive operation, it will error out called in Editor
        ///   or if the dest is not in the 'user://' directory. Use 'iAcceptFate' to
        ///   override those errors.
        /// </remarks>
        /// <param name="dirpath">
        ///   Path to a directory to copy.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="destDirpath">
        ///   Destination path for the file copy.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="iAcceptFate">
        ///   For some safety, this will produce an error if called in Editor or if
        ///   the dest is outside of the 'user://' folder. If true, errors will be
        ///   supressed.
        /// </param>
        /// <returns>
        ///   BatchResult with success or failure status set for each file and
        ///   directory operation.
        ///   Will have Error.Failed if 'iAcceptFate' is false and used dangrously.
        ///   Will contain all results from sub-operations.
        /// </returns>
        public static BatchResult CopyDir(string dirpath, string destDirpath, bool iAcceptFate = false, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref dirpath, callerPath);
            LocalizeIfRelative(ref destDirpath, callerPath);
            var results = new BatchResult($"Copy dir '{ dirpath }' to '{ destDirpath }'");
            if (dirpath.Empty())
            {
                GD.PushError("Invalid Parameter:  Dirpath was emtpy");
                return results.WithError(Error.Bug, "Invalid Parameter:  Dirpath was emtpy");
            }
            if (destDirpath.Empty())
            {
                GD.PushError("Invalid Parameter:  Dest was emtpy");
                return results.WithError(Error.Bug, "Invalid Parameter:  Dest was emtpy");
            }

            if (!dirpath.EndsWith("/")) { dirpath += "/"; }
            if (!destDirpath.EndsWith("/")) { destDirpath += "/"; }
            if (!iAcceptFate)
            {
                if (Engine.EditorHint)
                {
                    return results.WithError(Error.Failed, "Dangerous code. Use 'iAcceptFate' while in editor");
                }
                if (!dirpath.BeginsWith("user://"))
                {
                    return results.WithError(Error.Failed, "Dangerous code. Use 'iAcceptFate' for files outside 'user://'");
                }
            }

            // Get the source directory
            var dirResult = OpenDir(dirpath);
            if (dirResult.HasError) { return results.WithError(dirResult); }
            var dir = dirResult.Data;

            // Ensure dest directory
            results.SetError(EnsureDir(destDirpath));
            if (results.HasError) { return results; }

            // Copy everything inside
            var contents = dir.ListAll();
            foreach (var item in contents)
            {
                // Remove dirpath prefix
                var itemDest = destDirpath.PlusFile(RemovePrefix(item, dirpath));
                if (item.EndsWith("/")) { results.Add(item, CopyDir(item, itemDest, iAcceptFate)); }
                else { results.Add(item, CopyFile(item, itemDest)); }
            }

            return results;
        }



        /// <summary> Read a file as plain text </summary>
        /// <param name="filepath">
        ///   Path to a file to read as text.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <returns>
        ///   DataResult with the string contents on success or errors set on failure.
        /// </returns>
        public static DataResult<string> ReadText(string filepath, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref filepath, callerPath);
            var result = new DataResult<string>($"Read text from file '{ filepath }'");
            if (filepath.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return result.WithError(Error.Bug, "Invalid Parameter:  Path was emtpy");
            }

            var file = new File();
            result.SetError(file.Open(filepath, File.ModeFlags.Read), "Can't open file");
            if (result.HasError) { return result; }
            var text = file.GetAsText();
            var err = file.GetError();
            if (err != Error.Ok && err != Error.FileEof) { result.SetError(err); }
            else { result.Data = text; }
            file.Close();
            return result;
        }


        /// <summary>
        ///   Write plain text to a file. Creates directories if needed. Will overwrite.
        /// </summary>
        /// <param name="filePath">
        ///   Path to a file to write plain text to.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <returns>
        ///   Result with success or error status set.
        /// </returns>
        public static Result WriteText(string filepath, string text, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref filepath, callerPath);
            var result = new Result($"Write text to file '{ filepath }'");
            if (filepath.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return result.WithError(Error.Bug, "Invalid Parameter:  Path was emtpy");
            }

            // Make sure the dir exists
            result.SetError(EnsureBaseDir(filepath));
            if (result.HasError) { return result; }

            // Write the file
            var file = new File();
            result.SetError(file.Open(filepath, File.ModeFlags.Write), "Can't open file");
            if (result.HasError) { return result; }
            file.StoreString(text);
            var err = file.GetError();
            if (err != Error.Ok) // Can this be EOF?
            {
                result.SetError(err);
            }
            file.Close();
            return result;
        }

        #endregion ==== Directory and File Operations ====

        #region  Resource Loading/Saving 

        /// <summary>
        ///   Wrapper for 'ResourceLoader.Load'. Can take relative paths.
        /// </summary>
        /// <param name="path">
        ///   Path to a resource to load.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="typeHint"> String repr of resource type to load. </params>
        /// <param name="noCache"> If true, ignores cached resources and loads a new one. </params>
        /// <returns> The Resource or null if it doesn't exist or there was an error </returns>
        public static Resource? LoadResourceOrNull(string path, string typeHint = "", bool noCache = false, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref path, callerPath);
            if (path.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return null;
            }

            // If it doesn't exist, we are done
            if (noCache ? !FileExists(path) : !ResourceLoader.Exists(path)) { return null; }
            // Try loading the file
            var res = ResourceLoader.Load(path, typeHint, noCache);
            if (res != null) { return res; }
            // The file exists, but couldn't be read. Notify that it may be corrupt.
            GD.PushError($"Failed to load Resource '{path}', file may be corrupt or not a resource.");
            return null;
        }


        private static readonly object[] emptyObj = new object[0];
        /// <summary>
        ///   Wrapper for 'ResourceLoader.Load<>'. Can take relative paths.
        /// </summary>
        /// <param name="path">
        ///   Path to a resource to load.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="typeHint"> String repr of resource type to load. </params>
        /// <param name="noCache"> If true, ignores cached resources and loads a new one. </params>
        /// <returns> The Resource or null if it doesn't exist or there was an error </returns>
        public static TResource? LoadResourceOrNull<TResource>(string path, string typeHint = "", bool noCache = false, [CallerFilePath] string callerPath = "")
          where TResource : Resource
        {
            LocalizeIfRelative(ref path, callerPath);
            if (path.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return null;
            }

            // If it doesn't exist, we are done
            if (noCache ? !FileExists(path) : !ResourceLoader.Exists(path)) { return null; }
            // Try loading the file
            var res = ResourceLoader.Load<TResource>(path, typeHint, noCache);
            if (res != null)
            {
                if (IOSettings.CallReadyOnResources)
                {
                    var readyMethod = typeof(TResource).GetMethod("Ready", System.Type.EmptyTypes);
                    if (readyMethod != null)
                    {
                        try { readyMethod.Invoke(res, emptyObj); }
                        catch (System.Exception e) { GD.PushError($"Exception while calling 'Ready' on res '{path}':  {e}"); }
                    }
                }
                return res;
            }
            // The file exists, but couldn't be read. Notify that it may be corrupt.
            GD.PushError($"Failed to load Resource '{path}', file may be corrupt or not a resource.");
            return null;
        }


        /// <summary>
        ///   Wrapper for 'ResourceLoader.Load'.
        ///   Returns a DataResult with informative errors and can take relative paths.
        /// </summary>
        /// <param name="path">
        ///   Path to a resource to load.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="typeHint"> String repr of resource type to load. </params>
        /// <param name="noCache"> If true, ignores cached resources and loads a new one. </params>
        /// <returns>
        ///   DataResult with the resource on success or errors set on failure.
        ///   Has Error.FileCantOpen when the res doesn't exist.
        ///   Has Error.FileCantRead when the res returned is null, but the file does exist.
        /// </returns>
        public static DataResult<Resource> LoadResource(string path, string typeHint = "", bool noCache = false, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref path, callerPath);
            var result = new DataResult<Resource>($"Load resource from '{ path }'");
            if (path.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return result.WithError(Error.Bug, "Invalid Parameter:  Path was emtpy");
            }

            // If it doesn't exist, we are done
            if (noCache ? !FileExists(path) : !ResourceLoader.Exists(path)) { return result.WithError(Error.FileCantOpen); }
            // Try loading the file
            var res = ResourceLoader.Load(path, typeHint, noCache);
            if (res != null) { return result.WithData(res); }
            // Was probably corrupt or not a res at all
            return result.WithError(Error.FileCantRead);
        }


        /// <summary>
        ///   Wrapper for 'ResourceLoader.Load<>'.
        ///   Returns a DataResult with informative errors and can take relative paths.
        /// </summary>
        /// <param name="path">
        ///   Path to a resource to load.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="typeHint"> String repr of resource type to load. </params>
        /// <param name="noCache"> If true, ignores cached resources and loads a new one. </params>
        /// <returns>
        ///   DataResult with the resource on success or errors set on failure.
        ///   Has Error.FileCantOpen when the res doesn't exist.
        ///   Has Error.FileCantRead when the res returned is null, but the file does exist.
        /// </returns>
        public static DataResult<TResource> LoadResource<TResource>(string path, bool noCache = false, [CallerFilePath] string callerPath = "")
          where TResource : Resource
        {
            LocalizeIfRelative(ref path, callerPath);
            var result = new DataResult<TResource>($"Load resource from '{ path }'");
            if (path.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return result.WithError(Error.Bug, "Invalid Parameter:  Path was emtpy");
            }

            // If it doesn't exist, we are done
            if (noCache ? !new Directory().FileExists(path) : !ResourceLoader.Exists(path)) { return result.WithError(Error.FileCantOpen); }
            // Try loading the file
            var res = ResourceLoader.Load(path, "Resource", noCache);
            if (res is TResource tRes)
            {
                if (IOSettings.CallReadyOnResources)
                {
                    var readyMethod = typeof(TResource).GetMethod("Ready", System.Type.EmptyTypes);
                    if (readyMethod != null)
                    {
                        try { readyMethod.Invoke(res, emptyObj); }
                        catch (System.Exception e) { GD.PushError($"Exception while calling 'Ready' on res '{path}':  {e}"); }
                    }
                }
                return result.WithData(tRes);
            }
            // Failed... find out why
            // If it isn't null, then it just didn't match the script type
            if (res != null) { return result.WithError(Error.FileCantRead, $"Loaded Resource was not a '{ typeof(TResource).Name }'"); }
            // Was probably corrupt or not a res at all
            return result.WithError(Error.FileCantRead);
        }


        /// <summary>
        ///   Wrapper for 'ResourceLoader.Save'.
        ///   Returns a Result with informative errors, can take relative paths and tries to
        ///   make directories if they are missing.
        /// </summary>
        /// <param name="path">
        ///   Path to a resource to load.
        ///   Can take global, 'res://', or 'user://' paths.
        ///   Can take './' or '../' paths relative to calling script.
        ///   Paths with no base path indicator will be prefixed with 'res://'.
        /// </param>
        /// <param name="resource"> The resource object to save. </params>
        /// <param name="flags"> The save flags to pass to ResourceLoader. Default is 0. </params>
        /// <returns>
        ///   Result with success or failure state set.
        ///   Forwards errors from ResourceSaver.Save.
        /// </returns>
        public static Result SaveResource(string path, Resource resource, ResourceSaver.SaverFlags flags = 0, [CallerFilePath] string callerPath = "")
        {
            LocalizeIfRelative(ref path, callerPath);
            var result = new Result($"Save resource to '{ path }'");
            if (path.Empty())
            {
                GD.PushError("Invalid Parameter:  Path was emtpy");
                return result.WithError(Error.Bug, "Invalid Parameter:  Path was emtpy");
            }

            var basedirResult = EnsureBaseDir(path);
            if (basedirResult.HasError)
            {
                return result.WithError(basedirResult);
            }
            return result.WithError(ResourceSaver.Save(path, resource, flags));
        }

        #endregion ==== Resource Loading/Saving ====
    };
}
