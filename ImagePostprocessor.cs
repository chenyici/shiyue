using System.IO;
using UnityEditor;

public class ImagePostprocessor : AssetPostprocessor {

    private const string PlatformStandalone = "Standalone";
    private const string PlatformIphone = "iPhone";
    private const string PlatformAndroid = "Android";
    private const int MaxTexSize = 1024;

    private void OnPreprocessTexture() {

        string name = Path.GetFileNameWithoutExtension(assetPath);

        TextureImporter textureImporter = (TextureImporter) base.assetImporter;
        textureImporter.textureType = TextureImporterType.Default;
        textureImporter.mipmapEnabled = false;
        textureImporter.mipMapBias = -1;
        textureImporter.anisoLevel = 1;
        textureImporter.alphaIsTransparency = false;
        TextureImporterSettings initialSettings = new TextureImporterSettings();
        textureImporter.ReadTextureSettings(initialSettings);
        initialSettings.spriteGenerateFallbackPhysicsShape = false;
        textureImporter.SetTextureSettings(initialSettings);

        if (name.EndsWith("_TEX_RGB")) {
            textureImporter.npotScale = TextureImporterNPOTScale.None;
            SetRGBCrunchedPlatformTextureSettings(textureImporter);
        } else if (name.EndsWith("_UI_TEX_NOATLAS")) { 
            textureImporter.npotScale = TextureImporterNPOTScale.None;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.alphaIsTransparency = true;
            SetCompressedASTC5x5PlatformTextureSettings(textureImporter);
        } else {

        }

        //textureImporter.mipmapEnabled = true;
    }

    private TextureImporterPlatformSettings ASTC5x5(string platform) {
        TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
        settings.maxTextureSize = MaxTexSize;
        settings.format = TextureImporterFormat.ASTC_RGBA_5x5;
        settings.name = platform;
        settings.overridden = true;
        return settings;
    }

    private TextureImporterPlatformSettings Crunched(string platform) {
        TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
        switch (platform) {
            case PlatformStandalone:
                settings.format = TextureImporterFormat.Automatic;
                break;
            case PlatformAndroid:
                settings.format = TextureImporterFormat.ETC_RGB4Crunched;
                break;
            case PlatformIphone:
                settings.format = TextureImporterFormat.PVRTC_RGB4;
                break;
        }

        settings.maxTextureSize = MaxTexSize;
        settings.name = platform;
        settings.compressionQuality = 100;
        settings.allowsAlphaSplitting = false;
        settings.overridden = true;

        return settings;
    }


    private void SetCompressedASTC5x5PlatformTextureSettings(TextureImporter textureImporter) {
        textureImporter.SetPlatformTextureSettings(ASTC5x5(PlatformStandalone));
        textureImporter.SetPlatformTextureSettings(ASTC5x5(PlatformAndroid));
        textureImporter.SetPlatformTextureSettings(ASTC5x5(PlatformIphone));
    }

    private void SetRGBCrunchedPlatformTextureSettings(TextureImporter textureImporter) {
        textureImporter.SetPlatformTextureSettings(Crunched(PlatformStandalone));
        textureImporter.SetPlatformTextureSettings(Crunched(PlatformAndroid));
        textureImporter.SetPlatformTextureSettings(Crunched(PlatformIphone));
    }
}
