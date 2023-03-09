using Fractural.Plugin;
using Godot;
using System;
using System.Collections.Generic;
using GDC = Godot.Collections;
using static Godot.EditorPlugin;
using System.Linq;
using Fractural.Utils;
using System.Reflection;

#if TOOLS
namespace Fractural.Commons
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class RegisteredTypeAttribute : System.Attribute
    {
        public string name;
        public string iconPath;
        public string baseType;

        public RegisteredTypeAttribute(string name, string iconPath = "", string baseType = "")
        {
            this.name = name;
            this.iconPath = iconPath;
            this.baseType = baseType;
        }
    }

    public class CSharpResourceRegistryPlugin : SubPlugin
    {
        public static class Settings
        {
            public static CSharpResourceRegistryPlugin Plugin { get; private set; }
            public enum ResourceSearchType
            {
                Recursive = 0,
                Namespace = 1,
            }

            public static string ClassPrefix => Plugin.GetSetting<string>(nameof(ClassPrefix));
            public static ResourceSearchType SearchType => Plugin.GetSetting<ResourceSearchType>(nameof(SearchType));
            public static IReadOnlyCollection<string> ResourceScriptDirectories => Plugin.GetSetting<GDC.Array>(nameof(ResourceScriptDirectories)).ToArray<string>();

            public static void Init(CSharpResourceRegistryPlugin plugin)
            {
                Plugin = plugin;
                Plugin.AddSetting(nameof(ClassPrefix), Variant.Type.String, "");
                Plugin.AddSetting(nameof(SearchType), Variant.Type.Int, ResourceSearchType.Recursive, PropertyHint.Enum, "Recursive,Namespace");
                Plugin.AddSetting(nameof(ResourceScriptDirectories), Variant.Type.StringArray, new GDC.Array<string>(new string[] { "res://" }));
            }
        }

        public override string PluginName => "C# Resource Registry";

        // We're not going to hijack the Mono Build button since it actually takes time to build
        // and we can't be sure how long that is. I guess we have to leave refreshing to the user for now.
        // There isn't any automation we can do to fix that.
        // private Button MonoBuildButton => GetNode<Button>("/root/EditorNode/@@580/@@581/@@589/@@590/ToolButton");
        private readonly List<string> customTypes = new List<string>();
        private Button refreshButton;

        public override void Load()
        {
            Settings.Init(this);

            refreshButton = new Button();
            refreshButton.Text = "CSRG";

            Plugin.AddManagedControlToContainer(CustomControlContainer.Toolbar, refreshButton);
            refreshButton.Icon = refreshButton.GetIcon("Reload", "EditorIcons");
            refreshButton.Connect("pressed", this, nameof(OnRefreshPressed));

            RefreshCustomClasses();
        }

        public override void Unload()
        {
            UnregisterCustomClasses();
            if (refreshButton != null)
            {
                Plugin.DestroyManagedControl(refreshButton);
                refreshButton.QueueFree();
                refreshButton = null;
            }
        }

        public void RefreshCustomClasses()
        {
            Print("Refreshing Registered Resources...");
            UnregisterCustomClasses();
            RegisterCustomClasses();
        }

        private void RegisterCustomClasses()
        {
            customTypes.Clear();

            File file = new File();

            foreach (Type type in GetCustomRegisteredTypes())
                if (type.IsSubclassOf(typeof(Resource)))
                    AddRegisteredType(type, nameof(Resource), file);
                else
                    AddRegisteredType(type, nameof(Node), file);
        }

        private void AddRegisteredType(Type type, string defaultBaseTypeName, File file)
        {
            RegisteredTypeAttribute attribute = (RegisteredTypeAttribute)Attribute.GetCustomAttribute(type, typeof(RegisteredTypeAttribute));
            String path = FindClassPath(type);
            if (path == null && !file.FileExists(path))
                return;

            Script script = GD.Load<Script>(path);
            if (script == null)
                return;

            string baseTypeName = defaultBaseTypeName;
            if (attribute.baseType != "")
                baseTypeName = attribute.baseType;

            ImageTexture icon = null;
            string iconPath = attribute.iconPath;
            if (iconPath == "")
            {
                Type baseType = type.BaseType;
                while (baseType != null)
                {
                    RegisteredTypeAttribute baseTypeAttribute = (RegisteredTypeAttribute)Attribute.GetCustomAttribute(baseType, typeof(RegisteredTypeAttribute));
                    if (baseTypeAttribute != null && baseTypeAttribute.iconPath != "")
                    {
                        iconPath = baseTypeAttribute.iconPath;
                        break;
                    }
                    baseType = baseType.BaseType;
                }
            }

            if (iconPath != "")
            {
                if (file.FileExists(iconPath))
                {
                    Texture rawIcon = ResourceLoader.Load<Texture>(iconPath);
                    if (rawIcon != null)
                    {
                        Image image = rawIcon.GetData();
                        int length = (int)Mathf.Round(16 * Plugin.GetEditorInterface().GetEditorScale());
                        image.Resize(length, length);
                        icon = new ImageTexture();
                        icon.CreateFromImage(image);
                    }
                    else
                        GD.PushError($"Could not load the icon for the registered type \"{type.FullName}\" at path \"{path}\".");
                }
                else
                    GD.PushError($"The icon path of \"{path}\" for the registered type \"{type.FullName}\" does not exist.");
            }

            Plugin.AddCustomType($"{Settings.ClassPrefix}{type.Name}", baseTypeName, script, icon);
            customTypes.Add($"{Settings.ClassPrefix}{type.Name}");
            Print($"Registered custom type: {type.Name} -> {path}");
        }

        private static string FindClassPath(Type type)
        {
            switch (Settings.SearchType)
            {
                case Settings.ResourceSearchType.Recursive:
                    return FindClassPathRecursive(type);
                case Settings.ResourceSearchType.Namespace:
                    return FindClassPathNamespace(type);
                default:
                    throw new Exception($"ResourceSearchType {Settings.SearchType} not implemented!");
            }
        }

        private static string FindClassPathNamespace(Type type)
        {
            foreach (string dir in Settings.ResourceScriptDirectories)
            {
                string filePath = $"{dir}/{type.Namespace?.Replace(".", "/") ?? ""}/{type.Name}.cs";
                File file = new File();
                if (file.FileExists(filePath))
                    return filePath;
            }
            return null;
        }

        private static string FindClassPathRecursive(Type type)
        {
            foreach (string directory in Settings.ResourceScriptDirectories)
            {
                string fileFound = FindClassPathRecursiveHelper(type, directory);
                if (fileFound != null)
                    return fileFound;
            }
            return null;
        }

        private static string FindClassPathRecursiveHelper(Type type, string directory)
        {
            Directory dir = new Directory();

            if (dir.Open(directory) == Error.Ok)
            {
                dir.ListDirBegin();

                while (true)
                {
                    var fileOrDirName = dir.GetNext();

                    // Skips hidden files like .
                    if (fileOrDirName == "")
                        break;
                    else if (fileOrDirName.BeginsWith("."))
                        continue;
                    else if (dir.CurrentIsDir())
                    {
                        string foundFilePath = FindClassPathRecursiveHelper(type, dir.GetCurrentDir() + "/" + fileOrDirName);
                        if (foundFilePath != null)
                        {
                            dir.ListDirEnd();
                            return foundFilePath;
                        }
                    }
                    else if (fileOrDirName == $"{type.Name}.cs")
                        return dir.GetCurrentDir() + "/" + fileOrDirName;
                }
            }
            return null;
        }

        private static IEnumerable<Type> GetCustomRegisteredTypes()
        {
            var assembly = Assembly.GetAssembly(typeof(Plugin));
            return assembly.GetTypes().Where(t => !t.IsAbstract
                && Attribute.IsDefined(t, typeof(RegisteredTypeAttribute))
                && (t.IsSubclassOf(typeof(Node)) || t.IsSubclassOf(typeof(Resource)))
                );
        }

        private void UnregisterCustomClasses()
        {
            foreach (var script in customTypes)
            {
                Plugin.RemoveCustomType(script);
                Print($"Unregister custom resource: {script}");
            }

            customTypes.Clear();
        }

        private void OnRefreshPressed()
        {
            RefreshCustomClasses();
        }
    }
}
#endif