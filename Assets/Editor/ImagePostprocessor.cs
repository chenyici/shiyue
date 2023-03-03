using System.IO;
using UnityEditor;

public class ImagePostprocessor : AssetPostprocessor {

    private const string PlatformStandalone = "Standalone";
    private const string PlatformIphone = "iPhone";
    private const string PlatformAndroid = "Android";
    private const int MaxTexSize = 1024;

    private void OnPreprocessTexture() {

        string name = Path.GetFileNameWithoutExtension(assetPath);

        if (name.EndsWith("_TEX")) {
            TextureImporter textureImporter = (TextureImporter)base.assetImporter;
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.alphaIsTransparency = true;
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = false;
            SetCompressedASTC4x4PlatformTextureSettings(textureImporter);
        } 
    }

    private TextureImporterPlatformSettings ASTC4x4(string platform) {
        TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
        settings.maxTextureSize = MaxTexSize;
        settings.format = TextureImporterFormat.ASTC_4x4;
        settings.name = platform;
        settings.overridden = true;
        return settings;
    }

    private void SetCompressedASTC4x4PlatformTextureSettings(TextureImporter textureImporter) {
        //textureImporter.SetPlatformTextureSettings(ASTC4x4(PlatformStandalone));
        textureImporter.SetPlatformTextureSettings(ASTC4x4(PlatformAndroid));
        textureImporter.SetPlatformTextureSettings(ASTC4x4(PlatformIphone));
    }
}
