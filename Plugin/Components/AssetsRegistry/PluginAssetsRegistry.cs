using System.Collections.ObjectModel;
using System;
using Godot;
using System.Collections.Generic;

namespace Fractural.Plugin.AssetsRegistry
{
	public interface IAssetsRegistry
	{
		float Scale { get; }

		/// <summary>
		/// Loads the scaled veresion of an asset from a
		/// given path.
		/// </summary>
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T LoadAsset<T>(string path) where T : Godot.Object;

		/// <summary>
		/// Loads the scaled version of an existing asset.
		/// </summary>
		/// <param name="asset"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T LoadAsset<T>(T asset) where T : Godot.Object;
	}

	/// <summary>
	/// Used for stanalone builds, where the scale is set
	/// by the developer.
	/// </summary>
	public class DefaultAssetsRegistry : Godot.Reference, IAssetsRegistry
	{
		public float Scale { get; set; } = 1;
		public Dictionary<Godot.Object, Godot.Object> LoadedAssets { get; private set; } = new Dictionary<Godot.Object, Godot.Object>();
		private List<AssetProcessor> processors;

		public DefaultAssetsRegistry()
		{
			processors = new List<AssetProcessor> {
				new TextureProcessor(this),
				new DynamicFontProcessor(this),
			};
		}

		public DefaultAssetsRegistry(List<AssetProcessor> processors)
		{
			this.processors = processors;

			// Prime each processor for use.
			foreach (AssetProcessor processor in this.processors)
				processor.AssetsRegistry = this;
		}

		public void AddAssetProcessor(AssetProcessor processor)
		{
			processor.AssetsRegistry = this;
			processors.Add(processor);
		}

		public bool RemoveAssetProcessor(AssetProcessor processor)
		{
			return processors.Remove(processor);
		}

		public ReadOnlyCollection<AssetProcessor> GetAssetProcessors()
		{
			return new ReadOnlyCollection<AssetProcessor>(processors);
		}

		public T LoadAsset<T>(string path) where T : Godot.Object
		{
			return LoadAsset<T>(ResourceLoader.Load<T>(path));
		}

		public T LoadAsset<T>(T asset) where T : Godot.Object
		{
			if (LoadedAssets.ContainsKey(asset))
				return (T)LoadedAssets[asset];
			foreach (AssetProcessor processor in processors)
			{
				if (processor.CanProcess(asset))
				{
					T result = (T)processor.Process(asset);
					LoadedAssets.Add(asset, result);
					return result;
				}
			}
			GD.PushError($"Cannot scale asset of type {asset.GetType().FullName}");
			return default(T);
		}
	}

#if TOOLS
	/// <summary>
	/// Used in the editor, where the scale is retreived
	/// from the editor's settings.
	/// </summary>
	public class EditorAssetsRegistry : Godot.Reference, IAssetsRegistry
	{
		private EditorPlugin plugin;

		private float cachedEditorScale = -1;
		public float Scale
		{
			get
			{
#if GODOT_3_3_0_OR_NEWER
				return plugin.GetEditorInterface().GetEditorScale();
#else
				if (cachedEditorScale == -1)
				{
#if GODOT_3_0_0_OR_NEWER
					return CalculateCurrentEditorScale_3_0();
#elif GODOT_3_1_0_OR_NEWER
					return CalculateCurrentEditorScale_3_1(); 
#else
					GD.PushError($"Could not fetch editor scale for the current Godot version. ({Fractural.Utils.EngineUtils.CurrentVersionInfo})");
					return 1;
#endif
				} else
					return cachedEditorScale; 
#endif
			}
		}

		private DefaultAssetsRegistry assetsRegistry = new DefaultAssetsRegistry();

		public EditorAssetsRegistry(EditorPlugin plugin)
		{
			this.plugin = plugin;
		}

		public T LoadAsset<T>(string path) where T : Godot.Object
		{
			assetsRegistry.Scale = Scale;
			return assetsRegistry.LoadAsset<T>(path);
		}

		public T LoadAsset<T>(T asset) where T : Godot.Object
		{
			assetsRegistry.Scale = Scale;
			return assetsRegistry.LoadAsset<T>(asset);
		}

#if GODOT_3_0_0_OR_NEWER
		[Obsolete]
		private float CalculateCurrentEditorScale_3_0()
		{
			EditorSettings editorSettings = plugin.GetEditorInterface().GetEditorSettings();

			int displayScale = (int)editorSettings.GetSetting("interface/editor/display_scale");
			float customDisplayScale = (int)editorSettings.GetSetting("interface/editor/custom_display_scale");

			switch (displayScale)
			{
				case 0:
					if (OS.GetName() == "OSX")
						return OS.GetScreenMaxScale();
					else
					{
						int screen = OS.GetCurrentScreen();
						if (OS.GetScreenDpi(screen) >= 192 && OS.GetScreenSize(screen).x > 2000)
							return 2.0f;
						else
							return 1.0f;
					}
				case 1:
					return 0.75f;
				case 2:
					return 1.0f;
				case 3:
					return 1.25f;
				case 4:
					return 1.5f;
				case 5:
					return 1.5f;
				case 6:
					return 2.0f;
				default:
					return customDisplayScale;
			}
		}
#endif

#if GODOT_3_1_0_OR_NEWER
		[Obsolete]
		private float CalculateCurrentEditorScale_3_1()
		{
			EditorSettings editorSettings = plugin.GetEditorInterface().GetEditorSettings();

			int dpiMode = (int)editorSettings.GetSetting("interface/editor/hidpi_mode");

			switch (dpiMode)
			{
				case 0:
					int screen = OS.GetCurrentScreen();
					if (OS.GetScreenDpi(screen) >= 192 && OS.GetScreenSize(screen).x > 2000)
						return 2.0f;
					else
						return 1.0f;
				case 1:
					return 0.75f;
				case 2:
					return 1.0f;
				case 3:
					return 1.5f;
				case 4:
					return 2.0f;
				default:
					return 1;
			}
		}
#endif
	}
#endif
}