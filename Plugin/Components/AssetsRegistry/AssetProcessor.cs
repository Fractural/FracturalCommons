using Godot;

namespace Fractural.Plugin.AssetsRegistry
{
	public abstract class AssetProcessor : Godot.Reference
	{
		public IAssetsRegistry AssetsRegistry { get; set; }

		public abstract bool CanProcess(object asset);
		public abstract object Process(object asset);

		public AssetProcessor() { }

		public AssetProcessor(IAssetsRegistry assetsRegistry)
		{
			AssetsRegistry = assetsRegistry;
		}
	}

	public class DynamicFontProcessor : AssetProcessor
	{
		public DynamicFontProcessor() { }
		public DynamicFontProcessor(IAssetsRegistry assetsRegistry) : base(assetsRegistry) { }

		public override bool CanProcess(object asset)
		{
			return asset is DynamicFont;
		}

		public override object Process(object asset)
		{
			DynamicFont castedAsset = (DynamicFont)asset;
			DynamicFont duplicate = (DynamicFont)castedAsset.Duplicate();
			duplicate.Size = (int)Mathf.Round(duplicate.Size * AssetsRegistry.Scale);
			return duplicate;
		}
	}

	public class TextureProcessor : AssetProcessor
	{
		public TextureProcessor() { }
		public TextureProcessor(IAssetsRegistry assetsRegistry) : base(assetsRegistry) { }

		public override bool CanProcess(object asset)
		{
			return asset is Texture;
		}

		public override object Process(object asset)
		{
			Texture castedAsset = (Texture)asset;
			Image image = castedAsset.GetData();
			image.Resize((int)Mathf.Round(image.GetWidth() * AssetsRegistry.Scale), (int)Mathf.Round(image.GetHeight() * AssetsRegistry.Scale));
			ImageTexture scaledTexture = new ImageTexture();
			scaledTexture.CreateFromImage(image);
			return scaledTexture;
		}
	}
}