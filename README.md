# Fractural Commons 🔗

![Deploy](https://github.com/Fractural/FracturalCommons/actions/workflows/deploy.yml/badge.svg) ![Unit Tests](https://github.com/Fractural/FracturalCommons/actions/workflows/tests.yml/badge.svg)


Collection of commonly used C# scripts, nodes, and other content for Godot. Serves as a library to build Godot games and editor plugins from.

## Features

- Curves - Preset curve assets ready to be used
- Custom types
  - `Vector2Int` - Vector2 but with integer components
  - `GDScriptWrapper` - Can be extended to provide a typed wrappere around a GDScript godot object
  - `PropertyListBuilder` - A builder object for making property lists
  - `PropertyListItem` - Used by the `PropertyListBuilder` and represents an item in the property list
- IO - Utility methods to load resources, including from relative paths 
- `ExtendedPlugin` - Abstract base class for plugin development
  - Can load `SubPlugins`
  - Can add managed controls to docks that are automatically freed
  - Plugin is automatically reloaded on solution rebuild
- Custom plugin components
  - `PluginAssetsRegistry` -Handles loading of plugin assets, making sure to scale them with the screen size. This ensures the plugin looks right on High DPI displays.
  - `NodeSelectDialog` - Dialog that lets you select a node from the subtree of a given node 
- Utility classes
  - `BitUtils` - Utils for bit manipulation
  - `ByteSerializationUtils` - Utils for serializing to and from bytes
  - `CollectionUtils` - Utils for C# collections
  - `DirectoryUtils` - Utils for file directory traversal, using the `Godot.Directory` object.
  - `DrawUtils` - Utils for manually drawing in CanvasItem nodes
  - `EditorHackUtils` - Utils for peeking around the tree structure of the GodotEditor itself
  - `EditorUtils` - Utils for editor plugins
  - `EngineUtils` - Utils for setting up metadata about the engine, including version and preprocessor defines
    - `VersionInfo` - Struct that stores data about an engine version.
  - `FileUtils` - Utils for file and directory fetching
  - `GDCollectionUtils` - Utils for Godot collections
  - `GDUtils` - Utils for Godot objects
  - `GeneralUtils` - Utils for general C# stuff
  - `NodeUtils` - Utils for nodes
  - `RectUtils` - Utils for `Rect2`
  - `Reflectionutils` - Utils for C# reflection
  - `StringUtils` - Utils for strings
  - `VectorUtils` - Utils for vectors
- `FracturalCommonsPlugin` - Plugin for Fractural Commons, which contains multiple sub plugins
  - `MainPlugin` - Adds preprocessor defines for the Godot version to allow plugins to adjust their behaviour depending on the version being run on.
  - `EditorUtilsPlugin` - Allows some of `EditorUtils`' functionality to work, including having a black tint show up when a window dialog is popped up.
  - `CSharpResourceRegistryPlugin` - Registers custom C# nodes and resources to show up in the Create Node and Create Resource menus 
    - Custom C# nodes and resources should use the `RegisteredType` attribute.
    - Also adds a `CSRG` (CSharp Resource Registry) button to the top right to let you refresh C# resources manually.