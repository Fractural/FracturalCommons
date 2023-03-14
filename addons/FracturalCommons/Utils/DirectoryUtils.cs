using Godot;
using Godot.Collections;

namespace Fractural.Utils
{
    /// See `IO` for most directory/path helpers
    public static class DirectoryUtils
    {
        // List all the files in a given directory.
        // Suffixes can be string or array of suffixes to search for
        public static Array<string> ListFiles(this Directory dir, params string[] suffixes)
        {
            var results = new Array<string>();
            var err = dir.ListDirBegin(true, false);
            if (err != Error.Ok)
            {
                GD.PushError($"List files failed (Err: { err }). (Did you open dir before calling ListFiles?)");
                return results;
            }

            while (true)
            {
                var filename = dir.GetNext();
                if (filename == "") { break; }
                if (dir.CurrentIsDir()) { continue; }
                if (suffixes.Length == 0) { results.Add(dir.GetCurrentDir().PlusFile(filename)); }
                else
                {
                    foreach (var suffix in suffixes)
                    {
                        if (filename.EndsWith(suffix))
                        {
                            results.Add(dir.GetCurrentDir().PlusFile(filename));
                        }
                    }
                }
            }
            return results;
        }


        // List all directories in a given directory.
        // Does not append '/' unless requested
        public static Array<string> ListDirs(this Directory dir, bool appendSlash = false)
        {
            var results = new Array<string>();
            var err = dir.ListDirBegin(true, false);
            if (err != Error.Ok)
            {
                GD.PushError($"List dirs failed (Err: { err }). (Did you open dir before calling ListDirs?)");
                return results;
            }

            while (true)
            {
                var filename = dir.GetNext();
                if (filename == "") { break; }
                if (!dir.CurrentIsDir()) { continue; }
                if (appendSlash) { filename += "/"; }
                results.Add(dir.GetCurrentDir().PlusFile(filename));
            }
            return results;
        }

        // List all the files and directories in a given directory.
        // Directories will end with a '/', files will not
        public static Array<string> ListAll(this Directory dir)
        {
            var results = new Array<string>();
            var err = dir.ListDirBegin(true, false);
            if (err != Error.Ok)
            {
                GD.PushError($"List dirs failed (Err: { err }). (Did you open dir before calling ListAll?)");
                return results;
            }

            while (true)
            {
                var filename = dir.GetNext();
                if (filename == "") { break; }
                if (dir.CurrentIsDir()) { filename += "/"; }
                results.Add(dir.GetCurrentDir().PlusFile(filename));
            }
            return results;
        }
    };
}
